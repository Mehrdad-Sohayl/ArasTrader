using ArasTrader.Application.Common;
using ArasTrader.Application.DTOs;
using ArasTrader.Application.Enums;

namespace ArasTrader.Application.Interfaces.Gateways;

public interface IOrderGateway
{
    Task<Result<int>> CreateAsync(CreateOrderRequestDto request, OrderChannel channel, CancellationToken cancellationToken);
    Task<Result<int>> EditAsync(EditOrderRequestDto request, OrderChannel channel, CancellationToken cancellationToken);
}
