using ArasTrader.Domain.Enums;

namespace ArasTrader.Api.Contracts.Orders;

public class CreateOrderRequest
{
    public int CustomerId { get; set; }
    public string Symbol { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public OrderType Type { get; set; }
}
