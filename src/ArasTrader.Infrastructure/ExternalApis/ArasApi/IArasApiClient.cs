using Refit;

namespace ArasTrader.Infrastructure.ExternalApis.ArasApi;

public interface IArasApiClient
{
    [Post("/api/auth/token")]
    Task<TokenResponse> GetTokenAsync([Body] LoginRequest request);

    [Post("/api/auth/refresh")]
    Task<TokenResponse> RefreshTokenAsync([Body] RefreshTokenRequest request);

    [Get("/api/customers")]
    Task<List<CustomerResponse>> GetCustomersAsync([Header("Authorization")] string accessToken);
}
