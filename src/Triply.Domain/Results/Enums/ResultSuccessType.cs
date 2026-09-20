namespace Triply.Domain.Results.Enums;

/// <summary>The kinds of success a result can carry, each mapped to an HTTP status code.</summary>
public enum ResultSuccessType
{
    /// <summary>The request succeeded (200).</summary>
    Ok,
    /// <summary>The resource was created (201).</summary>
    Created,
    /// <summary>The request succeeded with no body to return (204).</summary>
    NoContent,
    /// <summary>The request was accepted and is processed in the background (202).</summary>
    Accepted
}
