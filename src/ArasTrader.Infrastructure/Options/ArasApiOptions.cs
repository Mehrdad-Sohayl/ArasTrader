namespace ArasTrader.Infrastructure.Options;

public class ArasApiOptions
{
    public const string SectionName = "ArasApi";

    public string BaseUrl { get; init; } = null!;
    public string Username { get; init; } = null!;
    public string Password { get; init; } = null!;
}
