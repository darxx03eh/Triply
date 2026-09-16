namespace Triply.Api.Responses;

public class ApiResponse<TResult>
{
    public TResult? Data { get; set; }
    public string? Message { get; set; }
    public string? Code { get; set; }
    public ApiErrors? Errors { get; set; }
}