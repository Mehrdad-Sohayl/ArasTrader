namespace ArasTrader.Application.Models.Orders;

public class EditOrderRequest
{
    public int OrderId { get; private set; }
    public int Quantity { get; private set; }
    public decimal Price { get; private set; }

    public EditOrderRequest(
        int orderId,
        int quantity,
        decimal price)
    {
        OrderId = orderId;
        Quantity = quantity;
        Price = price;
    }

}
