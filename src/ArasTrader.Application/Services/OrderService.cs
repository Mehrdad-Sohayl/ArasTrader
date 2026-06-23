using ArasTrader.Application.Common;
using ArasTrader.Application.Exceptions;
using ArasTrader.Application.Interfaces;
using ArasTrader.Application.Interfaces.Repositories;
using ArasTrader.Application.Models.Orders;
using ArasTrader.Domain.Entities;
using ArasTrader.Domain.Enums;
using ApplicationException = ArasTrader.Application.Exceptions.ApplicationException;

namespace ArasTrader.Application.Services;

public class OrderService : IOrderService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public OrderService(
        ICustomerRepository customerRepository,
        IWalletRepository walletRepository,
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _walletRepository = walletRepository;
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<int>> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId);
        if (customer == null)
            return Result<int>.Failure(new ApplicationError(ApplicationErrorCodes.CustomerNotFound, ApplicationErrorCodes.CustomerNotFound));

        var wallet = await _walletRepository.GetByCustomerIdAsync(request.CustomerId);
        if (wallet == null)
            return Result<int>.Failure(new ApplicationError(ApplicationErrorCodes.WalletNotFound, ApplicationErrorCodes.WalletNotFound));

        var order = Order.Create(
            customerId: request.CustomerId,
            symbol: request.Symbol,
            quantity: request.Quantity,
            price: request.Price,
            type: request.Type);

        if (request.Type == OrderType.Buy)
        {
            wallet.ReserveFunds(order.Amount);

        try
        {
            await _orderRepository.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return Result<int>.Failure(new ApplicationError(ApplicationErrorCodes.ConcurrencyException, ex.Message));
        }

        return Result<int>.Success(order.Id);
    }
        {
            await _orderRepository.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();
        }


        return order.Id;
    }
}
