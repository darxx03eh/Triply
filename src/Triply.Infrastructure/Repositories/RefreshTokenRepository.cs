using Triply.Application.Interfaces.Repositories;
using Triply.Domain.Entities.Identity;
using Triply.Infrastructure.Db;
using Triply.Infrastructure.Repositories.General;

namespace Triply.Infrastructure.Repositories;

/// <summary>Gets or sets the refresh token repository.</summary>
/// <summary>Persistence operations for refresh token.</summary>
public class RefreshTokenRepository(TriplyDbContext context) : GenericRepository<RefreshToken>(context),
    IRefreshTokenRepository
{
    
}