using ArasTrader.Application.Interfaces;
using ArasTrader.Application.Interfaces.Repositories;
using ArasTrader.Infrastructure.Auth;
using ArasTrader.Infrastructure.ExternalApis.ArasApi;
using ArasTrader.Infrastructure.Options;
using ArasTrader.Infrastructure.Persistence.Contexts;
using ArasTrader.Infrastructure.Repositories;
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


        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IWalletRepository, WalletRepository>();

        services.AddMemoryCache();

        services.AddSingleton<ITokenStore, ITokenStore>();
        services.AddSingleton<ITokenManager, TokenManager>();

        services.AddRefitClients(configuration);

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