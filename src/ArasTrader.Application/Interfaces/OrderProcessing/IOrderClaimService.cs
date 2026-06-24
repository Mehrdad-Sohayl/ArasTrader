namespace ArasTrader.Application.Interfaces.OrderProcessing;

public interface IOrderClaimService
{
    Task<IReadOnlyList<int>> ClaimPendingOrdersAsync(int batchSize, CancellationToken cancellationToken);
}
