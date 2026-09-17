namespace Triply.Domain.Results;

/// <summary>Describes a successful operation.</summary>
/// <param name="Code">The stable success code.</param>
/// <param name="Message">The user-facing success message.</param>
public sealed record ResultSuccess(string Code, string Message);