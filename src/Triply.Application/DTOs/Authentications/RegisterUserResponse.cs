namespace Triply.Application.DTOs.Authentications;

/// <summary>Public details returned for a newly registered user.</summary>
/// <param name="Name">The user's display name.</param>
/// <param name="Id">The user's identifier.</param>
/// <param name="Email">The user's email address.</param>
/// <param name="Username">The user's username.</param>
public sealed record RegisterUserResponse(string Name, Guid Id, string Email, string Username);