using ArasTrader.Application.Interfaces.OrderProcessing;
using Microsoft.Extensions.Logging;

namespace ArasTrader.Infrastructure.Jobs;

public class OrderProcessingJob
{
    private readonly IOrderProcessingService _orderProcessingService;
    private readonly ILogger<OrderProcessingJob> _logger;

    public OrderProcessingJob(
        IOrderProcessingService orderProcessingService,
        ILogger<OrderProcessingJob> logger)
    {
        _orderProcessingService = orderProcessingService;
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var result = await _orderProcessingService.ProcessPendingOrdersAsync(cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogError("Order processing failed: {Errors}", string.Join("; ", result.Errors.Select(e => e.Message)));
            return;
        }

        _logger.LogInformation("Order processing finished. Claimed={ClaimedCount} Completed={CompletedCount} Rejected={RejectedCount} Failed={FailedCount}",
            result.Value.ClaimedCount,
            result.Value.CompletedCount,
            result.Value.RejectedCount,
            result.Value.FailedCount);
    }
}