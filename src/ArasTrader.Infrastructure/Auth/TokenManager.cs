using ArasTrader.Application.Interfaces;
using ArasTrader.Application.Models;
using ArasTrader.Infrastructure.ExternalApis.ArasApi;
using ArasTrader.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace ArasTrader.Infrastructure.Auth;

internal class TokenManager : ITokenManager
{
    private readonly ITokenStore _tokenStore;
    private readonly IArasApiClient _arasApiClient;
    private readonly ArasApiOptions _arasApiOptions;

    private static readonly SemaphoreSlim _lock = new(1, 1);

    public TokenManager(
        ITokenStore tokenStore,
        IArasApiClient arasApiClient,
        IOptions<ArasApiOptions> arasApiOptions)
    {
        _tokenStore = tokenStore;
        _arasApiClient = arasApiClient;
        _arasApiOptions = arasApiOptions.Value;
    }

    public async Task<string> GetValidTokenAsync()
    {
        var cached = _tokenStore.Get();

        if (cached != null && !IsExpired(cached))
            return cached.AccessToken;

        await _lock.WaitAsync();

        try
        {
            cached = _tokenStore.Get();
            if (cached != null && !IsExpired(cached))
                return cached.AccessToken;

            var newToken = await LoginAsync();

            _tokenStore.Save(newToken);

            return newToken.AccessToken;
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

    private async Task<TokenState> LoginAsync()
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
                return new TokenState
                {
                    AccessToken = response.AccessToken,
                    ExpiresAtUtc = response.ExpiresAtUtc
                };

            }
            catch (Exception ex) when (IsTransient(ex) && attempt < maxRetries)
            {
                await Task.Delay(1000 * attempt);
            }
        }

        throw new Exception("Failed to retrive token after retries.");

    }

    private bool IsTransient(Exception ex)
    {
        return ex is HttpRequestException
            || ex is TaskCanceledException;
    }
}
