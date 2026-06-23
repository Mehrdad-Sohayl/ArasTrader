using ArasTrader.Application.Common;
using ArasTrader.Application.DTOs;

namespace ArasTrader.Application.Interfaces;

public interface ICustomerGateway
{
    Task<Result<List<CustomerDto>>> GetCustomerAsync(string accessToken);
}
