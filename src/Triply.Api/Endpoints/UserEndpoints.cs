using Microsoft.AspNetCore.Mvc;
using Triply.Api.Extensions;
using Triply.Api.Responses;
using Triply.Application.Common.Models;
using Triply.Application.DTOs.Users;
using Triply.Application.Features.Users.Queries.GetUsersRequest;
using Triply.Application.Interfaces.Services;
using Triply.Domain.Constants;
using Triply.Infrastructure.Routes;

namespace Triply.Api.Endpoints;

/// <summary>Maps administrative user-directory endpoints.</summary>
public static class UserEndpoints
{
    extension(IEndpointRouteBuilder app)
    {
        /// <summary>Maps administrative user-directory endpoints.</summary>
        public void MapUserEndpoints()
        {
            app.MapGet(Router.UserRoutes.GetAll, async (
                    [AsParameters] GetUsersRequest request,
                    IUserService userService,
                    CancellationToken cancellationToken) =>
                {
                    var result = await userService.GetPagedAsync(request, cancellationToken);
                    return result.ToMinimalApiResult();
                })
                .RequireAuthorization(policy => policy.RequireRole(Roles.Admin))
                .WithTags("Users")
                .WithName("GetAllUsers")
                .WithDisplayName("Get All Users")
                .WithSummary("Retrieves the administrative user directory")
                .WithDescription("Retrieves a paginated list of users for administrators. Password and security data are never returned.")
                .WithRateLimit(
                    "get-all-users",
                    60,
                    TimeSpan.FromMinutes(1),
                    ApiResponseMessages.User.GetAllRateLimited)
                .Produces<ApiResponse<PagedResult<UserResponse>>>(StatusCodes.Status200OK);
        }
    }
}
