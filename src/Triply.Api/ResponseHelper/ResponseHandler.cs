using System.Net;
using System.Text.Json;
using Triply.Api.Responses;

namespace Triply.Api.ResponseHelper;

public static class ResponseHandler
{
    private static readonly JsonSerializerOptions JSONOPTIONS = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
    };

    public static async Task WriteJsonResponse(
        HttpContext context,
        HttpStatusCode statusCode,
        string message, string code)
    {
        if (context.Response.HasStarted) return;
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json; charset=utf-8";

        var response = new ApiResponse<object>()
        {
            Message = message,
            Code = code,
            Errors = new ApiErrors()
            {
                Fields = [],
                General = []
            }
        };
        var json = JsonSerializer.Serialize(response, JSONOPTIONS);
        await context.Response.WriteAsync(json);
    }
}