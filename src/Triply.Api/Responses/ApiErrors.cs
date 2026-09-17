namespace Triply.Api.Responses;

/// <summary>Groups field-level and general API errors.</summary>
public class ApiErrors
{
    /// <summary>Gets field names and their validation messages.</summary>
    public Dictionary<string, List<string>> Fields { get; set; } = [];
    /// <summary>Gets errors that are not associated with a specific field.</summary>
    public List<ApiError> General { get; set; } = [];
}