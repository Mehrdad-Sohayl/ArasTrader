using ArasTrader.Domain.Entities;

namespace ArasTrader.Application.Interfaces.Repositories;

public interface IWalletRepository
{
    Task AddAsync(Wallet wallet);
    Task<Wallet?> GetByCustomerIdAsync(int customerId);
    void DepositAsync(Wallet wallet);
}
