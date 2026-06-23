using ArasTrader.Application.Interfaces;
using ArasTrader.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

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
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new Application.Exceptions.ApplicationException();
        }
    }
}
