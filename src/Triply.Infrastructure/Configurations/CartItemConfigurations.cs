using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Triply.Domain.Entities;

namespace Triply.Infrastructure.Configurations;

/// <summary>Entity Framework configuration of the cart item entity.</summary>
public class CartItemConfigurations : IEntityTypeConfiguration<CartItem>
{
    /// <summary>
    /// Apply CartItem Configurations
    /// </summary>
    /// <param name="builder"></param>
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable("CartItems", x =>
        {
            x.HasCheckConstraint("CK_CartItems_Adults",
                $"[{nameof(CartItem.Adults)}] > 0");

            x.HasCheckConstraint("CK_CartItems_Children",
                $"[{nameof(CartItem.Children)}] >= 0");

            x.HasCheckConstraint("CK_CartItems_Date",
                $"[{nameof(CartItem.CheckOut)}] > [{nameof(CartItem.CheckIn)}]");
        }).HasQueryFilter(x => !x.Room.IsDeleted);

        builder.HasIndex(x => x.CartItemId);
        
        builder.HasIndex(x => x.UserId)
            .HasDatabaseName("IX_CartItems_UserId");

        builder.HasOne(x => x.User)
            .WithMany(x => x.CartItems)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_CartItems_Users");

        builder.HasOne(x => x.Room)
            .WithMany(x => x.CartItems)
            .HasForeignKey(x => x.RoomId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_CartItems_Rooms");
        
        builder.Property(x => x.Adults)
            .IsRequired();

        builder.Property(x => x.Children)
            .HasDefaultValue(0)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRequired();
    }
}