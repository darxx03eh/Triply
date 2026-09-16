using Triply.Domain.Results.Enums;

namespace Triply.Domain.Results;

public sealed record ResultError(string Code, string Message, ResultErrorType Type = ResultErrorType.BusinessRule);