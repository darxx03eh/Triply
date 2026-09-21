namespace Triply.Domain.Constants;

/// <summary>Names of claims written to Triply JWTs.</summary>
public static class TokenClaims
{
    /// <summary>Claim identifying the token kind.</summary>
    public const string Type = "type";
    /// <summary>Claim containing the unique JWT identifier.</summary>
    public const string Jti = "jti";
    /// <summary>Claim containing the user identifier.</summary>
    public const string Id = "id";
    /// <summary>Claim containing the username.</summary>
    public const string Username = "username";
    /// <summary>Claim containing the email address.</summary>
    public const string Email = "email";
    /// <summary>Claim containing the user's phone number.</summary>
    public const string PhoneNumber = "phoneNumber";
    /// <summary>Claim containing a role.</summary>
    public const string Role = "role";
}
