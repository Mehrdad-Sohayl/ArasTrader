namespace ArasTrader.Api.Contracts.Wallets;

public class DepositWalletRequest
{
    public int CustomerId { get; set; }
    public decimal Amount { get; set; }
}
