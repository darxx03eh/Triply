namespace Triply.Api.Responses;

/// <summary>Represents a single API error.</summary>
public class ApiError
{
    /// <summary>Gets or sets the stable error code.</summary>
    public string Code { get; set; } = null!;
    /// <summary>Gets or sets the user-facing error message.</summary>
    public string Message { get; set; } = null!;
}