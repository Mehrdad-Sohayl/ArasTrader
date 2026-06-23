using ArasTrader.Application.Models.Wallets;

namespace ArasTrader.Application.Interfaces;

public interface IWalletService
{
    Task Deposit(DepositWalletRequest request, CancellationToken cancellationToken);
}
