using ArasTrader.Application.Gateways;
using ArasTrader.Application.Interfaces;
using ArasTrader.Application.Interfaces.Gateways;
using ArasTrader.Application.Interfaces.OrderProcessing;
using ArasTrader.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ArasTrader.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        #region Services
        services.AddScoped<ICustomerSyncService, CustomerSyncService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IWalletService, WalletService>();
        services.AddScoped<IOrderProcessor, OrderProcessor>();
        services.AddScoped<IOrderProcessingService, OrderProcessingService>();
        #endregion

        #region Gateways
        services.AddScoped<IOrderGateway, OrderGateway>();
        #endregion

        return services;
    }
}
