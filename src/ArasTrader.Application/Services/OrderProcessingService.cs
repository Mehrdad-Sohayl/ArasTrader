using ArasTrader.Application.Common;
using ArasTrader.Application.Interfaces;
using ArasTrader.Application.Interfaces.OrderProcessing;
using ArasTrader.Application.Interfaces.Repositories;
using ArasTrader.Application.Models.OrderProcessing;
using ArasTrader.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace ArasTrader.Application.Services;

internal class OrderProcessingService : IOrderProcessingService
{
    private readonly IOrderClaimService _claimService;
    private readonly IOrderRepository _orderRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly IOrderProcessor _orderProcessor;
    private readonly IUnitOfWork _unitOfWork;

    public OrderProcessingService(
        IOrderClaimService claimService,
        IOrderRepository orderRepository,
        IWalletRepository walletRepository,
        IOrderProcessor orderProcessor,
        IUnitOfWork unitOfWork)
    {
        _claimService = claimService;
        _orderRepository = orderRepository;
        _walletRepository = walletRepository;
        _orderProcessor = orderProcessor;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<OrderProcessingResult>> ProcessPendingOrdersAsync(
        CancellationToken cancellationToken)
    {
        int claimedCount = 0;
        int completedCount = 0;
        int rejectedCount = 0;
        int failedCount = 0;

        var orderIds = await _claimService.ClaimPendingOrdersAsync(cancellationToken);
        claimedCount = orderIds.Count();

        foreach (var orderId in orderIds)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);

                if (order is null)
                    continue;

                var wallet = await _walletRepository.GetByCustomerIdAsync(order.CustomerId, cancellationToken);
                if (wallet is null)
                    continue;

                await _orderProcessor.ProcessAsync(order, wallet, cancellationToken);
                if (order.Status == OrderStatus.Completed)
                    completedCount++;
                else if (order.Status == OrderStatus.Rejected)
                    rejectedCount++;

                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _unitOfWork.ClearTracking();
                failedCount++;
            }
        }

        return Result<OrderProcessingResult>.Success(new OrderProcessingResult(claimedCount, completedCount, rejectedCount, failedCount));
    }
}
