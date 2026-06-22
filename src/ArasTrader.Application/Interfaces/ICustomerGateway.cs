using ArasTrader.Application.DTOs;

namespace ArasTrader.Application.Interfaces;

public interface ICustomerGateway
{
    Task<List<CustomerDto>> GetCustomerAsync(string accessToken);
}
