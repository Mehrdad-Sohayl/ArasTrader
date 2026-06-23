using ArasTrader.Application.Interfaces.Repositories;
using ArasTrader.Domain.Entities;
using ArasTrader.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

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

    public async Task<bool> ExistsByIdAsync(int customerId)
    {
        return await _dbContext.Customers.AnyAsync(c => c.Id == customerId);
    }

    public async Task<bool> ExistsByNationalCodeAsync(string nationalCode)
    {
        return await _dbContext.Customers.AnyAsync(c => c.NationalCode == nationalCode);
    }

    public async Task<Customer?> GetByIdAsync(int customerId)
    {
        return await _dbContext.Customers.SingleOrDefaultAsync(c => c.Id == customerId);
    }
}
