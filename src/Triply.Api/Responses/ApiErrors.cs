namespace Triply.Api.Responses;

public class ApiErrors
{
    public Dictionary<string, List<string>> Fields { get; set; } = [];
    public List<ApiError> General { get; set; } = [];
}