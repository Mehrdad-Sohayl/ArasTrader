namespace ArasTrader.Application.Models.OrderProcessing;

public sealed record OrderProcessingResult(
    int ClaimedCount,
    int CompletedCount,
    int RejectedCount,
    int FailedCount);