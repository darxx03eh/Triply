using Triply.Application.Interfaces.Repositories.General;
using Triply.Domain.Entities.Identity;

namespace Triply.Application.Interfaces.Repositories;

public interface IUserRepository : IGenericRepository<TriplyUser>
{
    Task<bool> IsEmailExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> IsUsernameExistsAsync(string username, CancellationToken cancellationToken = default);
    Task<TriplyUser?> GetByPhoneNumberAsync(string phoneNumber,  CancellationToken cancellationToken = default);
}