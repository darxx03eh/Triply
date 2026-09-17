using Triply.Domain.Results.Enums;

namespace Triply.Domain.Results;

/// <summary>Describes a failed operation.</summary>
/// <param name="Code">The stable error code.</param>
/// <param name="Message">The user-facing error message.</param>
/// <param name="Type">The category of failure.</param>
public sealed record ResultError(string Code, string Message, ResultErrorType Type = ResultErrorType.BusinessRule);