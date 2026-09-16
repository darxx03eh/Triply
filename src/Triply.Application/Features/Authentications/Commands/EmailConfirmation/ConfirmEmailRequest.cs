namespace Triply.Application.Features.Authentications.Commands.EmailConfirmation;

public class ConfirmEmailRequest
{
    public string Email { get; set; }
    public string Token { get; set; }
}