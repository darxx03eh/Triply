using Microsoft.EntityFrameworkCore;
using Triply.Application.Interfaces.Repositories;
using Triply.Domain.Entities.Identity;
using Triply.Infrastructure.Db;
using Triply.Infrastructure.Repositories.General;

namespace Triply.Infrastructure.Repositories;

public class UserRepository(TriplyDbContext context) : GenericRepository<TriplyUser>(context),
    IUserRepository
{
    public async Task<bool> IsEmailExistsAsync(string email, CancellationToken cancellationToken = default)
        => await context.Users.AnyAsync(user => user.NormalizedEmail == email.ToUpperInvariant(), cancellationToken);

    public async Task<bool> IsUsernameExistsAsync(string username, CancellationToken cancellationToken = default)
        => await context.Users.AnyAsync(user => user.NormalizedUserName == username.ToUpperInvariant(),
            cancellationToken);

    public Task<TriplyUser?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default)
        => context.Users.FirstOrDefaultAsync(user => user.PhoneNumber == phoneNumber, cancellationToken);
}