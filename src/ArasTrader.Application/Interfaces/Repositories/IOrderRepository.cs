using ArasTrader.Domain.Entities;

namespace ArasTrader.Application.Interfaces.Repositories;

public interface IOrderRepository
{
    Task AddAsync(Order order, CancellationToken cancellationToken);
    Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken);
}
