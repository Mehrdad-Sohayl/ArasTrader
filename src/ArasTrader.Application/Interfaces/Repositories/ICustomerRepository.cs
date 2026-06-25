using ArasTrader.Domain.Entities;

namespace ArasTrader.Application.Interfaces.Repositories;

public interface ICustomerRepository
{
    Task AddAsync(Customer customer, CancellationToken cancellationToken);
    Task<bool> ExistsByNationalCodeAsync(string nationalCode, CancellationToken cancellationToken);
    Task<bool> ExistsByIdAsync(int customerId, CancellationToken cancellationToken);
    Task<Customer?> GetByIdAsync(int customerId, CancellationToken cancellationToken);
}
