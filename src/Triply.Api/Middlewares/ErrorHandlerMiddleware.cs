using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Triply.Api.ResponseHelper;
using Triply.Api.Responses;
using Triply.Application.Exceptions;
using Triply.Domain.Exceptions;

namespace Triply.Api.Middlewares;

public class ErrorHandlerMiddleware(RequestDelegate next)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
    };
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
            if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
                await ResponseHandler.WriteJsonResponse(
                    context, HttpStatusCode.Forbidden, 
                    ApiResponseMessages.Authentication.ForbiddenResource.Message,
                    ApiResponseMessages.Authentication.ForbiddenResource.Code);
            else if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                await ResponseHandler.WriteJsonResponse(
                    context, HttpStatusCode.Unauthorized,
                    ApiResponseMessages.Authentication.NotAuthenticated.Message, 
                    ApiResponseMessages.Authentication.NotAuthenticated.Code);
        }
        catch (Exception exp)
        {
            await HandleExceptionAsync(context, exp, JsonOptions);
        }
    }
    private async Task HandleExceptionAsync(
        HttpContext context, Exception exp,
        JsonSerializerOptions options)
    {
        if (context.Response.HasStarted) return;
        var response = new ApiResponse<object>() { Message = ApiResponseMessages.General.UnknownError.Message };
        switch (exp)
        {
            case UnprocessableEntityException e:
                response.Message = ApiResponseMessages.Validation.ValidationError.Message;
                response.Code = ApiResponseMessages.Validation.ValidationError.Code;
                context.Response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                response.Errors = new() { Fields = e.Errors, General = [] };
                break;
            
            case UnauthorizedAccessException:
                response.Message = ApiResponseMessages.Authentication.AccessDenied.Message;
                response.Code = ApiResponseMessages.Authentication.AccessDenied.Code;
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                break;
         
            case NotFoundException e:
                response.Message = e.Message;
                response.Code = e.Code;
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                break;
            
            case InvalidFormatException e:
                response.Message = e.Message;
                response.Code = e.Code;
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;
            
            case BadHttpRequestException:
                response.Message = ApiResponseMessages.Validation.InvalidRequestBody.Message;
                response.Code = ApiResponseMessages.Validation.InvalidRequestBody.Code;
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;
            
            case InvalidOperationException e:
                response.Message = e.Message;
                response.Code = ApiResponseMessages.Validation.InvalidOperation.Code;
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;
            
            case DbUpdateException:
                response.Message = ApiResponseMessages.Database.UpdateError.Message;
                response.Code = ApiResponseMessages.Database.UpdateError.Code;
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                break;
            
            case SecurityTokenExpiredException:
                response.Message = ApiResponseMessages.Authentication.TokenExpired.Message;
                response.Code = ApiResponseMessages.Authentication.TokenExpired.Code;
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                break;
            
            default:
                response.Message = ApiResponseMessages.General.UnknownError.Message;
                response.Code = ApiResponseMessages.General.UnknownError.Code;
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                break;
        }
        context.Response.ContentType = "application/json; charset=utf-8";
        var result = JsonSerializer.Serialize(response, options);
        await context.Response.WriteAsync(result, Encoding.UTF8);
    }
}