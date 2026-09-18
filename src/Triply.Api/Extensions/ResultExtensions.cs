using Triply.Api.Responses;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Api.Extensions;

/// <summary>Converts domain results into Minimal API HTTP responses.</summary>
public static class ResultExtensions
{
    /// <summary>Maps a domain result to its corresponding HTTP response.</summary>
    /// <typeparam name="T">The result value type.</typeparam>
    /// <param name="result">The domain result to map.</param>
    /// <returns>A Minimal API result with the appropriate status code and response body.</returns>
    public static IResult ToMinimalApiResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
        {
            var successResponse = new ApiResponse<T>
            {
                Data = result.Value,
                Message = result.SuccessObject!.Message,
                Code = result.SuccessObject.Code
            };

            return result.SuccessType switch
            {
                ResultSuccessType.Created => Results.Json(successResponse, statusCode: StatusCodes.Status201Created),
                ResultSuccessType.NoContent => Results.NoContent(),
                ResultSuccessType.Accepted => Results.Json(successResponse, statusCode: StatusCodes.Status202Accepted),
                _ => Results.Ok(successResponse)
            };
        }

        if (result.Fields.Count > 0)
        {
            var validationResponse = new ApiResponse<object>
            {
                Message = ApiResponseMessages.Validation.ValidationError.Message,
                Code = ApiResponseMessages.Validation.ValidationError.Code,
                Errors = new ApiErrors
                {
                    Fields = result.Fields,
                    General = []
                }
            };
            return Results.UnprocessableEntity(validationResponse);
        }

        var error = result.Error!;

        var errorResponse = new ApiResponse<object>
        {
            Message = error.Message,
            Code = error.Code
        };

        return error.Type switch
        {
            ResultErrorType.NotFound => Results.NotFound(errorResponse),
            ResultErrorType.Conflict => Results.Conflict(errorResponse),
            ResultErrorType.BusinessRule => Results.BadRequest(errorResponse),
            ResultErrorType.Unauthorized => Results.Json(errorResponse, statusCode: StatusCodes.Status401Unauthorized),
            ResultErrorType.Forbidden => Results.StatusCode(StatusCodes.Status403Forbidden),
            _ => Results.UnprocessableEntity(errorResponse)
        };
    }
}