namespace Triply.Domain.Entities.Identity;

public sealed class RefreshToken
{
    public RefreshToken()
    {
        RefreshId = Guid.NewGuid();
        IsActive = true;
        IsRevoked = false;
        AddedDate = DateTime.UtcNow;
    }
    public Guid RefreshId { get; set; }
    public string Jti { get; set; }
    public Guid UserId { get; set; }
    public string Token { get; set; }
    public bool IsActive { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime AddedDate  { get; set; }
    public DateTime ExpiryDate   { get; set; }
    public TriplyUser User { get; set; }
}