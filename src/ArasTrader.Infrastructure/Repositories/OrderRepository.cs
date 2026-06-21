using ArasTrader.Application.Interfaces.Repositories;
using ArasTrader.Domain.Entities;
using ArasTrader.Infrastructure.Persistence.Contexts;

namespace ArasTrader.Infrastructure.Repositories;

internal class OrderRepository : IOrderRepository
{
    private readonly ArasTraderDbContext _dbContext;

    public OrderRepository(ArasTraderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Order order)
    {
        await _dbContext.Orders.AddAsync(order);
    }
}
