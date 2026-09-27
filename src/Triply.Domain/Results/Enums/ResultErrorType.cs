namespace Triply.Domain.Results.Enums;

/// <summary>The kinds of failure a result can carry, each mapped to an HTTP status code.</summary>
public enum ResultErrorType
{
    /// <summary>The request body failed validation (422).</summary>
    Validation,
    /// <summary>The requested resource does not exist (404).</summary>
    NotFound,
    /// <summary>The request conflicts with the current state of the resource (409).</summary>
    Conflict,
    /// <summary>The caller is not authenticated (401).</summary>
    Unauthorized,
    /// <summary>The caller is authenticated but not allowed to do this (403).</summary>
    Forbidden,
    /// <summary>A business rule rejected the request (400).</summary>
    BusinessRule,
    /// <summary>The caller has sent too many requests in a given amount of time (429).</summary>
    ToManyRequest,
}