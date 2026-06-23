using ArasTrader.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArasTrader.Infrastructure.Configurations;

public sealed class WalletConfiguration : IEntityTypeConfiguration<Wallet>
{
    private const string AvailableBalancedColumn = nameof(Wallet.AvailableBalance);
    private const string ReservedBalanceColumn = nameof(Wallet.ReservedBalance);
    public void Configure(EntityTypeBuilder<Wallet> builder)
    {
        builder.ToTable("Wallets", tableBuilder =>
        {
            tableBuilder.HasCheckConstraint(
                "CK_Wallet_AvailableBalance_NonNegative",
                $"\"{AvailableBalancedColumn}\" >= 0");

            tableBuilder.HasCheckConstraint(
                "CK_Wallet_ReservedBalance_NonNegative",
                $"\"{ReservedBalanceColumn}\" >= 0");
        });

        builder.HasKey(w => w.Id);
        builder.Property(w => w.Id)
            .ValueGeneratedOnAdd();

        builder.Property(w => w.AvailableBalance)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(w => w.ReservedBalance)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(w => w.CustomerId)
            .IsRequired();
        builder.HasIndex(w => w.CustomerId)
            .IsUnique();
        builder.HasOne<Customer>(w => w.Customer)
            .WithOne(c => c.Wallet)
            .HasForeignKey<Wallet>(w => w.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.Version)
            .IsConcurrencyToken();
    }
}
