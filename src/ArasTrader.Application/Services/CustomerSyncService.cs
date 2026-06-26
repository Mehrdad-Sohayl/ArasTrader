using ArasTrader.Application.Common;
using ArasTrader.Application.DTOs;
using ArasTrader.Application.Interfaces;
using ArasTrader.Application.Interfaces.Repositories;
using ArasTrader.Domain.Entities;

namespace ArasTrader.Application.Services;

public class CustomerSyncService : ICustomerSyncService
{
    private readonly ICustomerGateway _customerGateway;
    private readonly ITokenManager _tokenManager;
    private readonly ICustomerRepository _customerRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CustomerSyncService(
        ICustomerGateway customerGateway,
        ITokenManager tokenManager,
        ICustomerRepository customerRepository,
        IWalletRepository walletRepository,
        IUnitOfWork unitOfWork)
    {
        _customerGateway = customerGateway;
        _tokenManager = tokenManager;
        _customerRepository = customerRepository;
        _walletRepository = walletRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<CustomerDto>>> SyncAsync(CancellationToken cancellationToken = default)
    {
        var token = await _tokenManager.GetValidTokenAsync(cancellationToken);
        if (!token.IsSuccess)
            return Result<List<CustomerDto>>.Failure(token.Errors.First());

        var customers = await _customerGateway.GetCustomerAsync(token.Value);
        if (!customers.IsSuccess)
            return Result<List<CustomerDto>>.Failure(customers.Errors.First());

        foreach (var c in customers.Value)
        {
            var exists = await _customerRepository.ExistsByNationalCodeAsync(c.NationalCode, cancellationToken);

            if (exists)
                continue;

            var customer = Customer.Create(
                nationalCode: c.NationalCode,
                firstName: c.FirstName,
                lastName: c.LastName,
                fatherName: c.FatherName,
                birthCertificationNumber: c.BirthCertificationNumber,
                registrationNumber: c.RegisterationNumber,
                birthDate: c.BirthDate,
                branchName: c.BranchName,
                mobileNumber: c.MobileNumber);

            await _customerRepository.AddAsync(customer, cancellationToken);


            var wallet = Wallet.Create(
                customer: customer,
                availableBalance: 0);

            await _walletRepository.AddAsync(wallet, cancellationToken);

            await _unitOfWork.SaveChangesAsync();

        }

        return Result<List<CustomerDto>>.Success(customers.Value);
    }
}
