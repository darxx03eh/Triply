using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Triply.Api.ResponseHelper;
using Triply.Api.Responses;
using Triply.Application.Exceptions;
using Triply.Domain.Exceptions;
using Serilog;

namespace Triply.Api.Middlewares;

/// <summary>Gets or sets the error handler middleware.</summary>
/// <summary>Middleware that handles the error handler.</summary>
public class ErrorHandlerMiddleware(
    RequestDelegate next,
    IDiagnosticContext diagnostic,
    ILogger<ErrorHandlerMiddleware> logger)
{
    /// <summary>
    /// JSON Options to Apply when Writes the JSON response
    /// </summary>
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
    };
    /// <summary>Runs the middleware for the current request.</summary>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
            if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
            {
                diagnostic.Set("ResultCode", ApiResponseMessages.Authentication.ForbiddenResource.Code);
                await ResponseHandler.WriteJsonResponse(
                    context, HttpStatusCode.Forbidden, 
                    ApiResponseMessages.Authentication.ForbiddenResource.Message,
                    ApiResponseMessages.Authentication.ForbiddenResource.Code);
            }
            else if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
            {
                diagnostic.Set("ResultCode", ApiResponseMessages.Authentication.NotAuthenticated.Code);
                await ResponseHandler.WriteJsonResponse(
                    context, HttpStatusCode.Unauthorized,
                    ApiResponseMessages.Authentication.NotAuthenticated.Message, 
                    ApiResponseMessages.Authentication.NotAuthenticated.Code);
            }
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
        if (context.Response.HasStarted)
        {
            logger.LogError(exp, 
                "Unhandled exception after the response started for {RequestMethod} {RequestPath}",
                context.Request.Method, context.Request.Path);
            return;
        }
        var response = new ApiResponse<object>()
        {
            Message = ApiResponseMessages.General.UnknownError.Message
        };
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
        // The response is written here instead of ToMinimalApiResult, so the request log
        // gets the code from here (e.g. VALIDATION_ERROR from FluentValidation).
        diagnostic.Set("ResultCode", response.Code);
        diagnostic.Set("ErrorMessage", response.Message);
        if (exp is UnprocessableEntityException validationException)
            diagnostic.Set("ValidationErrors", validationException.Errors, destructureObjects: true);

        LogException(context, exp, response);
        context.Response.ContentType = "application/json; charset=utf-8";
        var result = JsonSerializer.Serialize(response, options);
        await context.Response.WriteAsync(result, Encoding.UTF8);
    }

    private void LogException(HttpContext context, Exception exp, ApiResponse<object> response)
    {
        var statusCode = context.Response.StatusCode;
        if (statusCode >= StatusCodes.Status500InternalServerError)
            logger.LogError(exp, "Request {RequestMethod} {RequestPath} failed with {StatusCode} ({ErrorCode})",
                context.Request.Method, context.Request.Path, statusCode, response.Code);
        else if (exp is UnprocessableEntityException validation)
            logger.LogWarning("Validation failed for {RequestMethod} {RequestPath}: {@ValidationErrors}",
                context.Request.Method, context.Request.Path, validation.Errors);
        else
            logger.LogWarning(
                "Request {RequestMethod} {RequestPath} returned {StatusCode} ({ErrorCode}): {ExceptionType} {ErrorMessage}",
                context.Request.Method, context.Request.Path, statusCode, response.Code, exp.GetType().Name, exp.Message);
    }
}
