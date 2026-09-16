using Triply.Api.Extensions;
using Triply.Api.Routes;
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
        }
    }
}