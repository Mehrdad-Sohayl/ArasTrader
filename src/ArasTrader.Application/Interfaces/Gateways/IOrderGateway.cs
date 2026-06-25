using ArasTrader.Application.Common;
using ArasTrader.Application.Enums;
using ArasTrader.Application.Models.Orders;

namespace ArasTrader.Application.Interfaces.Gateways;

public interface IOrderGateway
{
    Task<Result<int>> CreateAsync(CreateOrderRequest request, OrderChannel channel, CancellationToken cancellationToken);
    Task<Result<int>> EditAsync(EditOrderRequest request, OrderChannel channel, CancellationToken cancellationToken);
}
