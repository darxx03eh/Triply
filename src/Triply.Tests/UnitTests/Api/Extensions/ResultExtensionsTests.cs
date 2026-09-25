using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Triply.Api.Extensions;
using Triply.Api.Responses;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Tests.UnitTests.Api.Extensions;

public class ResultExtensionsTests
{
    private static async Task<(int StatusCode, ApiResponse<T>? Body)> ExecuteAsync<T>(IResult result)
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        context.RequestServices = new ServiceCollection().AddLogging().BuildServiceProvider();

        await result.ExecuteAsync(context);
        context.Response.Body.Position = 0;
        var body = context.Response.Body.Length == 0
            ? null
            : await System.Text.Json.JsonSerializer.DeserializeAsync<ApiResponse<T>>(context.Response.Body,
                new System.Text.Json.JsonSerializerOptions(System.Text.Json.JsonSerializerDefaults.Web));
        return (context.Response.StatusCode, body);
    }

    [Theory]
    [InlineData(ResultSuccessType.Ok, StatusCodes.Status200OK)]
    [InlineData(ResultSuccessType.Created, StatusCodes.Status201Created)]
    [InlineData(ResultSuccessType.Accepted, StatusCodes.Status202Accepted)]
    public async Task ToMinimalApiResult_Success_ReturnsMatchingStatusWithEnvelope(ResultSuccessType type, int expected)
    {
        var result = Result<string>.Success("payload", type, 
            new ResultSuccess("DONE", "Done")).ToMinimalApiResult();

        var (status, body) = await ExecuteAsync<string>(result);
        Assert.Equal(expected, status);
        Assert.NotNull(body);
        Assert.Equal("payload", body.Data);
        Assert.Equal("DONE", body.Code);
        Assert.Equal("Done", body.Message);
    }

    [Fact]
    public async Task ToMinimalApiResult_NoContent_Returns204WithoutBody()
    {
        var result = Result<bool>.Success(true, ResultSuccessType.NoContent).ToMinimalApiResult();

        var (status, body) = await ExecuteAsync<bool>(result);
        Assert.Equal(StatusCodes.Status204NoContent, status);
        Assert.Null(body);
    }

    [Theory]
    [InlineData(ResultErrorType.NotFound, StatusCodes.Status404NotFound)]
    [InlineData(ResultErrorType.Conflict, StatusCodes.Status409Conflict)]
    [InlineData(ResultErrorType.BusinessRule, StatusCodes.Status400BadRequest)]
    [InlineData(ResultErrorType.Unauthorized, StatusCodes.Status401Unauthorized)]
    [InlineData(ResultErrorType.Validation, StatusCodes.Status422UnprocessableEntity)]
    public async Task ToMinimalApiResult_Failure_ReturnsMatchingStatusWithError(ResultErrorType type, int expected)
    {
        var result = Result<string>.Failure("SOME_CODE", "Some message", type).ToMinimalApiResult();

        var (status, body) = await ExecuteAsync<object>(result);
        Assert.Equal(expected, status);
        Assert.NotNull(body);
        Assert.Equal("SOME_CODE", body.Code);
        Assert.Equal("Some message", body.Message);
    }

    [Fact]
    public async Task ToMinimalApiResult_Forbidden_Returns403()
    {
        var result = Result<string>.Failure("NO", "No", ResultErrorType.Forbidden)
            .ToMinimalApiResult();

        Assert.Equal(StatusCodes.Status403Forbidden, (await ExecuteAsync<object>(result)).StatusCode);
    }

    [Fact]
    public async Task ToMinimalApiResult_FieldErrors_Returns422WithFields()
    {
        var fields = new Dictionary<string, List<string>> { ["General"] = ["Password too weak"] };

        var result = Result<string>.Failure(fields).ToMinimalApiResult();

        var (status, body) = await ExecuteAsync<object>(result);
        Assert.Equal(StatusCodes.Status422UnprocessableEntity, status);
        Assert.NotNull(body);
        Assert.Equal(ApiResponseMessages.Validation.ValidationError.Code, body.Code);
        Assert.Equal("Password too weak", body.Errors!.Fields["General"].Single());
    }
}
