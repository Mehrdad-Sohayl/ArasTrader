using ArasTrader.Application.Common;
using ArasTrader.Application.Models.Wallets;

namespace ArasTrader.Application.Interfaces;

public interface IWalletService
{
    Task<Result<decimal>> Deposit(DepositWalletRequest request, CancellationToken cancellationToken);
}
