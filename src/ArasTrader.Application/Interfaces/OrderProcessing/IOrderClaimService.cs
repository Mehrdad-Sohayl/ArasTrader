namespace ArasTrader.Application.Interfaces.OrderProcessing;

public interface IOrderClaimService
{
    Task<IReadOnlyList<int>> ClaimPendingOrdersAsync(CancellationToken cancellationToken);
}
