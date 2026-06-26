namespace ArasTrader.Infrastructure.Options;

public class OrderProcessingOptions
{
    public const string SectionName = "OrderProcessing";
    public int BatchSize { get; init; } = 100;
    public TimeSpan ProcessingTimeout { get; init; } = TimeSpan.FromMinutes(1);
    public string CronExpression { get; init; } = "* * * * *";
}
