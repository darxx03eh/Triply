using Triply.Application.DTOs.Tokens;
using Triply.Domain.Entities.Identity;

namespace Triply.Infrastructure.Services.Tokens;
public partial class TokenService
{
    public Task<TokenResponse> GenerateAccessTokenAsync(TriplyUser user, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}