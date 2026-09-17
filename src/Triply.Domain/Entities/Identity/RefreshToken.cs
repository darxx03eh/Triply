namespace Triply.Domain.Entities.Identity;

/// <summary>Represents a refresh token issued to a user.</summary>
public sealed class RefreshToken
{
    /// <summary>Initializes an active refresh token.</summary>
    public RefreshToken()
    {
        RefreshId = Guid.NewGuid();
        IsActive = true;
        IsRevoked = false;
        AddedDate = DateTime.UtcNow;
    }
    /// <summary>Gets or sets the refresh-token identifier.</summary>
    public Guid RefreshId { get; set; }
    /// <summary>Gets or sets the JWT identifier.</summary>
    public string Jti { get; set; }
    /// <summary>Gets or sets the owning user identifier.</summary>
    public Guid UserId { get; set; }
    /// <summary>Gets or sets the serialized refresh token.</summary>
    public string Token { get; set; }
    /// <summary>Gets or sets whether the token can be used.</summary>
    public bool IsActive { get; set; }
    /// <summary>Gets or sets whether the token has been revoked.</summary>
    public bool IsRevoked { get; set; }
    /// <summary>Gets or sets when the token was issued.</summary>
    public DateTime AddedDate  { get; set; }
    /// <summary>Gets or sets when the token expires.</summary>
    public DateTime ExpiryDate   { get; set; }
    /// <summary>Gets or sets the owning user navigation property.</summary>
    public TriplyUser User { get; set; }
}