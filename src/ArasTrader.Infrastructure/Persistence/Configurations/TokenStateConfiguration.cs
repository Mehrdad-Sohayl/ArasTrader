using ArasTrader.Application.Models;
using ArasTrader.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArasTrader.Infrastructure.Persistence.Configurations;

public class TokenStateConfiguration : IEntityTypeConfiguration<TokenState>
{
    public void Configure(EntityTypeBuilder<TokenState> builder)
    {
        builder.ToTable("AuthTokens");

        builder.HasKey(t => t.Id);
        builder.Property(o => o.Id)
            .ValueGeneratedOnAdd();

        builder.Property(t => t.AccessToken)
            .IsRequired();

        builder.Property(t => t.ExpiresAtUtc)
            .IsRequired();

        builder.Property(t => t.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAdd();

        builder.Property(t => t.ModifiedAt)
            .IsRequired()
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAddOrUpdate();
    }
}
