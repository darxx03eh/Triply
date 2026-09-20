using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Triply.Domain.Entities.Identity;

namespace Triply.Infrastructure.Configurations.Identity;

/// <summary>Entity Framework configuration of the refresh token entity.</summary>
public class RefreshTokenConfigurations : IEntityTypeConfiguration<RefreshToken>
{
    /// <summary>
    /// Apply Refresh Tokens Configurations
    /// </summary>
    /// <param name="builder"></param>
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");
        
        builder.HasOne(r => r.User)
            .WithMany(r => r.RefreshTokens)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_RefreshTokens_Users");
        
        builder.HasIndex(r => r.Token)
            .IsUnique()
            .HasDatabaseName("IX_RefreshTokens_Token");
        
        builder.HasKey(r => r.RefreshId);
        
        builder.Property(r => r.Token)
            .IsRequired();
        
        builder.Property(r => r.AddedDate)
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRequired();
    }
}