namespace Triply.Application.DTOs.Authentications;

public sealed record RegisterUserResponse(string Name, Guid Id, string Email, string Username);