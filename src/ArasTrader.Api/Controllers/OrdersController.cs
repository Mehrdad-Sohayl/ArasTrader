using ArasTrader.Api.Contracts.Orders;
using ArasTrader.Application.DTOs;
using ArasTrader.Application.Enums;
using ArasTrader.Application.Interfaces.Gateways;
using Microsoft.AspNetCore.Mvc;

namespace ArasTrader.Api.Controllers;

[ApiController]
[Route("/api/orders")]
public class OrdersController : ControllerBase
{
    private readonly IOrderGateway _orderGateway;

    public OrdersController(IOrderGateway orderGateway)
    {
        _orderGateway = orderGateway;
    }

    [HttpPost]
    public async Task<ActionResult<CreateOrderResponse>> Create(CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var createOrderRequest = new CreateOrderRequestDto(
            customerId: request.CustomerId,
            symbol: request.Symbol,
            quantity: request.Quantity,
            price: request.Price,
            type: request.Type);

        var result = await _orderGateway.CreateAsync(createOrderRequest, OrderChannel.API, cancellationToken);

        if (result.IsSuccess)
            return Ok(result);

        if (result != null && result.Errors.Any())
            return BadRequest(result.Errors.FirstOrDefault()!.Message);

        return BadRequest();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Edit(int id, EditOrderRequest request, CancellationToken cancellationToken)
    {
        var createOrderRequest = new EditOrderRequestDto(
            orderId: id,
            quantity: request.Quantity,
            price: request.Price);

        var result = await _orderGateway.EditAsync(createOrderRequest, OrderChannel.API, cancellationToken);

        if (result.IsSuccess)
            return Ok(result);

        if (result != null && result.Errors.Any())
            return BadRequest(result.Errors.FirstOrDefault()!.Message);

        return BadRequest();
    }
}
