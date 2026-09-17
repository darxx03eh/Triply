using Triply.Application.Interfaces.Repositories.General;
using Triply.Domain.Entities.Identity;

namespace Triply.Application.Interfaces.Repositories;

/// <summary>Provides persistence operations for refresh tokens.</summary>
public interface IRefreshTokenRepository : IGenericRepository<RefreshToken>
{
    
}