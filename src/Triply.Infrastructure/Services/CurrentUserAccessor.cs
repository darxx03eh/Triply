using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Triply.Application.Interfaces.Services;
using Triply.Domain.Constants;

namespace Triply.Infrastructure.Services;

public class CurrentUserAccessor(IHttpContextAccessor httpContextAccessor) : ICurrentUserAccessor
{
    public Guid UserId
    {
        get
        {
            var claim = httpContextAccessor.HttpContext?.User.FindFirstValue(TokenClaims.Id);
            return claim is not null ? Guid.Parse(claim) : Guid.Empty;
        }
    }
    public bool IsAdmin => httpContextAccessor.HttpContext?.User.IsInRole(Roles.Admin) ?? false;
}