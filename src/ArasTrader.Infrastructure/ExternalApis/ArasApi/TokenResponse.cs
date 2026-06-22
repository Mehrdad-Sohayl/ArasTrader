namespace ArasTrader.Infrastructure.ExternalApis.ArasApi;

public class TokenResponse
{
    public string AccessToken { get; set; }
    public string TokenType { get; set; }
    public int ExpiresInSeconds { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
}
