using Triply.Domain.Results.Enums;

namespace Triply.Domain.Results;

/// <summary>Represents the default success messages.</summary>
public static class DefaultSuccessMessages
{
    /// <summary>Fors.</summary>
    public static ResultSuccess For(ResultSuccessType type) => type switch
    {
        ResultSuccessType.Created => new ResultSuccess(
            ResultResponseMessages.Success.Created.Code, 
            ResultResponseMessages.Success.Created.Message),
        ResultSuccessType.NoContent => new ResultSuccess(
            ResultResponseMessages.Success.NoContent.Code,
            ResultResponseMessages.Success.NoContent.Message),
        _ => new ResultSuccess(
            ResultResponseMessages.Success.Ok.Code,
            ResultResponseMessages.Success.Ok.Message)
    };
}