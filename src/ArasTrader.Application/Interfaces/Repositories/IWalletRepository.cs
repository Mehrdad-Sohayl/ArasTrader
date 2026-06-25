using ArasTrader.Domain.Entities;

namespace ArasTrader.Application.Interfaces.Repositories;

public interface IWalletRepository
{
    Task AddAsync(Wallet wallet, CancellationToken cancellationToken);
    Task<Wallet?> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken);
    void DepositAsync(Wallet wallet);
}
