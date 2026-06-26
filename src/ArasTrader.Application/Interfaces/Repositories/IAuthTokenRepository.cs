using ArasTrader.Application.Models;

namespace ArasTrader.Application.Interfaces.Repositories;

public interface IAuthTokenRepository
{
    Task<TokenState?> GetAsync(CancellationToken cancellationToken);

    Task UpsertAsync(TokenState token, CancellationToken cancellationToken);
}
