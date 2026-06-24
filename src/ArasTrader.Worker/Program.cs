using ArasTrader.Application;
using ArasTrader.Infrastructure;
using ArasTrader.Infrastructure.Jobs;
using ArasTrader.Infrastructure.Options;
using ArasTrader.Worker;
using Hangfire;
using Microsoft.Extensions.Options;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHangfireServer();

var host = builder.Build();

using var scope = host.Services.CreateScope();

var options =
    scope.ServiceProvider
        .GetRequiredService<IOptions<OrderProcessingOptions>>()
        .Value;

var recurringJobManager =
    scope.ServiceProvider
        .GetRequiredService<IRecurringJobManager>();

recurringJobManager.AddOrUpdate<OrderProcessingJob>(
    recurringJobId: "order-processing",
    methodCall: job => job.ExecuteAsync(CancellationToken.None),
    cronExpression: options.CronExpression);

await host.RunAsync();
