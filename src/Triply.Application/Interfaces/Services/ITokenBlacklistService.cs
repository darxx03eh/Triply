namespace Triply.Application.Interfaces.Services;

public interface ITokenBlacklistService
{
    Task BlacklistTokenAsync(string jti, DateTime expiryUtc, CancellationToken cancellationToken = default);
    Task<bool> IsBlacklistedAsync(string jti, CancellationToken cancellationToken = default);
}