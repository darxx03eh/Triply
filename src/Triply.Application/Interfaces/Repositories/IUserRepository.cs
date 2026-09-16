using Triply.Application.Interfaces.Repositories.General;
using Triply.Domain.Entities.Identity;

namespace Triply.Application.Interfaces.Repositories;

public interface IUserRepository : IGenericRepository<TriplyUser>
{
    public Task<bool> IsEmailExistsAsync(string email, CancellationToken cancellationToken = default);
    public Task<bool> IsUsernameExistsAsync(string username, CancellationToken cancellationToken = default);
}