using Triply.Application.Common.Models;
using Triply.Application.DTOs.Users;
using Triply.Application.Features.Users.Queries.GetUsersRequest;
using Triply.Domain.Results;

namespace Triply.Application.Interfaces.Services;

/// <summary>Defines administrative user-directory operations.</summary>
public interface IUserService
{
    /// <summary>Gets a filtered and paginated list of users.</summary>
    Task<Result<PagedResult<UserResponse>>> GetPagedAsync(GetUsersRequest request,
        CancellationToken cancellationToken = default);
}
