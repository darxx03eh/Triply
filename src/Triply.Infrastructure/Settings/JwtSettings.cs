using System.ComponentModel.DataAnnotations;

namespace Triply.Infrastructure.Settings;

/// <summary>Configuration values used to create and validate JWTs.</summary>
public class JwtSettings
{
    /// <summary>Gets or sets the symmetric signing key.</summary>
    [Required(AllowEmptyStrings = false)]
    public string SecretKey { get; set; } = null!;
    /// <summary>Gets or sets the token issuer.</summary>
    [Required(AllowEmptyStrings = false)]
    public string Issuer { get; set; } = null!;
    /// <summary>Gets or sets the token audience.</summary>
    [Required(AllowEmptyStrings = false)]
    public string Audience { get; set; } = null!;

    /// <summary>Gets or sets whether the issuer is validated.</summary>
    public bool ValidateIssuer { get; set; } = true;
    /// <summary>Gets or sets whether the audience is validated.</summary>
    public bool ValidateAudience { get; set; } = true;
    /// <summary>Gets or sets whether the signing key is validated.</summary>
    public bool ValidateIssuerSigningKey { get; set; } = true;
    /// <summary>Gets or sets whether token lifetime is validated.</summary>
    public bool ValidateLifetime { get; set; } = true;
    
    /// <summary>Gets or sets the access-token lifetime in minutes.</summary>
    [Range(1, int.MaxValue, ErrorMessage = "ExpiryMinutes must be greater than 0.")]
    public int ExpiryMinutes { get; set; }
    /// <summary>Gets or sets the refresh-token lifetime in days.</summary>
    [Range(1, int.MaxValue, ErrorMessage = "RefreshTokenExpiryDays must be greater than 0.")]
    public int RefreshTokenExpiryDays { get; set; }
}