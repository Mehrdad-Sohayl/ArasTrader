using ArasTrader.Application.Common;

namespace ArasTrader.Application.Interfaces;

public interface ITokenManager
{
    Task<Result<string>> GetValidTokenAsync();
}
