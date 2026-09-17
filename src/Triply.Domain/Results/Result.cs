using Triply.Domain.Results.Enums;

namespace Triply.Domain.Results;

/// <summary>Represents either a successful operation or a structured failure.</summary>
/// <typeparam name="TResult">The type of the successful result value.</typeparam>
public class Result<TResult>
{
    /// <summary>Gets or sets whether the operation succeeded.</summary>
    public bool IsSuccess { get; set; }
    /// <summary>Gets or sets the successful operation value.</summary>
    public TResult? Value { get; set; }
    /// <summary>Gets field-level validation messages.</summary>
    public Dictionary<string, List<string>> Fields { get; set; } = [];
    /// <summary>Gets the operation error when the result is unsuccessful.</summary>
    public ResultError? Error { get; set; }
    /// <summary>Gets the category of the successful response.</summary>
    public ResultSuccessType SuccessType { get; set; } = ResultSuccessType.Ok;
    /// <summary>Gets the message metadata for a successful response.</summary>
    public ResultSuccess? SuccessObject { get; set; }

    /// <summary>Creates a successful result.</summary>
    /// <param name="value">The result value.</param>
    /// <param name="type">The response category.</param>
    /// <param name="success">Optional response metadata.</param>
    /// <returns>A successful result.</returns>
    public static Result<TResult> Success(
        TResult value, ResultSuccessType type = ResultSuccessType.Ok, ResultSuccess? success = null)
        => new()
        {
            IsSuccess = true,
            Value = value,
            SuccessType = type,
            SuccessObject = success ?? DefaultSuccessMessages.For(type)
        };

    /// <summary>Creates a validation failure result.</summary>
    /// <param name="fields">Field-level validation messages.</param>
    /// <returns>A failed result.</returns>
    public static Result<TResult> Failure(Dictionary<string, List<string>> fields)
        => new() { IsSuccess = false, Fields = fields };

    /// <summary>Creates a failed result from an existing error.</summary>
    /// <param name="error">The operation error.</param>
    /// <returns>A failed result.</returns>
    public static Result<TResult> Failure(ResultError error)
        => new() { IsSuccess = false, Error = error };

    /// <summary>Creates a failed result from an error code and message.</summary>
    /// <param name="code">The stable error code.</param>
    /// <param name="message">The user-facing error message.</param>
    /// <param name="type">The category of failure.</param>
    /// <returns>A failed result.</returns>
    public static Result<TResult> Failure(
        string code, string message, ResultErrorType type = ResultErrorType.BusinessRule)
        => new() { IsSuccess = false, Error = new ResultError(code, message, type) };
}