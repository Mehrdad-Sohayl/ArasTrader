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
        var cached = _tokenStore.Get();

        if (cached != null && !IsExpired(cached))
            return Result<string>.Success(cached.AccessToken);

        await _lock.WaitAsync();

        try
        {
            cached = _tokenStore.Get();
            if (cached == null)
                cached = await _authTokenRepository.GetAsync(cancellationToken);
            if (cached != null && !IsExpired(cached))
                return Result<string>.Success(cached.AccessToken);

            var newToken = await LoginAsync();
            if (newToken == null || !newToken.IsSuccess)
                return Result<string>.Failure(newToken!.Errors!.First());

            _tokenStore.Save(newToken.Value);
            await _authTokenRepository.AddAsync(newToken.Value, cancellationToken);
            await _unitOfWork.SaveChangesAsync();
            return Result<string>.Success(newToken.Value.AccessToken);
        }
        finally
        {
            _lock.Release();
        }
    }

    private bool IsExpired(TokenState token)
    {
        return token.ExpiresAtUtc < DateTime.UtcNow.AddSeconds(30);
    }

    private async Task<Result<TokenState>> LoginAsync()
    {
        const int maxRetries = 3;

        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                var response = await _arasApiClient.GetTokenAsync(new LoginRequest
                {
                    Username = _arasApiOptions.Username,
                    Password = _arasApiOptions.Password
                });

                var result = new TokenState
                {
                    AccessToken = response.AccessToken,
                    ExpiresAtUtc = response.ExpiresAtUtc
                };

                return Result<TokenState>.Success(result);

            }
            catch (Exception ex) when (IsTransient(ex) && attempt < maxRetries)
            {
                await Task.Delay(1000 * attempt);
            }
            catch (Exception ex)
            {
                return Result<TokenState>.Failure(new ApplicationError(ApplicationErrorCodes.CannotRetriveToken, ex.Message));
            }

        }

        return Result<TokenState>.Failure(new ApplicationError(ApplicationErrorCodes.CannotRetriveToken, ApplicationErrorCodes.CannotRetriveToken));
    }

    private bool IsTransient(Exception ex)
    {
        return ex is HttpRequestException
            || ex is TaskCanceledException;
    }
}
