using ArasTrader.Application.Common;
using ArasTrader.Application.DTOs;

namespace ArasTrader.Application.Interfaces;

public interface IOrderService
{
    Task<Result<int>> CreateOrderAsync(CreateOrderRequestDto request, CancellationToken cancellationToken = default);
    Task<Result<int>> EditOrderAsync(EditOrderRequestDto request, CancellationToken cancellationToken = default);
}
