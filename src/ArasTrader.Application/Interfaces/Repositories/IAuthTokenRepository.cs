using ArasTrader.Application.Models;

namespace ArasTrader.Application.Interfaces.Repositories;

public interface IAuthTokenRepository
{
    Task<TokenState?> GetAsync();
    Task AddAsync(TokenState tokenState);
    void Update(TokenState tokenState);
}
