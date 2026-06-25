using ArasTrader.Application.Interfaces.Repositories;
using ArasTrader.Domain.Entities;
using ArasTrader.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace ArasTrader.Infrastructure.Persistence.Repositories;

internal class WalletRepository : IWalletRepository
{
    private readonly ArasTraderDbContext _dbContext;

    public WalletRepository(ArasTraderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Wallet wallet, CancellationToken cancellationToken)
    {
        await _dbContext.Wallets.AddAsync(wallet);
    }

    public void DepositAsync(Wallet wallet)
    {
        _dbContext.Wallets.Update(wallet);
    }

    public async Task<Wallet?> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken)
    {
        return await _dbContext.Wallets.SingleOrDefaultAsync(w => w.CustomerId == customerId);
    }
}
