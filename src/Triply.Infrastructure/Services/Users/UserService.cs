using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Triply.Application.Common.Models;
using Triply.Application.DTOs.Users;
using Triply.Application.Features.Users.Queries.GetUsersRequest;
using Triply.Application.Interfaces.Repositories;
using Triply.Application.Interfaces.Services;
using Triply.Domain.Results;
using Triply.Infrastructure.Db;

namespace Triply.Infrastructure.Services.Users;

/// <summary>Provides the administrative user directory.</summary>
public sealed class UserService(
    IUserRepository userRepository,
    TriplyDbContext context,
    ILogger<UserService> logger) : IUserService
{
    /// <inheritdoc />
    public async Task<Result<PagedResult<UserResponse>>> GetPagedAsync(GetUsersRequest request,
        CancellationToken cancellationToken = default)
    {
        var (users, totalCount) = await userRepository.GetPagedAsync(request, cancellationToken);
        var userIds = users.Select(user => user.Id).ToList();

        var rolesByUserId = await context.UserRoles
            .Where(userRole => userIds.Contains(userRole.UserId))
            .Join(context.Roles,
                userRole => userRole.RoleId,
                role => role.Id,
                (userRole, role) => new { userRole.UserId, role.Name })
            .ToListAsync(cancellationToken);

        var roleLookup = rolesByUserId
            .GroupBy(item => item.UserId)
            .ToDictionary(group => group.Key,
                group => (IReadOnlyList<string>)group
                    .Where(item => item.Name is not null)
                    .Select(item => item.Name!)
                    .Order()
                    .ToList());

        var response = new PagedResult<UserResponse>
        {
            Items = users.Select(user => new UserResponse
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                EmailConfirmed = user.EmailConfirmed,
                Roles = roleLookup.GetValueOrDefault(user.Id, []),
                IsActive = user.IsActive,
                IsDeleted = user.IsDeleted,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt
            }).ToList(),
            Page = request.Page ?? 1,
            PageSize = request.PageSize ?? 10,
            TotalCount = totalCount
        };

        logger.LogDebug("Users page {Page} returned {Count} of {TotalCount}", response.Page,
            response.Items.Count, totalCount);

        return Result<PagedResult<UserResponse>>.Success(response);
    }
}
