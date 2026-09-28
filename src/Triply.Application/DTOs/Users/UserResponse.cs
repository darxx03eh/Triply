namespace Triply.Application.DTOs.Users;

/// <summary>Safe administrative representation of an application user.</summary>
public sealed record UserResponse
{
    /// <summary>Gets the user identifier.</summary>
    public Guid UserId { get; init; }
    /// <summary>Gets the first name.</summary>
    public string FirstName { get; init; } = string.Empty;
    /// <summary>Gets the last name.</summary>
    public string LastName { get; init; } = string.Empty;
    /// <summary>Gets the username.</summary>
    public string? UserName { get; init; }
    /// <summary>Gets the email address.</summary>
    public string? Email { get; init; }
    /// <summary>Gets the phone number.</summary>
    public string? PhoneNumber { get; init; }
    /// <summary>Gets whether the email has been confirmed.</summary>
    public bool EmailConfirmed { get; init; }
    /// <summary>Gets the assigned roles.</summary>
    public IReadOnlyList<string> Roles { get; init; } = [];
    /// <summary>Gets whether the account is active.</summary>
    public bool IsActive { get; init; }
    /// <summary>Gets whether the account has been soft deleted.</summary>
    public bool IsDeleted { get; init; }
    /// <summary>Gets the account creation time.</summary>
    public DateTime CreatedAt { get; init; }
    /// <summary>Gets the last successful login time.</summary>
    public DateTime? LastLoginAt { get; init; }
}
