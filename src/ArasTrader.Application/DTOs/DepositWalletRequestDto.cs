namespace ArasTrader.Application.DTOs;

public class DepositWalletRequestDto
{
    public int CustomerId { get; private set; }
    public decimal Amount { get; private set; }

    public DepositWalletRequestDto(int customerId, decimal amount)
    {
        CustomerId = customerId;
        Amount = amount;
    }

}
