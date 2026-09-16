using System.ComponentModel.DataAnnotations;

namespace Triply.Infrastructure.Settings;

public class JwtSettings
{
    [Required(AllowEmptyStrings = false)]
    public string SecretKey { get; set; } = null!;
    [Required(AllowEmptyStrings = false)]
    public string Issuer { get; set; } = null!;
    [Required(AllowEmptyStrings = false)]
    public string Audience { get; set; } = null!;

    public bool ValidateIssuer { get; set; } = true;
    public bool ValidateAudience { get; set; } = true;
    public bool ValidateIssuerSigningKey { get; set; } = true;
    public bool ValidateLifetime { get; set; } = true;
    
    [Range(1, int.MaxValue, ErrorMessage = "ExpiryMinutes must be greater than 0.")]
    public int ExpiryMinutes { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "RefreshTokenExpiryDays must be greater than 0.")]
    public int RefreshTokenExpiryDays { get; set; }
}