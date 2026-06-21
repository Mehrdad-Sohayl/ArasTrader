using ArasTrader.Application.Interfaces.Repositories;
using ArasTrader.Domain.Entities;
using ArasTrader.Infrastructure.Persistence.Contexts;

namespace ArasTrader.Infrastructure.Repositories;

internal class WalletRepository : IWalletRepository
{
    private readonly ArasTraderDbContext _dbContext;

    public WalletRepository(ArasTraderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Wallet wallet)
    {
        await _dbContext.Wallets.AddAsync(wallet);
    }
}
