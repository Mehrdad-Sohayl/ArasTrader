namespace ArasTrader.Application.Models.Wallets;

public class DepositWalletRequest
{
    public int CustomerId { get; private set; }
    public decimal Amount { get; private set; }

    public DepositWalletRequest(int customerId, decimal amount)
    {
        CustomerId = customerId;
        Amount = amount;
    }

}
