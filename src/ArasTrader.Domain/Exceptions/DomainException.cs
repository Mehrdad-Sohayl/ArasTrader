using ArasTrader.Domain.Common;

namespace ArasTrader.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainError Error { get; }

    public DomainException(DomainError error)
        : base(error.Message)
    {
        Error = error;
    }
}
