namespace Triply.Application.DTOs.Tokens;

/// <summary>Contains a generated access token and refresh token.</summary>
/// <param name="Access">The access JWT.</param>
/// <param name="Refresh">The refresh JWT, or an empty value when not requested.</param>
public record TokenResponse(string Access, string Refresh);