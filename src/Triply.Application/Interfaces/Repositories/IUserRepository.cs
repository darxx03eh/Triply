using Triply.Application.Interfaces.Repositories.General;
using Triply.Domain.Entities.Identity;

namespace Triply.Application.Interfaces.Repositories;

/// <summary>Provides persistence operations for application users.</summary>
public interface IUserRepository : IGenericRepository<TriplyUser>
{
    /// <summary>Checks whether an email address is already registered.</summary>
    Task<bool> IsEmailExistsAsync(string email, CancellationToken cancellationToken = default);
    /// <summary>Checks whether a username is already registered.</summary>
    Task<bool> IsUsernameExistsAsync(string username, CancellationToken cancellationToken = default);
    /// <summary>Finds a user by phone number.</summary>
    Task<TriplyUser?> GetByPhoneNumberAsync(string phoneNumber,  CancellationToken cancellationToken = default);
}