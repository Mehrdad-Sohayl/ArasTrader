using ArasTrader.Application.Common;
using ArasTrader.Application.DTOs;
using ArasTrader.Application.Enums;
using ArasTrader.Application.Interfaces;
using ArasTrader.Application.Interfaces.Gateways;
using Microsoft.Extensions.Logging;

namespace ArasTrader.Infrastructure.Gateways;

internal class OrderGateway : IOrderGateway
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrderGateway> _logger;

    public OrderGateway(
        IOrderService orderService,
        ILogger<OrderGateway> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    public async Task<Result<int>> CreateAsync(CreateOrderRequestDto request, OrderChannel channel, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Create order received from channel: {channel}");
        return await _orderService.CreateOrderAsync(request, cancellationToken);

    }

    public async Task<Result<int>> EditAsync(EditOrderRequestDto request, OrderChannel channel, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Edit order request received from channel: {channel}");
        return await _orderService.EditOrderAsync(request, cancellationToken);

    }
}
