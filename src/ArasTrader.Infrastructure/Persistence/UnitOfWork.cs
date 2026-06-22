using ArasTrader.Application.Interfaces;
using ArasTrader.Infrastructure.Persistence.Contexts;

namespace ArasTrader.Infrastructure.Persistence;

internal class UnitOfWork : IUnitOfWork
{
    private readonly ArasTraderDbContext _dbContext;

    public UnitOfWork(ArasTraderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
