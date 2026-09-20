using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Triply.Domain.Entities;
using Triply.Domain.Enums.Payments;

namespace Triply.Infrastructure.Configurations;

/// <summary>Entity Framework configuration of the payment entity.</summary>
public class PaymentConfigurations : IEntityTypeConfiguration<Payment>
{
    /// <summary>
    /// Apply Payment Configurations
    /// </summary>
    /// <param name="builder"></param>
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments", x =>
        {
            var statuses = string.Join(", ", Enum.GetNames<PaymentStatus>().Select(status => $"'{status}'"));
            x.HasCheckConstraint("CK_Payments_Status",
                $"[{nameof(Payment.Status)}] IN ({statuses})");

            x.HasCheckConstraint("CK_Payments_Amounts",
                $"[{nameof(Payment.Amount)}] > 0");
        });

        builder.HasKey(x => x.PaymentId);
        
        builder.HasIndex(x => x.BookingId)
            .IsUnique()
            .HasDatabaseName("IX_Payments_BookingId");
        
        builder.HasOne(x => x.Booking)
            .WithOne(x => x.Payment)
            .HasForeignKey<Payment>(x => x.BookingId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_Payments_Bookings");
        
        builder.Property(x => x.Provider)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Amount)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.TransactionId)
            .HasMaxLength(255);
    }
}