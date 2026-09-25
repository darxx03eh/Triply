using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Tests.UnitTests.Domain.Results;

public class ResultTests
{
    [Fact]
    public void Success_WithValue_SetsValueAndDefaultOkMessage()
    {
        var result = Result<int>.Success(42);

        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.Value);
        Assert.Equal(ResultSuccessType.Ok, result.SuccessType);
        Assert.Equal(ResultResponseMessages.Success.Ok.Code, result.SuccessObject!.Code);
        Assert.Null(result.Error);
    }

    [Theory]
    [InlineData(ResultSuccessType.Created, "CREATED")]
    [InlineData(ResultSuccessType.NoContent, "DELETED")]
    [InlineData(ResultSuccessType.Ok, "SUCCESS")]
    [InlineData(ResultSuccessType.Accepted, "SUCCESS")]
    public void Success_WithoutMessage_UsesDefaultMessageForType(ResultSuccessType type, string expectedCode)
    {
        var result = Result<string>.Success("value", type);

        Assert.Equal(type, result.SuccessType);
        Assert.Equal(expectedCode, result.SuccessObject!.Code);
    }

    [Fact]
    public void Success_WithCustomMessage_KeepsCustomMessage()
    {
        var result = Result<string>.Success("value", ResultSuccessType.Created, new ResultSuccess("CUSTOM", "Custom"));

        Assert.Equal("CUSTOM", result.SuccessObject!.Code);
        Assert.Equal("Custom", result.SuccessObject.Message);
    }

    [Fact]
    public void Failure_WithCodeAndMessage_DefaultsToBusinessRule()
    {
        var result = Result<string>.Failure("CODE", "Message");

        Assert.False(result.IsSuccess);
        Assert.Equal(new ResultError("CODE", "Message", ResultErrorType.BusinessRule), result.Error);
        Assert.Null(result.Value);
    }

    [Theory]
    [InlineData(ResultErrorType.NotFound)]
    [InlineData(ResultErrorType.Conflict)]
    [InlineData(ResultErrorType.Unauthorized)]
    [InlineData(ResultErrorType.Forbidden)]
    [InlineData(ResultErrorType.Validation)]
    public void Failure_WithType_KeepsType(ResultErrorType type)
    {
        var result = Result<string>.Failure("CODE", "Message", type);

        Assert.Equal(type, result.Error!.Type);
    }

    [Fact]
    public void Failure_WithResultError_KeepsError()
    {
        var error = new ResultError("CODE", "Message", ResultErrorType.Conflict);

        var result = Result<int>.Failure(error);

        Assert.False(result.IsSuccess);
        Assert.Same(error, result.Error);
    }

    [Fact]
    public void Failure_WithFields_KeepsFieldsAndHasNoError()
    {
        var fields = new Dictionary<string, List<string>> { ["General"] = ["Password is too weak"] };

        var result = Result<string>.Failure(fields);

        Assert.False(result.IsSuccess);
        Assert.Null(result.Error);
        Assert.Equal("Password is too weak", result.Fields["General"].Single());
    }
}
