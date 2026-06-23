using ArasTrader.Application.Common;
using ArasTrader.Application.DTOs;

namespace ArasTrader.Application.Interfaces;

public interface ICustomerSyncService
{
    Task<Result<List<CustomerDto>>> SyncAsync(CancellationToken cancellationToken = default);
}
