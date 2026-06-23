using ArasTrader.Application.Models.Orders;

namespace ArasTrader.Application.Interfaces;

public interface IOrderService
{
    Task<int> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken = default);
}
