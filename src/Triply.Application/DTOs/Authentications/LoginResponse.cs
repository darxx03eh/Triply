namespace Triply.Application.DTOs.Authentications;

/// <summary>Authentication result containing access and refresh tokens.</summary>
/// <param name="Name">The user's display name.</param>
/// <param name="Access">The access JWT.</param>
/// <param name="Refresh">The refresh JWT.</param>
public record LoginResponse(string Name, string Access, string Refresh);