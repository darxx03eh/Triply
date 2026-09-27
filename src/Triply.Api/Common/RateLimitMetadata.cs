using Triply.Api.Responses;

namespace Triply.Api.Common;

/// <summary>Describes a rate limiting policy applied to an endpoint.</summary>
/// <param name="Name">The name of the rate limiting policy.</param>
/// <param name="PermitLimit">The maximum number of requests permitted within the window.</param>
/// <param name="Window">The time window during which the permit limit applies.</param>
/// <param name="Response">The response returned to the caller when the rate limit is exceeded.</param>
public sealed record RateLimitMetadata(string Name, int PermitLimit, TimeSpan Window, ApiMessage Response);