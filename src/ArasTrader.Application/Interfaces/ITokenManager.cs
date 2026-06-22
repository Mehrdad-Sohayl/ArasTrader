namespace ArasTrader.Application.Interfaces;

public interface ITokenManager
{
    Task<string> GetValidTokenAsync();
}
