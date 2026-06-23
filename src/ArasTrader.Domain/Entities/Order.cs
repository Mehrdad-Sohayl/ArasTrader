using ArasTrader.Domain.Enums;
using ArasTrader.Domain.Exceptions;

namespace ArasTrader.Domain.Entities;

public class Order
{
    public int Id { get; private set; }
    public int CustomerId { get; private set; }
    public string Symbol { get; private set; }
    public int Quantity { get; private set; }
    public decimal Price { get; private set; }
    public OrderType Type { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime ModifiedAt { get; private set; }
    public uint Version { get; set; }
    public decimal Amount
    {
        get { return Quantity * Price; }
    }

    private Order()
    {
        
    }

    internal Order(
        int customerId,
        string symbol,
        int quantity,
        decimal price,
        OrderType type,
        OrderStatus status,
        DateTime createdAt)
    {
        CustomerId = customerId;
        Symbol = symbol;
        Quantity = quantity;
        Price = price;
        Type = type;
        Status = status;
        CreatedAt = createdAt;
    }

    public static Order Create(
        int customerId,
        string symbol,
        int quantity,
        decimal price,
        OrderType type)
    {
        if (customerId <= 0)
            throw new DomainException();

        if (string.IsNullOrWhiteSpace(symbol))
            throw new DomainException();

        if (quantity <= 0)
            throw new DomainException();

        if (price <= 0)
            throw new DomainException();

        return new Order(
            customerId,
            symbol,
            quantity,
            price,
            type,
            OrderStatus.Pending,
            DateTime.UtcNow);
    }

    public void Edit(int quantity, decimal price)
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException();

        if (quantity <= 0)
            throw new DomainException();

        if (price <= 0)
            throw new DomainException();

        Quantity = quantity;
        Price = price;
        ModifiedAt = DateTime.UtcNow;
    }

    public void MarkInProgress()
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException();

        Status = OrderStatus.InProgress;
        ModifiedAt = DateTime.UtcNow;
    }

    public void MarkCompleted()
    {
        if (Status != OrderStatus.InProgress)
            throw new DomainException();

        Status = OrderStatus.Completed;
        ModifiedAt = DateTime.UtcNow;
    }

    public void MarkRejected()
    {
        if (Status != OrderStatus.InProgress)
            throw new DomainException();

        Status = OrderStatus.Rejected;
        ModifiedAt = DateTime.UtcNow;
    }
}
