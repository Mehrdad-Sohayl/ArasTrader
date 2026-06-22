using ArasTrader.Application.DTOs;
using ArasTrader.Application.Interfaces;

namespace ArasTrader.Infrastructure.ExternalApis.ArasApi;

internal class ArasCustomerGateway : ICustomerGateway
{
    private readonly IArasApiClient _arasApiClient;

    public ArasCustomerGateway(IArasApiClient arasApiClient)
    {
        _arasApiClient = arasApiClient;
    }

    public async Task<List<CustomerDto>> GetCustomerAsync(string accessToken)
    {
        var customers = await _arasApiClient.GetCustomersAsync($"Bearer {accessToken}");

        return customers.Select(customers => new CustomerDto
        {
            NationalCode = customers.NationalCode,
            FirstName = customers.FirstName,
            LastName = customers.LastName,
            FatherName = customers.FatherName,
            BirthCertificationNumber = customers.BirthCertificationNumber,
            RegisterationNumber = customers.RegisterationNumber,
            BirthDate = DateOnly.Parse(customers.BirthDate),
            BranchName = customers.BranchName,
            MobileNumber = customers.MobileNumber
        }).ToList();
    }


}
