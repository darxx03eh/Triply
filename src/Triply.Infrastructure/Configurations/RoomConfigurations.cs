using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Triply.Domain.Entities;
using Triply.Domain.Enums.Rooms;

namespace Triply.Infrastructure.Configurations;

public class RoomConfigurations : IEntityTypeConfiguration<Room>
{
    /// <summary>
    /// Apply Rooms Configurations
    /// </summary>
    /// <param name="builder"></param>
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.ToTable("Rooms", x =>
        {
            var roomTypes = string.Join(", ", Enum.GetNames<RoomType>().Select(room => $"'{room}'"));
            x.HasCheckConstraint("CK_Rooms_RoomType",
                $"[{nameof(Room.RoomType)}] IN ({roomTypes})");

            x.HasCheckConstraint("CK_Rooms_AdultCapacity",
                $"[{nameof(Room.AdultCapacity)}] >= 0");
            
            x.HasCheckConstraint("CK_Rooms_ChildCapacity", 
                $"[{nameof(Room.ChildCapacity)}] >= 0");
            
            x.HasCheckConstraint("CK_Rooms_PricePerNight", 
                $"[{nameof(Room.PricePerNight)}] >= 0");
        })
        .HasQueryFilter(x => !x.IsDeleted);

        builder.HasKey(x => x.RoomId);
        
        builder.HasIndex(x => new { x.HotelId, x.Number})
            .HasFilter($"[{nameof(Room.IsDeleted)}] = 0")
            .IsUnique()
            .HasDatabaseName("UQ_Rooms_Hotel_Number");
        
        builder.HasIndex(x => x.HotelId)
            .HasDatabaseName("IX_Rooms_HotelId");
        
        builder.HasIndex(x => x.PricePerNight)
            .HasDatabaseName("IX_Rooms_PricePerNight");
        
        builder.HasOne(x => x.Hotel)
            .WithMany(x => x.Rooms)
            .HasForeignKey(x => x.HotelId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_Rooms_Hotels");
        
        builder.Property(x => x.Number)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.RoomType)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.AdultCapacity)
            .HasDefaultValue(2)
            .IsRequired();

        builder.Property(x => x.ChildCapacity)
            .HasDefaultValue(0)
            .IsRequired();

        builder.Property(x => x.PricePerNight)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        builder.Property(x => x.IsAvailable)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(x => x.RowVersion)
            .IsRowVersion();

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRequired();

    }
}