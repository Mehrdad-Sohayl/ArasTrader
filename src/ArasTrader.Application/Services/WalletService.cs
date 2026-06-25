using ArasTrader.Application.Common;
using ArasTrader.Application.DTOs;
using ArasTrader.Application.Interfaces;
using ArasTrader.Application.Interfaces.Repositories;
using ApplicationException = ArasTrader.Application.Exceptions.ApplicationException;

namespace ArasTrader.Application.Services;

internal class WalletService : IWalletService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly IUnitOfWork _unitOfWork;

    public WalletService(
        ICustomerRepository customerRepository,
        IWalletRepository walletRepository,
        IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _walletRepository = walletRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<decimal>> Deposit(DepositWalletRequestDto request, CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
        if (customer == null)
            return Result<decimal>.Failure(new ApplicationError(ApplicationErrorCodes.CustomerNotFound, ApplicationErrorCodes.CustomerNotFound));

        var wallet = await _walletRepository.GetByCustomerIdAsync(request.CustomerId, cancellationToken);
        if (wallet == null)
            return Result<decimal>.Failure(new ApplicationError(ApplicationErrorCodes.WalletNotFound, ApplicationErrorCodes.WalletNotFound));

        wallet.Credit(request.Amount);

        _walletRepository.DepositAsync(wallet);
        await _unitOfWork.SaveChangesAsync();

        return Result<decimal>.Success(wallet.AvailableBalance);
    }
}
