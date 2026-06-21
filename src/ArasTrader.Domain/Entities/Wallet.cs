using ArasTrader.Domain.Exceptions;

namespace ArasTrader.Domain.Entities;

public class Wallet
{
    public int Id { get; private set; }
    public int CustomerId { get; private set; }
    public decimal AvailableBalance { get; private set; }
    public decimal ReservedBalance { get; private set; }

    private Wallet()
    {

    }

    internal Wallet(
        int customerId,
        decimal availableBalance,
        decimal reservedBalance)
    {
        CustomerId = customerId;
        AvailableBalance = availableBalance;
        ReservedBalance = reservedBalance;
    }

    public static Wallet Create(
        int customerId,
        decimal availableBalance,
        decimal reservedBalance)
    {
        if (customerId <= 0)
            throw new DomainException();

        if (availableBalance < 0)
            throw new DomainException();

        if (reservedBalance < 0)
            throw new DomainException();

        return new Wallet(customerId, availableBalance, reservedBalance);
    }

    public void ReserveFunds(decimal amount)
    {
        if (amount <= 0)
            throw new DomainException();

        if (AvailableBalance < amount)
            throw new DomainException();

        AvailableBalance -= amount;
        ReservedBalance += amount;
    }

    public void ReleaseFunds(decimal amount)
    {
        if (amount <= 0)
            throw new DomainException();

        if (ReservedBalance < amount)
            throw new DomainException();

        ReservedBalance -= amount;
        AvailableBalance += amount;
    }

    public void FinalizeReservation(decimal amount)
    {
        if (amount <= 0)
            throw new DomainException();

        if (ReservedBalance < amount)
            throw new DomainException();

        ReservedBalance -= amount;
    }

    public void Credit(decimal amount)
    {
        if (amount <= 0)
            throw new DomainException();

        AvailableBalance += amount;
    }
}
