using ArasTrader.Application.Common;

namespace ArasTrader.Application.Exceptions;

public class ApplicationException : Exception
{
    private readonly ApplicationError _error;

    public ApplicationException(ApplicationError error)
    {
        _error = error;
    }
}

public class ConcurrencyException : ApplicationException
{
    public ConcurrencyException(ApplicationError error) : base(error)
    {
    }
}
