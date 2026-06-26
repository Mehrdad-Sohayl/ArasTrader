using ArasTrader.Application.Interfaces.Repositories;
using ArasTrader.Application.Models;
using ArasTrader.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace ArasTrader.Infrastructure.Persistence.Repositories;

internal class AuthTokenRepository : IAuthTokenRepository
{
    private readonly ArasTraderDbContext _dbContext;

    public AuthTokenRepository(ArasTraderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TokenState?> GetAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.TokenStates
            .SingleOrDefaultAsync(t => t.ExpiresAtUtc >= DateTime.UtcNow);
    }

    public async Task UpsertAsync(TokenState token, CancellationToken cancellationToken)
    {
        var existing = await _dbContext.TokenStates.SingleOrDefaultAsync(cancellationToken);
        if (existing == null)
            await _dbContext.TokenStates.AddAsync(token, cancellationToken);
        else
        {
            existing.AccessToken = token.AccessToken;
            existing.ExpiresAtUtc = token.ExpiresAtUtc;
        }
    }
}
