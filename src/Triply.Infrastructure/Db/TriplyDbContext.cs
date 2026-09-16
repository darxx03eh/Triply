using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Triply.Domain.Entities;
using Triply.Domain.Entities.Identity;

namespace Triply.Infrastructure.Db;

public class TriplyDbContext : IdentityDbContext<TriplyUser, TriplyRole, Guid,
    IdentityUserClaim<Guid>, IdentityUserRole<Guid>, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>,
    IdentityUserToken<Guid>>
{
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
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<TriplyUser> Users { get; set; }
    public DbSet<TriplyRole> Roles { get; set; }
    
    // regular tables
    public DbSet<Amenity> Amenities { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<Hotel> Hotels { get; set; }
    public DbSet<HotelAmenities> HotelAmenities { get; set; }
    public DbSet<HotelImage> HotelImages { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<UserRecentVisit>  UserRecentVisits { get; set; }
}