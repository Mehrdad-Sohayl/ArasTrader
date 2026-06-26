using ArasTrader.Application.Common;
using ArasTrader.Application.DTOs;
using ArasTrader.Application.Exceptions;
using ArasTrader.Application.Interfaces;
using ArasTrader.Application.Interfaces.Repositories;
using ArasTrader.Domain.Entities;
using ArasTrader.Domain.Enums;
using ApplicationException = ArasTrader.Application.Exceptions.ApplicationException;

namespace ArasTrader.Application.Services;

public class OrderService : IOrderService
{
    private const int MaxRetries = 20;

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

    public async Task<Result<int>> CreateOrderAsync(CreateOrderRequestDto request, CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
        if (customer == null)
            return Result<int>.Failure(new ApplicationError(ApplicationErrorCodes.CustomerNotFound, ApplicationErrorCodes.CustomerNotFound));

        var wallet = await _walletRepository.GetByCustomerIdAsync(request.CustomerId, cancellationToken);
        if (wallet == null)
            return Result<int>.Failure(new ApplicationError(ApplicationErrorCodes.WalletNotFound, ApplicationErrorCodes.WalletNotFound));

        var order = Order.Create(
            customerId: request.CustomerId,
            symbol: request.Symbol,
            quantity: request.Quantity,
            price: request.Price,
            type: request.Type);

        if (request.Type == OrderType.Buy)
            wallet.ReserveFunds(order.Amount);

        try
        {
            await _orderRepository.AddAsync(order, cancellationToken);
            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return Result<int>.Failure(new ApplicationError(ApplicationErrorCodes.ConcurrencyException, ex.Message));
        }

        return Result<int>.Success(order.Id);
    }

    public async Task<Result<int>> EditOrderAsync(EditOrderRequestDto request, CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt < MaxRetries; attempt++)
        {
            try
            {
                var result = await InnerEditOrderAsync(request, cancellationToken);
                return Result<int>.Success(result);
            }
            catch (ConcurrencyException)
            {
                _unitOfWork.ClearTracking();

                if (attempt < MaxRetries - 1)
                {
                    var delayMs = Math.Pow(2, attempt) * 10;
                    await Task.Delay((int)Math.Min(delayMs, 500), cancellationToken);
                }
            }
        }

        return Result<int>.Failure(new ApplicationError(ApplicationErrorCodes.CannotEditOrder, ApplicationErrorCodes.CannotEditOrder));
    }

    private async Task<int> InnerEditOrderAsync(EditOrderRequestDto request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order == null)
            throw new ApplicationException(new ApplicationError(ApplicationErrorCodes.OrderNotFound, ApplicationErrorCodes.OrderNotFound));

        var wallet = await _walletRepository.GetByCustomerIdAsync(order.CustomerId, cancellationToken);
        if (wallet == null)
            throw new ApplicationException(new ApplicationError(ApplicationErrorCodes.WalletNotFound, ApplicationErrorCodes.WalletNotFound));

        var newAmount = request.Price * request.Quantity;
        var delta = newAmount - order.Amount;

        if (delta > 0)
            wallet.ReserveFunds(delta);

        if (delta < 0)
            wallet.ReleaseFunds(Math.Abs(delta));

        order.Edit(request.Quantity, request.Price);
        await _unitOfWork.SaveChangesAsync();
        return order.Id;
    }
}
