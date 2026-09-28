using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;
using Triply.Application.Interfaces.Repositories;
using Triply.Domain.Entities.Identity;
using Triply.Infrastructure.Db;
using Triply.Infrastructure.Repositories.General;

namespace Triply.Infrastructure.Repositories;

/// <summary>Gets or sets the user repository.</summary>
/// <summary>Persistence operations for user.</summary>
public class UserRepository(TriplyDbContext context, ISieveProcessor sieveProcessor) : GenericRepository<TriplyUser>(context),
    IUserRepository
{
    /// <summary>Checks whether the email exists.</summary>
    public async Task<bool> IsEmailExistsAsync(string email, CancellationToken cancellationToken = default)
        => await context.Users.AnyAsync(user => user.NormalizedEmail == email.ToUpperInvariant(), cancellationToken);

    /// <summary>Checks whether the username exists.</summary>
    public async Task<bool> IsUsernameExistsAsync(string username, CancellationToken cancellationToken = default)
        => await context.Users.AnyAsync(user => user.NormalizedUserName == username.ToUpperInvariant(),
            cancellationToken);

    /// <summary>Gets the user by its phone number.</summary>
    public Task<TriplyUser?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default)
        => context.Users.FirstOrDefaultAsync(user => user.PhoneNumber == phoneNumber, cancellationToken);

    /// <inheritdoc />
    public async Task<(List<TriplyUser> Users, int TotalCount)> GetPagedAsync(SieveModel sieveModel,
        CancellationToken cancellationToken = default)
    {
        var query = sieveProcessor.Apply(sieveModel, context.Users.AsNoTracking(), applyPagination: false);
        var totalCount = await query.CountAsync(cancellationToken);
        var users = await sieveProcessor
            .Apply(sieveModel, query, applyFiltering: false, applySorting: false)
            .ToListAsync(cancellationToken);

        return (users, totalCount);
    }
}
