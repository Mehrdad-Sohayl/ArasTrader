using ArasTrader.Application.Common;
using ArasTrader.Application.Models.Orders;

namespace ArasTrader.Application.Interfaces;

public interface IOrderService
{
    Task<Result<int>> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken = default);
    Task<Result<int>> EditOrderAsync(EditOrderRequest request, CancellationToken cancellationToken = default);
}
