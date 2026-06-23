using ArasTrader.Domain.Common;
using ArasTrader.Domain.Exceptions;

namespace ArasTrader.Domain.Entities;

public class Wallet
{
    public int Id { get; private set; }
    public int CustomerId { get; private set; }
    public Customer Customer { get; private set; }
    public decimal AvailableBalance { get; private set; }
    public decimal ReservedBalance { get; private set; }
    public uint Version { get; set; }

    private Wallet()
    {

    }

    internal Wallet(
        Customer customer,
        int customerId,
        decimal availableBalance,
        decimal reservedBalance)
    {
        Customer = customer;
        CustomerId = customer.Id;
        AvailableBalance = availableBalance;
        ReservedBalance = reservedBalance;
    }

    public static Wallet Create(
        Customer customer,
        decimal availableBalance)
    {
        if (customer == null)
            throw new DomainException(new DomainError(DomainErrorCodes.CustomerNotFound,DomainErrorCodes.CustomerNotFound));

        if (availableBalance < 0)
            throw new DomainException(new DomainError(DomainErrorCodes.InsufficientBalance,DomainErrorCodes.InsufficientBalance));

        return new Wallet(customer, customer.Id, availableBalance, 0);
    }

    public void ReserveFunds(decimal amount)
    {
        if (amount <= 0)
            throw new DomainException(new DomainError(DomainErrorCodes.InvalidAmount,DomainErrorCodes.InvalidAmount));

        if (AvailableBalance < amount)
            throw new DomainException(new DomainError(DomainErrorCodes.InsufficientBalance,DomainErrorCodes.InsufficientBalance));

        AvailableBalance -= amount;
        ReservedBalance += amount;
    }

    public void ReleaseFunds(decimal amount)
    {
        if (amount <= 0)
            throw new DomainException(new DomainError(DomainErrorCodes.InvalidAmount,DomainErrorCodes.InvalidAmount));

        if (ReservedBalance < amount)
            throw new DomainException(new DomainError(DomainErrorCodes.InsufficientBalance,DomainErrorCodes.InsufficientBalance));

        ReservedBalance -= amount;
        AvailableBalance += amount;
    }

    public void FinalizeReservation(decimal amount)
    {
        if (amount <= 0)
            throw new DomainException(new DomainError(DomainErrorCodes.InvalidAmount,DomainErrorCodes.InvalidAmount));

        if (ReservedBalance < amount)
            throw new DomainException(new DomainError(DomainErrorCodes.InsufficientBalance,DomainErrorCodes.InsufficientBalance));

        ReservedBalance -= amount;
    }

    public void Credit(decimal amount)
    {
        if (amount <= 0)
            throw new DomainException(new DomainError(DomainErrorCodes.InvalidAmount,DomainErrorCodes.InvalidAmount));

        AvailableBalance += amount;
    }
}
