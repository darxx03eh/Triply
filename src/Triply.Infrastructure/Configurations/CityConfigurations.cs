using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Triply.Domain.Entities;
using Triply.Domain.Entities.Identity;

namespace Triply.Infrastructure.Configurations;

public class CityConfigurations : IEntityTypeConfiguration<City>
{
    /// <summary>
    /// Apply City Configuration
    /// </summary>
    /// <param name="builder"></param>
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.ToTable("Cities")
            .HasQueryFilter(x => !x.IsDeleted);

        builder.HasKey(x => x.CityId);

        builder.HasIndex(x => new { x.Name, x.Country })
            .HasFilter($"[{nameof(TriplyUser.IsDeleted)}] = 0")
            .IsUnique()
            .HasDatabaseName("IX_Cities_Name_Country");
            
        
        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Country)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.PostOffice)
            .HasMaxLength(20);

        builder.Property(x => x.ThumbnailUrl)
            .HasMaxLength(1000);

        builder.Property(x => x.ThumbnailPublicId)
            .HasMaxLength(255);

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