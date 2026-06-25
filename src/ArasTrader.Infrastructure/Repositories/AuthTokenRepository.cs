using ArasTrader.Application.Interfaces.Repositories;
using ArasTrader.Application.Models;
using ArasTrader.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace ArasTrader.Infrastructure.Repositories;

internal class AuthTokenRepository : IAuthTokenRepository
{
    private readonly ArasTraderDbContext _dbContext;

    public AuthTokenRepository(ArasTraderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(TokenState tokenState, CancellationToken cancellationToken)
    {
        await _dbContext.TokenStates.AddAsync(tokenState);
    }

    public async Task<TokenState?> GetAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.TokenStates
            .SingleOrDefaultAsync(t => t.ExpiresAtUtc >= DateTime.UtcNow);
    }

    public void Update(TokenState tokenState)
    {
        _dbContext.TokenStates.Update(tokenState);
    }
}
