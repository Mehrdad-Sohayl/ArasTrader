using ArasTrader.Domain.Entities;

namespace ArasTrader.Application.Interfaces.Repositories;

public interface IWalletRepository
{
    Task AddAsync(Wallet wallet);
}
