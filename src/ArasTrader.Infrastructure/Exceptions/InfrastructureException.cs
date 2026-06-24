namespace ArasTrader.Infrastructure.Exceptions;

public class InfrastructureException : Exception
{
    private readonly InfrastructureError _error;

    public InfrastructureException(InfrastructureError error)
    {
        _error = error;
    }
}

public record InfrastructureError(string code, string message);
