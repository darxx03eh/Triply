namespace Triply.Application.Features.Authentications.Commands.Register;

/// <summary>Details required to create a user account.</summary>
public class RegisterUserRequest
{
    /// <summary>Gets or sets the user's first name.</summary>
    public string FirstName { get; set; } = null!;
    /// <summary>Gets or sets the user's last name.</summary>
    public string LastName { get; set; } = null!;
    /// <summary>Gets or sets the user's email address.</summary>
    public string Email { get; set; } = null!;
    /// <summary>Gets or sets the username.</summary>
    public string Username { get; set; } = null!;
    /// <summary>Gets or sets the password.</summary>
    public string Password { get; set; } = null!;
    /// <summary>Gets or sets the confirmation of the password.</summary>
    public string ConfirmPassword { get; set; } = null!;
    /// <summary>Gets or sets the user's phone number.</summary>
    public string PhoneNumber { get; set; } = null!;
    /// <summary>Gets or sets the user's optional date of birth.</summary>
    public DateTime? DateOfBirth { get; set; }
}