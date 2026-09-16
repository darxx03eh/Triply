using Triply.Domain.Results.Enums;

namespace Triply.Domain.Results;

public class Result<TResult>
{
    public bool IsSuccess { get; set; }
    public TResult? Value { get; set; }
    public Dictionary<string, List<string>> Fields { get; set; } = [];
    public ResultError? Error { get; set; }
    public ResultSuccessType SuccessType { get; set; } = ResultSuccessType.Ok;
    public ResultSuccess? SuccessObject { get; set; }

    public static Result<TResult> Success(
        TResult value, ResultSuccessType type = ResultSuccessType.Ok, ResultSuccess? success = null)
        => new()
        {
            IsSuccess = true,
            Value = value,
            SuccessType = type,
            SuccessObject = success ?? DefaultSuccessMessages.For(type)
        };

    public static Result<TResult> Failure(Dictionary<string, List<string>> fields)
        => new() { IsSuccess = false, Fields = fields };

    public static Result<TResult> Failure(ResultError error)
        => new() { IsSuccess = false, Error = error };

    public static Result<TResult> Failure(
        string code, string message, ResultErrorType type = ResultErrorType.BusinessRule)
        => new() { IsSuccess = false, Error = new ResultError(code, message, type) };
}