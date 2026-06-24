using ArasTrader.Api.Middlewares;
using ArasTrader.Application;
using ArasTrader.Infrastructure;
using Hangfire;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.MapControllers();

app.UseHangfireDashboard("/hangfire");

app.Run();
