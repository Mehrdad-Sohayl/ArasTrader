using ArasTrader.Application.Interfaces.Repositories;
using ArasTrader.Domain.Entities;
using ArasTrader.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace ArasTrader.Infrastructure.Persistence.Repositories;

internal class OrderRepository : IOrderRepository
{
    private readonly ArasTraderDbContext _dbContext;

    public OrderRepository(ArasTraderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Order order, CancellationToken cancellationToken)
    {
        await _dbContext.Orders.AddAsync(order);
    }

    public async Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _dbContext.Orders.SingleOrDefaultAsync(o => o.Id == id);
    }
}
