using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Triply.Domain.Entities;
using Triply.Domain.Enums.Hotels;

namespace Triply.Infrastructure.Configurations;

public class HotelConfigurations : IEntityTypeConfiguration<Hotel>
{
    /// <summary>
    /// Apply Hotel Configurations
    /// </summary>
    /// <param name="builder"></param>
    public void Configure(EntityTypeBuilder<Hotel> builder)
    {
        builder.ToTable("Hotels", x =>
            {
                x.HasCheckConstraint("CK_Hotels_StarRating",
                    $"[{nameof(Hotel.StarRating)}] BETWEEN 1 AND 5");

                var hotelTypes = string.Join(", ", Enum.GetNames<HotelType>().Select(type => $"'{type}'"));
                x.HasCheckConstraint("CK_Hotels_HotelType",
                    $"[{nameof(Hotel.HotelType)}] IN ({hotelTypes})");
            })
            .HasQueryFilter(x => !x.IsDeleted);
        
        builder.HasOne(x => x.Owner)
            .WithMany(x => x.OwnedHotels)
            .HasForeignKey(x => x.OwnerId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_Hotels_Owners");
        
        builder.HasOne(x => x.City)
            .WithMany(x => x.Hotels)
            .HasForeignKey(x => x.CityId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_Hotels_Cities");
        
        builder.HasIndex(x => x.CityId)
            .HasDatabaseName("IX_Hotels_CityId");   
        
        builder.HasIndex(x => x.OwnerId)
            .HasDatabaseName("IX_Hotels_OwnerId");
        
        builder.HasIndex(x => x.StarRating)
            .HasDatabaseName("IX_Hotels_StarRating");

        builder.HasIndex(x => x.HotelType)
            .HasDatabaseName("IX_Hotels_HotelType");

        builder.HasKey(x => x.HotelId);
        
        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.StarRating)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(2000);

        builder.Property(x => x.HotelType)
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(HotelType.Budget)
            .IsRequired();

        builder.Property(x => x.Address)
            .HasMaxLength(300);

        builder.Property(x => x.Latitude)
            .HasPrecision(9, 6);

        builder.Property(x => x.Longitude)
            .HasPrecision(9, 6);

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