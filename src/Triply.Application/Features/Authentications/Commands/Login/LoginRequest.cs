namespace Triply.Application.Features.Authentications.Commands.Login;

/// <summary>Credentials submitted for authentication.</summary>
public class LoginRequest
{
    /// <summary>Gets or sets the email, username, or phone number.</summary>
    public string Identifier { get; set; } = null!;
    /// <summary>Gets or sets the user's password.</summary>
    public string Password { get; set; } = null!;
}