namespace Triply.Api.Responses;

/// <summary>Standard API response envelope.</summary>
/// <typeparam name="TResult">The response data type.</typeparam>
public class ApiResponse<TResult>
{
    /// <summary>Gets or sets the response data.</summary>
    public TResult? Data { get; set; }
    /// <summary>Gets or sets the response message.</summary>
    public string? Message { get; set; }
    /// <summary>Gets or sets the stable response code.</summary>
    public string? Code { get; set; }
    /// <summary>Gets or sets validation or general errors.</summary>
    public ApiErrors? Errors { get; set; }
}