using ArasTrader.Domain.Entities;

namespace ArasTrader.Application.Interfaces.OrderProcessing;

public interface IOrderProcessor
{
    Task ProcessAsync(Order order,Wallet wallet, CancellationToken cancellationToken);
}
