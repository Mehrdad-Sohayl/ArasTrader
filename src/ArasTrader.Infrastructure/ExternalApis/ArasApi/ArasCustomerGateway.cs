using ArasTrader.Application.Common;
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

    public async Task<Result<List<CustomerDto>>> GetCustomerAsync(string accessToken)
    {
        var customers = await _arasApiClient.GetCustomersAsync($"Bearer {accessToken}");
        if (customers == null || !customers.Any())
            return Result<List<CustomerDto>>.Failure(new ApplicationError(ApplicationErrorCodes.CannotRetriveCustomers, ApplicationErrorCodes.CannotRetriveCustomers));

        var result = customers.Select(customers => new CustomerDto
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

        return Result<List<CustomerDto>>.Success(result);
    }


}
