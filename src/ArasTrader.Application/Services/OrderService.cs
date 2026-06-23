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

    public async Task<int> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId);
        if (customer == null)
            throw new ApplicationException();

        var wallet = await _walletRepository.GetByCustomerIdAsync(request.CustomerId);
        if (wallet == null)
            throw new ApplicationException();

        var order = Order.Create(
            customerId: request.CustomerId,
            symbol: request.Symbol,
            quantity: request.Quantity,
            price: request.Price,
            type: request.Type);

        if (request.Type == OrderType.Buy)
        {
            wallet.ReserveFunds(order.Amount);
            await _orderRepository.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();
        }
        else
        {
            await _orderRepository.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();
        }


        return order.Id;
    }
}
