using ArasTrader.Domain.Common;

namespace ArasTrader.Domain.Exceptions;

public class DomainException : Exception
{
    private readonly DomainError _error;

    public DomainException(DomainError error)
    {
        _error = error;
    }
}
