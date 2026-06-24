using ArasTrader.Application.Interfaces.OrderProcessing;
using ArasTrader.Domain.Entities;

namespace ArasTrader.Application.Services;

internal class OrderProcessor : IOrderProcessor
{

    private readonly Random _random = new();

    public Task ProcessAsync(Order order, CancellationToken cancellationToken)
    {
        var randomValue = _random.Next(100);

        if (randomValue < 70)
        {
            order.MarkCompleted();
            order.ClearProcessingTimeout();
        }
        else
        {
            order.MarkRejected();
            order.ClearProcessingTimeout();
        }

        return Task.CompletedTask;
    }
}