using ArasTrader.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArasTrader.Infrastructure.Persistence.Configurations;

public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .ValueGeneratedOnAdd();

        builder.Property(c => c.NationalCode)
            .IsRequired()
            .HasMaxLength(10);
        builder.HasIndex(c => c.NationalCode)
            .IsUnique();

        builder.Property(c => c.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.FatherName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.BirthCertificationNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.RegistrationNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.BirthDate)
            .HasConversion(
            v => v.ToDateTime(TimeOnly.MinValue),
            v => DateOnly.FromDateTime(v))
            .HasColumnType("date");

        builder.Property(c => c.BranchName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.MobileNumber)
            .IsRequired()
            .HasMaxLength(11);

        builder.Property(c => c.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAdd();





    }
}
