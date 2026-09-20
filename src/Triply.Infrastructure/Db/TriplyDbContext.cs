using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Triply.Domain.Entities;
using Triply.Domain.Entities.Identity;

namespace Triply.Infrastructure.Db;

/// <summary>Represents the Triply database context.</summary>
public class TriplyDbContext : IdentityDbContext<TriplyUser, TriplyRole, Guid,
    IdentityUserClaim<Guid>, IdentityUserRole<Guid>, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>,
    IdentityUserToken<Guid>>
{
    /// <summary>Initializes a new instance of the Triply database context.</summary>
    public TriplyDbContext(DbContextOptions<TriplyDbContext> options) : base(options) {}
    /// <summary>
    /// Apply the tables configurations when adding migrations files
    /// </summary>
    /// <param name="builder"></param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
    
    // identity tables
    /// <summary>Gets or sets the refresh tokens.</summary>
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    /// <summary>Gets or sets the users.</summary>
    public DbSet<TriplyUser> Users { get; set; }
    /// <summary>Gets or sets the roles.</summary>
    public DbSet<TriplyRole> Roles { get; set; }
    
    // regular tables
    /// <summary>Gets or sets the amenities.</summary>
    public DbSet<Amenity> Amenities { get; set; }
    /// <summary>Gets or sets the bookings.</summary>
    public DbSet<Booking> Bookings { get; set; }
    /// <summary>Gets or sets the cities.</summary>
    public DbSet<City> Cities { get; set; }
    /// <summary>Gets or sets the hotels.</summary>
    public DbSet<Hotel> Hotels { get; set; }
    /// <summary>Gets or sets the hotel amenities.</summary>
    public DbSet<HotelAmenities> HotelAmenities { get; set; }
    /// <summary>Gets or sets the hotel images.</summary>
    public DbSet<HotelImage> HotelImages { get; set; }
    /// <summary>Gets or sets the payments.</summary>
    public DbSet<Payment> Payments { get; set; }
    /// <summary>Gets or sets the rooms.</summary>
    public DbSet<Room> Rooms { get; set; }
    /// <summary>Gets or sets the user recent visits.</summary>
    public DbSet<UserRecentVisit>  UserRecentVisits { get; set; }
    /// <summary>Gets or sets the attractions.</summary>
    public DbSet<Attraction> Attractions { get; set; }
    /// <summary>Gets or sets the reviews.</summary>
    public DbSet<Review> Reviews { get; set; }
    /// <summary>Gets or sets the deals.</summary>
    public DbSet<Deal> Deals { get; set; }
}