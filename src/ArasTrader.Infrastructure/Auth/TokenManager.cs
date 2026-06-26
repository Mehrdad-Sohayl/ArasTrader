using ArasTrader.Application.Common;
using ArasTrader.Application.Interfaces;
using ArasTrader.Application.Interfaces.Repositories;
using ArasTrader.Application.Models;
using ArasTrader.Infrastructure.ExternalApis.ArasApi;
using ArasTrader.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace ArasTrader.Infrastructure.Auth;

internal class TokenManager : ITokenManager
{
    private readonly ITokenStore _tokenStore;
    private readonly IAuthTokenRepository _authTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IArasApiClient _arasApiClient;
    private readonly ArasApiOptions _arasApiOptions;

    private static readonly SemaphoreSlim _lock = new(1, 1);

    public TokenManager(
        ITokenStore tokenStore,
        IAuthTokenRepository authTokenRepository,
        IUnitOfWork unitOfWork,
        IArasApiClient arasApiClient,
        IOptions<ArasApiOptions> arasApiOptions)
    {
        _tokenStore = tokenStore;
        _authTokenRepository = authTokenRepository;
        _unitOfWork = unitOfWork;
        _arasApiClient = arasApiClient;
        _arasApiOptions = arasApiOptions.Value;
    }

    public async Task<Result<string>> GetValidTokenAsync(CancellationToken cancellationToken = default)
    {
        var cached = await LoadTokenAsync(cancellationToken);

        if (cached != null && !IsExpired(cached))
            return Result<string>.Success(cached.AccessToken);

        await _lock.WaitAsync();

        try
        {
            cached = await LoadTokenAsync(cancellationToken);
            if (cached != null && !IsExpired(cached))
                return Result<string>.Success(cached.AccessToken);

            if (cached != null)
                return await RefreshTokenAsync(cached, cancellationToken);

            return await LoginAndPersistAsync(cancellationToken);
        }
        finally
        {
            _lock.Release();
        }
    }

    private bool IsExpired(TokenState token)
    {
        return token.ExpiresAtUtc < DateTime.UtcNow.AddMinutes(5);
    }


    private async Task<Result<TokenState>> ExecuteTokenOperationAsync(
    Func<Task<TokenState>> operation)
    {
        const int maxRetries = 3;

        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                var token = await operation();
                return Result<TokenState>.Success(token);
            }
            catch (Exception ex) when (IsTransient(ex) && attempt < maxRetries)
            {
                await Task.Delay(1000 * attempt);
            }
            catch (Exception ex)
            {
                return Result<TokenState>.Failure(
                    new ApplicationError(
                        ApplicationErrorCodes.CannotRetriveToken,
                        ex.Message));
            }
        }

        return Result<TokenState>.Failure(
            new ApplicationError(
                ApplicationErrorCodes.CannotRetriveToken,
                ApplicationErrorCodes.CannotRetriveToken));
    }


    private Task<Result<TokenState>> LoginAsync()
    {
        return ExecuteTokenOperationAsync(async () =>
        {
            var response = await _arasApiClient.GetTokenAsync(new LoginRequest
            {
                Username = _arasApiOptions.Username,
                Password = _arasApiOptions.Password
            });

            return new TokenState
            {
                AccessToken = response.AccessToken,
                ExpiresAtUtc = response.ExpiresAtUtc
            };
        });
    }

    private async Task<Result<string>> LoginAndPersistAsync(
    CancellationToken cancellationToken)
    {
        var loginResult = await LoginAsync();

        if (!loginResult.IsSuccess)
            return Result<string>.Failure(loginResult.Errors!.First());

        var token = loginResult.Value;

        await PersistTokenAsync(token, cancellationToken);

        return Result<string>.Success(token.AccessToken);
    }

    private Task<Result<TokenState>> RefreshAsync(RefreshTokenRequest refreshTokenRequest)
    {
        return ExecuteTokenOperationAsync(async () =>
        {
            var response = await _arasApiClient.RefreshTokenAsync(refreshTokenRequest);

            return new TokenState
            {
                AccessToken = response.AccessToken,
                ExpiresAtUtc = response.ExpiresAtUtc
            };

        });
    }

    private async Task<Result<string>> RefreshTokenAsync(
    TokenState token,
    CancellationToken cancellationToken)
    {
        var refreshResult = await RefreshAsync(new RefreshTokenRequest
        {
            Token = token.AccessToken
        });

        if (!refreshResult.IsSuccess)
            return Result<string>.Failure(refreshResult.Errors!.First());

        token.AccessToken = refreshResult.Value.AccessToken;
        token.ExpiresAtUtc = refreshResult.Value.ExpiresAtUtc;

        await PersistTokenAsync(token, cancellationToken);

        return Result<string>.Success(token.AccessToken);
    }

    private async Task<TokenState?> LoadTokenAsync(CancellationToken cancellationToken)
    {
        var token = _tokenStore.Get();

        if (token != null)
            return token;

        token = await _authTokenRepository.GetAsync(cancellationToken);

        if (token != null)
            _tokenStore.Save(token);

        return token;
    }

    private async Task PersistTokenAsync(
        TokenState token,
        CancellationToken cancellationToken)
    {
        _tokenStore.Save(token);

        await _authTokenRepository.UpsertAsync(token, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private bool IsTransient(Exception ex)
    {
        return ex is HttpRequestException
            || ex is TaskCanceledException;
    }
}
