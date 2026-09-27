using Triply.Api.Common;
using Triply.Api.Responses;

namespace Triply.Api.Extensions;

public static class RateLimitEndpointExtensions
{
    public static TBuilder WithRateLimit<TBuilder>(
        this TBuilder builder, string name,
        int permitLimit, TimeSpan window,
        ApiMessage response) where TBuilder : IEndpointConventionBuilder
    {
        builder.WithMetadata(
            new RateLimitMetadata(name, permitLimit, window, response));
        return builder;
    }
}