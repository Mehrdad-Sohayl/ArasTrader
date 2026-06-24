namespace ArasTrader.Infrastructure.Options;

internal class HangfireOptions
{
    public const string SectionName = "Hangfire";
    public string SchemaName { get; init; } = "hangfire";
}