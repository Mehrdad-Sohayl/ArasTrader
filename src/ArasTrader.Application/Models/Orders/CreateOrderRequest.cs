using ArasTrader.Domain.Enums;

namespace ArasTrader.Application.Models.Orders;

public class CreateOrderRequest
{
    public int CustomerId { get; private set; }
    public string Symbol { get; private set; }
    public int Quantity { get; private set; }
    public decimal Price { get; private set; }
    public OrderType Type { get; private set; }

    public CreateOrderRequest(
        int customerId,
        string symbol,
        int quantity,
        decimal price,
        OrderType type)
    {
        CustomerId = customerId;
        Symbol = symbol;
        Quantity = quantity;
        Price = price;
        Type = type;
    }

}
