namespace Triply.Application.Interfaces.Services;

/// <summary>Defines the rate limit operations for failed login requests.</summary>
public interface IFailedLoginRateLimitService
{
    /// <summary>Determines whether the given IP address and user are currently blocked due to failed login attempts.</summary>
    Task<bool> IsBlockedAsync(string ip, string userId, CancellationToken cancellationToken = default);

    /// <summary>Records a failed login attempt for the given IP address and user, and determines whether this causes them to become blocked.</summary>
    Task<long> RecordFailureAsync(string ip, string userId, CancellationToken cancellationToken = default);

    /// <summary>Resets the recorded failed login attempts for the given IP address and user.</summary>
    Task ResetAsync(string ip, string userId, CancellationToken cancellationToken = default);
}