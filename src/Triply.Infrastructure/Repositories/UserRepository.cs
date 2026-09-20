using Microsoft.EntityFrameworkCore;
using Triply.Application.Interfaces.Repositories;
using Triply.Domain.Entities.Identity;
using Triply.Infrastructure.Db;
using Triply.Infrastructure.Repositories.General;

namespace Triply.Infrastructure.Repositories;

/// <summary>Gets or sets the user repository.</summary>
/// <summary>Persistence operations for user.</summary>
public class UserRepository(TriplyDbContext context) : GenericRepository<TriplyUser>(context),
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
}