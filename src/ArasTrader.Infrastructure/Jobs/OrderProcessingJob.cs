using ArasTrader.Application.Interfaces.OrderProcessing;
using ArasTrader.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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

        _logger.LogInformation($"Order processing finished. " +
            $"Claimed={result.Value.ClaimedCount}" +
            $" Completed={result.Value.CompletedCount}" +
            $" Rejected ={result.Value.RejectedCount}" +
            $" Failed ={result.Value.FailedCount}");
    }
}