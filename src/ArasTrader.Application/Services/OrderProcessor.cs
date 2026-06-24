using ArasTrader.Application.Interfaces.OrderProcessing;
using ArasTrader.Domain.Entities;
using ArasTrader.Domain.Enums;

namespace ArasTrader.Application.Services;

internal class OrderProcessor : IOrderProcessor
{

    private readonly Random _random = new();

    public Task ProcessAsync(Order order, Wallet wallet, CancellationToken cancellationToken)
    {
        var randomValue = _random.Next(100);

        if (randomValue < 70)
        {
            if (order.Type == OrderType.Buy)
                wallet.FinalizeReservation(order.Amount);

            if (order.Type == OrderType.Sell)
                wallet.Credit(order.Amount);

            order.MarkCompleted();
            order.ClearProcessingTimeout();
        }
        else
        {
            wallet.ReleaseFunds(order.Amount);
            order.MarkRejected();
            order.ClearProcessingTimeout();
        }

        return Task.CompletedTask;
    }
}