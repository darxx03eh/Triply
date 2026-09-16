using Microsoft.AspNetCore.Mvc;
using Triply.Api.Extensions;
using Triply.Application.Features.Authentications.Commands.EmailConfirmation;
using Triply.Infrastructure.Routes;
using Triply.Application.Features.Authentications.Commands.Register;
using Triply.Application.Interfaces.Services;

namespace Triply.Api.Endpoints;

public static class AuthenticationEndpoints
{
    extension(IEndpointRouteBuilder app)
    {
        public void MapAuthenticationEndpoints()
        {
            var group = app.MapGroup("")
                .WithTags("Authentications");

            group.MapPost(Router.AuthenticationRoutes.Register, async (
                    RegisterUserRequest request,
                    IAuthenticationService authenticationService,
                    CancellationToken cancellationToken) =>
                {
                    var result = await authenticationService.RegisterNewUserAsync(request, cancellationToken);
                    return result.ToMinimalApiResult();
                }).AllowAnonymous()
                .WithName("RegisterNewUser")
                .WithDisplayName("Register New User")
                .WithSummary("Registers a new user account")
                .WithDescription("""
                                 Creates a new user account in the system using the provided registration details,
                                 such as username, email, and password. Returns the full name, user id and email.
                                 """)
                .Produces(StatusCodes.Status201Created);
            
            group.MapGet(Router.AuthenticationRoutes.EmailConfirmation, async (
                [FromQuery] string email,
                [FromQuery] string token,
                IAuthenticationService authenticationService,
                CancellationToken cancellationToken) =>
            {
                var request = new ConfirmEmailRequest
                {
                    Email = email,
                    Token = token
                };
                var result =
                    await authenticationService.ConfirmationEmailAsync(request, cancellationToken);
                return result.ToMinimalApiResult();
            }).AllowAnonymous()
            .WithName("ConfirmEmail")
            .WithDisplayName("Confirm Email")
            .WithSummary("Confirms a user's email address")
            .WithDescription("""
                             Confirms a user's email address using the email address and
                             confirmation token provided in the verification link.
                             """)
            .Produces(StatusCodes.Status200OK);
        }
    }
}