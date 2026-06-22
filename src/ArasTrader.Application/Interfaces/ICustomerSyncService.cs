namespace ArasTrader.Application.Interfaces;

public interface ICustomerSyncService
{
    Task SyncAsync(CancellationToken cancellationToken = default);
}
