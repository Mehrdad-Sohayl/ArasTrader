using ArasTrader.Application.Interfaces;
using ArasTrader.Application.Interfaces.Repositories;
using ArasTrader.Application.Models.Wallets;
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

    public async Task Deposit(DepositWalletRequest request, CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId);
        if (customer == null)
            throw new ApplicationException();

        var wallet = await _walletRepository.GetByCustomerIdAsync(request.CustomerId);
        if (wallet == null)
            throw new ApplicationException();

        wallet.Credit(request.Amount);

        _walletRepository.DepositAsync(wallet);
        await _unitOfWork.SaveChangesAsync();
    }
}
