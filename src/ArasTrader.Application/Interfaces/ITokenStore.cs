using ArasTrader.Application.Models;

namespace ArasTrader.Application.Interfaces;

public interface ITokenStore
{
    void Save(TokenState token);
    TokenState? Get();
    void Clear();
}

