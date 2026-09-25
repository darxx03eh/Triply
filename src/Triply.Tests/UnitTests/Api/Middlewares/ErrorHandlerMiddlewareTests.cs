using System.Text.Json;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Serilog;
using Serilog.Extensions.Hosting;
using Triply.Api.Middlewares;
using Triply.Api.Responses;
using Triply.Application.Exceptions;
using Triply.Domain.Exceptions;
using SecurityTokenExpiredException = Microsoft.IdentityModel.Tokens.SecurityTokenExpiredException;

namespace Triply.Tests.UnitTests.Api.Middlewares;

public class ErrorHandlerMiddlewareTests
{
    private static async Task<(int Status, JsonElement Body)> InvokeAsync(RequestDelegate next)
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        await new ErrorHandlerMiddleware(next, new DiagnosticContext(Log.Logger),
            NullLogger<ErrorHandlerMiddleware>.Instance).InvokeAsync(context);

        context.Response.Body.Position = 0;
        var text = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var body = string.IsNullOrEmpty(text) ? default : JsonDocument.Parse(text).RootElement;
        return (context.Response.StatusCode, body);
    }

    private static RequestDelegate Throws(Exception exception) => _ => throw exception;

    [Fact]
    public async Task InvokeAsync_NoException_LeavesResponseUntouched()
    {
        var (status, body) = await InvokeAsync(ctx =>
        {
            ctx.Response.StatusCode = StatusCodes.Status200OK;
            return Task.CompletedTask;
        });

        Assert.Equal(StatusCodes.Status200OK, status);
        Assert.Equal(JsonValueKind.Undefined, body.ValueKind);
    }

    [Fact]
    public async Task InvokeAsync_UnprocessableEntityException_Returns422WithFieldErrors()
    {
        var failures = new[]
        {
            new ValidationFailure("Name", "Name is required."),
            new ValidationFailure("Name", "Name is too short."),
            new ValidationFailure("Country", "Country is required.")
        };

        var (status, body) = await InvokeAsync(Throws(new UnprocessableEntityException(failures)));

        Assert.Equal(StatusCodes.Status422UnprocessableEntity, status);
        Assert.Equal(ApiResponseMessages.Validation.ValidationError.Code, body.GetProperty("code").GetString());
        var fields = body.GetProperty("errors").GetProperty("fields");
        Assert.Equal(2, fields.GetProperty("name").GetArrayLength());
        Assert.Equal("Country is required.", fields.GetProperty("country")[0].GetString());
    }

    public static TheoryData<Exception, int, string> MappedExceptions => new()
    {
        { new NotFoundException("User missing", "USER_NOT_FOUND"), 
            StatusCodes.Status404NotFound, "USER_NOT_FOUND" },
        { new InvalidFormatException("Bad email", "INVALID_EMAIL"), 
            StatusCodes.Status400BadRequest, "INVALID_EMAIL" },
        { new InvalidOperationException("Nope"), StatusCodes.Status400BadRequest, 
            ApiResponseMessages.Validation.InvalidOperation.Code },
        { new BadHttpRequestException("Broken json"), StatusCodes.Status400BadRequest, 
            ApiResponseMessages.Validation.InvalidRequestBody.Code },
        { new UnauthorizedAccessException(), StatusCodes.Status401Unauthorized, 
            ApiResponseMessages.Authentication.AccessDenied.Code },
        { new SecurityTokenExpiredException(), StatusCodes.Status401Unauthorized, 
            ApiResponseMessages.Authentication.TokenExpired.Code },
        { new DbUpdateException("db"), StatusCodes.Status500InternalServerError, 
            ApiResponseMessages.Database.UpdateError.Code },
        { new Exception("boom"), StatusCodes.Status500InternalServerError, 
            ApiResponseMessages.General.UnknownError.Code }
    };

    [Theory]
    [MemberData(nameof(MappedExceptions))]
    public async Task InvokeAsync_KnownException_MapsToStatusAndCode(Exception exception, 
        int expectedStatus, string expectedCode)
    {
        var (status, body) = await InvokeAsync(Throws(exception));

        Assert.Equal(expectedStatus, status);
        Assert.Equal(expectedCode, body.GetProperty("code").GetString());
    }

    [Fact]
    public async Task InvokeAsync_NotFoundException_UsesExceptionMessage()
    {
        var (_, body) = await InvokeAsync(Throws(
            new NotFoundException("User missing", "USER_NOT_FOUND")));

        Assert.Equal("User missing", body.GetProperty("message").GetString());
    }

    [Theory]
    [InlineData(StatusCodes.Status401Unauthorized, "NOT_AUTHENTICATED")]
    [InlineData(StatusCodes.Status403Forbidden, "ACCESS_DENIED")]
    public async Task InvokeAsync_EmptyAuthResponse_WritesJsonBody(int statusCode, string expectedCode)
    {
        var (status, body) = await InvokeAsync(ctx =>
        {
            ctx.Response.StatusCode = statusCode;
            return Task.CompletedTask;
        });

        Assert.Equal(statusCode, status);
        Assert.Equal(expectedCode, body.GetProperty("code").GetString());
    }
}
