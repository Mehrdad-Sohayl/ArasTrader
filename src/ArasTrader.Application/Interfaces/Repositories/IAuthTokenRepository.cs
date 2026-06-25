using ArasTrader.Application.Models;

namespace ArasTrader.Application.Interfaces.Repositories;

public interface IAuthTokenRepository
{
    Task<TokenState?> GetAsync(CancellationToken cancellationToken);
    Task AddAsync(TokenState tokenState, CancellationToken cancellationToken);
    void Update(TokenState tokenState);
}
