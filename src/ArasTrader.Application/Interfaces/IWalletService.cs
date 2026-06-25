using ArasTrader.Application.Common;
using ArasTrader.Application.DTOs;

namespace ArasTrader.Application.Interfaces;

public interface IWalletService
{
    Task<Result<decimal>> Deposit(DepositWalletRequestDto request, CancellationToken cancellationToken);
}
