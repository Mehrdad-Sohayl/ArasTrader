using ArasTrader.Application.Interfaces.Repositories;
using ArasTrader.Domain.Entities;
using ArasTrader.Infrastructure.Persistence.Contexts;

namespace ArasTrader.Infrastructure.Repositories;

internal class CustomerRepository : ICustomerRepository
{
    private readonly ArasTraderDbContext _dbContext;

    public CustomerRepository(ArasTraderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Customer customer)
    {
        await _dbContext.Customers.AddAsync(customer);
    }
}
