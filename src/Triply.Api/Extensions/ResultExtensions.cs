using Triply.Api.Responses;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Api.Extensions;

public static class ResultExtensions
{
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
            ResultErrorType.Unauthorized => Results.Json(errorResponse, statusCode: StatusCodes.Status401Unauthorized),
            ResultErrorType.Forbidden => Results.StatusCode(StatusCodes.Status403Forbidden),
            _ => Results.UnprocessableEntity(errorResponse)
        };
    }
}