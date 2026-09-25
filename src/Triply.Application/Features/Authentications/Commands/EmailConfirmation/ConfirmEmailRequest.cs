namespace Triply.Application.Features.Authentications.Commands.EmailConfirmation;

/// <summary>Data required to confirm a user's email address.</summary>
public class ConfirmEmailRequest
{
    /// <summary>Gets or sets the email address being confirmed.</summary>
    public string? Email { get; set; }
    /// <summary>Gets or sets the confirmation token.</summary>
    public string? Token { get; set; }
}