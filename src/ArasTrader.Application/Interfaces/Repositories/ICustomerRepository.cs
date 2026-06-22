using ArasTrader.Domain.Entities;

namespace ArasTrader.Application.Interfaces.Repositories;

public interface ICustomerRepository
{
    Task AddAsync(Customer customer);
    Task<bool> ExistsByNationalCodeAsync(string nationalCode);
}
