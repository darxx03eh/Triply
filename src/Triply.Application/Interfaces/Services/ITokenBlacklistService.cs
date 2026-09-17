namespace Triply.Application.Interfaces.Services;

/// <summary>Stores and checks revoked access-token identifiers.</summary>
public interface ITokenBlacklistService
{
    /// <summary>Adds a token identifier to the blacklist until its expiration time.</summary>
    /// <param name="jti">The JWT identifier to blacklist.</param>
    /// <param name="expiryUtc">The UTC expiration time of the token.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task BlacklistTokenAsync(string jti, DateTime expiryUtc, CancellationToken cancellationToken = default);

    /// <summary>Checks whether a token identifier is currently blacklisted.</summary>
    /// <param name="jti">The JWT identifier to check.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns><see langword="true"/> when the identifier is blacklisted; otherwise, <see langword="false"/>.</returns>
    Task<bool> IsBlacklistedAsync(string jti, CancellationToken cancellationToken = default);
}