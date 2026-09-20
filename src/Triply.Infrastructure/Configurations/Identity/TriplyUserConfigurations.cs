using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Triply.Domain.Entities.Identity;

namespace Triply.Infrastructure.Configurations.Identity;

/// <summary>Entity Framework configuration of the Triply user entity.</summary>
public class TriplyUserConfigurations : IEntityTypeConfiguration<TriplyUser>
{
    /// <summary>
    /// Apply Users Configurations
    /// </summary>
    /// <param name="builder"></param>
    public void Configure(EntityTypeBuilder<TriplyUser> builder)
    {
        builder.ToTable("AspNetUsers")
            .HasQueryFilter(u => !u.IsDeleted);

        builder.Property(u => u.FirstName)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.LastName)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.DateOfBirth)
            .IsRequired();
        
        builder.Property(u => u.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRequired();

        builder.Property(u => u.LastLoginAt)
            .IsRequired(false);

        builder.Property(u => u.IsDeleted)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(u => u.IsActive)
            .HasDefaultValue(true)
            .IsRequired();
        
        builder.HasIndex(u => u.PhoneNumber)
            .IsUnique()
            .HasDatabaseName("IX_Users_PhoneNumber");
    }
}