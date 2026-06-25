namespace ArasTrader.Application.DTOs;

public class EditOrderRequestDto
{
    public int OrderId { get; private set; }
    public int Quantity { get; private set; }
    public decimal Price { get; private set; }

    public EditOrderRequestDto(
        int orderId,
        int quantity,
        decimal price)
    {
        OrderId = orderId;
        Quantity = quantity;
        Price = price;
    }

}
