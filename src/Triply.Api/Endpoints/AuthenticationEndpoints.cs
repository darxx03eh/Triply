using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Triply.Api.Extensions;
using Triply.Api.Responses;
using Triply.Application.DTOs.Authentications;
using Triply.Application.Extensions;
using Triply.Application.Features.Authentications.Commands.EmailConfirmation;
using Triply.Application.Features.Authentications.Commands.Login;
using Triply.Infrastructure.Routes;
using Triply.Application.Features.Authentications.Commands.Register;
using Triply.Application.Interfaces.Services;
using Triply.Domain.Results;
using Triply.Infrastructure.Settings;

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

            group.MapPost(Router.AuthenticationRoutes.Login, async (
                    LoginRequest request,
                    IAuthenticationService authenticationService,
                    IOptions<JwtSettings> jwtSettings,
                    HttpContext context,
                    IWebHostEnvironment env,
                    CancellationToken cancellationToken
                ) =>
                {
                    var result = await authenticationService.LoginAsync(request, cancellationToken);
                    if (!result.IsSuccess)
                        return result.ToMinimalApiResult();

                    SetRefreshTokenCookie(context, result.Value!.Refresh, jwtSettings.Value, env);

                    var response = result.Value!.ToLoginApiResponse();
                    return Results.Ok(new ApiResponse<LoginApiResponse>()
                    {
                        Data = response,
                        Message = ResultResponseMessages.Authentication.Api.LoginSucceeded.Message,
                        Code =  ResultResponseMessages.Authentication.Api.LoginSucceeded.Code
                    });
                })
                .AllowAnonymous()
                .WithName("Login")
                .WithDisplayName("User Login")
                .WithSummary("Authenticates a user and returns a JWT token")
                .WithDescription("""
                                 Validates the provided credentials (identifier and password) and, if valid,
                                 returns a JWT access token to be used for authenticating subsequent requests.
                                 """)
                .Produces(StatusCodes.Status200OK);

            group.MapDelete(Router.AuthenticationRoutes.Logout, async (
                HttpContext context,
                IAuthenticationService authenticationService,
                ClaimsPrincipal user,
                CancellationToken cancellationToken
            ) =>
            {
                var result = await authenticationService.Logout(user, cancellationToken);
                if (!result.IsSuccess)
                    return result.ToMinimalApiResult();

                RemoveRefreshTokenCookie(context);

                return Results.NoContent();
            }).RequireAuthorization()
            .WithName("Logout")
            .WithDisplayName("User Logout")
            .WithSummary("Logs out the authenticated user")
            .WithDescription("""
                             Invalidates the authenticated user's refresh token and removes
                             the refresh token cookie from the client.
                             """)
            .Produces(StatusCodes.Status204NoContent);
        }

        private static void RemoveRefreshTokenCookie(HttpContext context)
        {
            context.Response.Cookies.Delete("refresh",
                new CookieOptions { Path = Router.AuthenticationRoutes.Refresh });
        }
        private static void SetRefreshTokenCookie(
            HttpContext context,
            string refresh,
            JwtSettings jwtSettings,
            IWebHostEnvironment env)
        {
            context.Response.Cookies.Append("refresh", refresh, new CookieOptions()
            {
                HttpOnly = true,
                Secure = !env.IsDevelopment(),
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(jwtSettings.RefreshTokenExpiryDays),
                Path = Router.AuthenticationRoutes.Refresh
            });
        }
    }
}