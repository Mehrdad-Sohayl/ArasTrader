namespace ArasTrader.Application.Models;

public class TokenState
{
    public int Id { get; set; }
    public string AccessToken { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
}
