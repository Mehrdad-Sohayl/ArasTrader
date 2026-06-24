using ArasTrader.Application.Common;
using ArasTrader.Application.Models.OrderProcessing;

namespace ArasTrader.Application.Interfaces.OrderProcessing;

public interface IOrderProcessingService
{
    Task<Result<OrderProcessingResult>> ProcessPendingOrdersAsync(CancellationToken cancellationToken);
}
