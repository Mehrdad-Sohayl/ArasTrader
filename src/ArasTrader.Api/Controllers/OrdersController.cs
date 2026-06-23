using ArasTrader.Api.Contracts.Orders;
using ArasTrader.Application.Interfaces;
using ArasTrader.Application.Models.Orders;
using Microsoft.AspNetCore.Mvc;

namespace ArasTrader.Api.Controllers;

[ApiController]
[Route("/api/orders")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<ActionResult<CreateOrderResponse>> Create(Contracts.Orders.CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var createOrderRequest = new Application.Models.Orders.CreateOrderRequest(
            customerId: request.CustomerId,
            symbol: request.Symbol,
            quantity: request.Quantity,
            price: request.Price,
            type: request.Type);

        var orderId = await _orderService.CreateOrderAsync(createOrderRequest, cancellationToken);

        return Ok(new CreateOrderResponse { OrderId = orderId });
    }
}
