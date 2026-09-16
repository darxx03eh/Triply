using Triply.Application.Interfaces.Repositories;
using Triply.Domain.Entities.Identity;
using Triply.Infrastructure.Db;
using Triply.Infrastructure.Repositories.General;

namespace Triply.Infrastructure.Repositories;

public class RefreshTokenRepository(TriplyDbContext context) : GenericRepository<RefreshToken>(context),
    IRefreshTokenRepository
{
    
}