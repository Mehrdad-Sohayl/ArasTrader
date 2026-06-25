using ArasTrader.Application.Common;

namespace ArasTrader.Application.Exceptions;

public class ApplicationException : Exception
{
    public ApplicationError Error { get; }

    public ApplicationException(ApplicationError error)
        : base(error.Message)
    {
        Error = error;
    }
}

public class ConcurrencyException : ApplicationException
{
    public ConcurrencyException(ApplicationError error) : base(error)
    {
    }
}
