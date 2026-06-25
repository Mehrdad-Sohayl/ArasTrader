using ArasTrader.Application.Interfaces;
using ArasTrader.Application.Interfaces.Gateways;
using ArasTrader.Application.Interfaces.OrderProcessing;
using ArasTrader.Application.Interfaces.Repositories;
using ArasTrader.Infrastructure.Auth;
using ArasTrader.Infrastructure.Caching;
using ArasTrader.Infrastructure.ExternalApis.ArasApi;
using ArasTrader.Infrastructure.Gateways;
using ArasTrader.Infrastructure.Jobs;
using ArasTrader.Infrastructure.Options;
using ArasTrader.Infrastructure.Persistence;
using ArasTrader.Infrastructure.Persistence.Contexts;
using ArasTrader.Infrastructure.Persistence.Repositories;
using ArasTrader.Infrastructure.Persistence.Repositories.OrderProcessing;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace ArasTrader.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {

        services.AddDbContext<ArasTraderDbContext>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"));
        });

        #region DataStorage
        services.AddScoped<IAuthTokenRepository, AuthTokenRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IWalletRepository, WalletRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IOrderClaimService, PostgresOrderClaimService>();
        #endregion

        #region Memory
        services.AddMemoryCache();
        services.AddScoped<ITokenStore, MemoryTokenStore>();
        #endregion

        services.AddScoped<ITokenManager, TokenManager>();

        #region ExternalApis
        services.AddRefitClients(configuration);
        services.Configure<ArasApiOptions>(configuration.GetRequiredSection(ArasApiOptions.SectionName));
        services.AddScoped<ICustomerGateway, ArasCustomerGateway>();
        #endregion

        #region OrderProcessingJob
        services.Configure<OrderProcessingOptions>(configuration.GetSection(OrderProcessingOptions.SectionName));
        services.AddScoped<OrderProcessingJob>();
        services.Configure<HangfireOptions>(configuration.GetSection(HangfireOptions.SectionName));

        var hangfireOptions =
            configuration
            .GetSection(HangfireOptions.SectionName)
            .Get<HangfireOptions>() ?? new HangfireOptions();

        services.AddHangfire(config =>
        {
            config.UsePostgreSqlStorage(storage =>
                storage.UseNpgsqlConnection(
                    configuration.GetConnectionString("DefaultConnection")));
        });
        #endregion

        #region Gateways
        services.AddScoped<IOrderGateway, OrderGateway>();
        #endregion

        return services;
    }
}

internal static class RefitDependencyInjection
{
    public static IServiceCollection AddRefitClients(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var arasConfig = configuration
            .GetRequiredSection(ArasApiOptions.SectionName)
            .Get<ArasApiOptions>()!;

        services
            .AddRefitClient<IArasApiClient>()
            .ConfigureHttpClient(client =>
            {
                client.BaseAddress = new Uri(arasConfig.BaseUrl);
            });

        return services;
    }
}