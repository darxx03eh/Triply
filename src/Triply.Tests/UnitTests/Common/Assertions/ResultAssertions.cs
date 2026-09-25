using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Tests.UnitTests.Common.Assertions;

public static class ResultAssertions
{
    public static T AssertSuccess<T>(this Result<T> result, ResultSuccessType type = ResultSuccessType.Ok)
    {
        Assert.True(result.IsSuccess, 
            $"Expected success but got {result.Error?.Code}: {result.Error?.Message}");
        Assert.Equal(type, result.SuccessType);
        return result.Value!;
    }

    public static ResultError AssertFailure<T>(this Result<T> result, string code, ResultErrorType type)
    {
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Equal(code, result.Error!.Code);
        Assert.Equal(type, result.Error.Type);
        return result.Error;
    }
}
