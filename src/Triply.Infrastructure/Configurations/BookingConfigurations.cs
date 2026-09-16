using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Triply.Domain.Entities;
using Triply.Domain.Enums.Bookings;

namespace Triply.Infrastructure.Configurations;

public class BookingConfigurations : IEntityTypeConfiguration<Booking>
{
    /// <summary>
    /// Apply Booking Configurations
    /// </summary>
    /// <param name="builder"></param>
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("Bookings", x =>
        {
            var statuses = string.Join(", ", Enum.GetNames<BookingStatus>().Select(status => $"'{status}'"));
            x.HasCheckConstraint("CK_Bookings_Status",
                $"[{nameof(Booking.Status)}] IN ({statuses})");
            
            x.HasCheckConstraint("CK_Bookings_Adults", 
                $"[{nameof(Booking.Adults)}] > 0");
            
            x.HasCheckConstraint("CK_Bookings_Children",
                $"[{nameof(Booking.Children)}] >= 0");
            
            x.HasCheckConstraint("CK_Bookings_TotalPrice",
                $"[{nameof(Booking.TotalPrice)}] >= 0");

            x.HasCheckConstraint("CK_Bookings_Date",
                $"[{nameof(Booking.CheckOut)}] > [{nameof(Booking.CheckIn)}]");
        });

        builder.HasKey(x => x.BookingId);

        builder.HasIndex(x => new { x.RoomId, x.CheckIn, x.CheckOut })
            .HasDatabaseName("IX_Bookings_RoomId_Dates");

        builder.HasIndex(x => x.UserId)
            .HasDatabaseName("IX_Bookings_UserId");
        
        builder.HasOne(x => x.User)
            .WithMany(x => x.Bookings)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_Bookings_Users");
        
        builder.HasOne(x => x.Room)
            .WithMany(x => x.Bookings)
            .HasForeignKey(x => x.RoomId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_Bookings_Rooms");
        
        builder.Property(x => x.Adults)
            .IsRequired();

        builder.Property(x => x.Children)
            .HasDefaultValue(0)
            .IsRequired();

        builder.Property(x => x.TotalPrice)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.SpecialRequests)
            .HasMaxLength(2000)
            .IsRequired(false);
    }
}