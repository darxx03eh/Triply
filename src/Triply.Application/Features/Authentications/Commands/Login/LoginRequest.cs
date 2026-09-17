namespace Triply.Application.Features.Authentications.Commands.Login;

public class LoginRequest
{
    public string Identifier { get; set; } = null!;
    public string Password { get; set; } = null!;
}