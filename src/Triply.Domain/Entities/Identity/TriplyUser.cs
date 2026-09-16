using Microsoft.AspNetCore.Identity;

namespace Triply.Domain.Entities.Identity;

public sealed class TriplyUser : IdentityUser<Guid>
{
    public TriplyUser()
    {
        RefreshTokens = new HashSet<RefreshToken>();
        CreatedAt = DateTime.UtcNow;
        IsDeleted = false;
        IsActive = true;
    }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsActive { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; set; }
}