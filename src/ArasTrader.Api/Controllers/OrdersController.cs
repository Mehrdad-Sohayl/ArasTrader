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

        var result = await _orderService.CreateOrderAsync(createOrderRequest, cancellationToken);

        if (result.IsSuccess)
            return Ok(result);

        if (result != null && result.Errors.Any())
            return BadRequest(result.Errors.FirstOrDefault()!.Message);

        return BadRequest();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Edit(int id, Contracts.Orders.EditOrderRequest request, CancellationToken cancellationToken)
    {
        var createOrderRequest = new Application.Models.Orders.EditOrderRequest(
            orderId: id,
            quantity: request.Quantity,
            price: request.Price);

        var result = await _orderService.EditOrderAsync(createOrderRequest, cancellationToken);

        if (result.IsSuccess)
            return Ok(result);

        if (result != null && result.Errors.Any())
            return BadRequest(result.Errors.FirstOrDefault()!.Message);

        return BadRequest();
    }
}
