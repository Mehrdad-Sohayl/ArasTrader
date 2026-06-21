using ArasTrader.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArasTrader.Infrastructure.Configurations;

public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id)
            .ValueGeneratedOnAdd();

        builder.Property(o => o.CustomerId)
            .IsRequired();
        builder.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(o => o.CustomerId);

        builder.Property(o => o.Symbol)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(o => o.Quantity)
            .IsRequired();

        builder.Property(o => o.Price)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(o => o.Type)
            .IsRequired();

        builder.Property(o => o.Status)
            .IsRequired();
        builder.HasIndex(o => o.Status);

        builder.Property(o => o.CreatedAt)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(o => o.ModifiedAt);

        builder.Ignore(o => o.Amount);
    }
}
