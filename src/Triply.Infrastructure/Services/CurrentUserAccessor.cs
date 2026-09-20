using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Triply.Application.Interfaces.Services;
using Triply.Domain.Constants;

namespace Triply.Infrastructure.Services;

/// <summary>Gets or sets the current user accessor.</summary>
/// <summary>Gives access to the current user of the request.</summary>
public class CurrentUserAccessor(IHttpContextAccessor httpContextAccessor) : ICurrentUserAccessor
{
    /// <summary>Gets or sets the identifier of the user.</summary>
    public Guid UserId
    {
        get
        {
            var claim = httpContextAccessor.HttpContext?.User.FindFirstValue(TokenClaims.Id);
            return claim is not null ? Guid.Parse(claim) : Guid.Empty;
        }
    }
    /// <summary>Gets whether the current user accessor is admin.</summary>
    public bool IsAdmin => httpContextAccessor.HttpContext?.User.IsInRole(Roles.Admin) ?? false;
}