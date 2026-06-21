using ArasTrader.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ArasTrader.Infrastructure.Persistence.Contexts;

internal class ArasTraderDbContext : DbContext
{
    public ArasTraderDbContext(DbContextOptions options) : base(options)
    {
    }

    protected ArasTraderDbContext()
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Wallet> Wallets => Set<Wallet>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ArasTraderDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
