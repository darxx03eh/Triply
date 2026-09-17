namespace Triply.Application.DTOs.Authentications;

/// <summary>Public response returned after a successful login or token refresh.</summary>
/// <param name="Name">The user's display name.</param>
/// <param name="Access">The access JWT.</param>
public record LoginApiResponse(string Name, string Access);