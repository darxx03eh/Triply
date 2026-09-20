namespace Triply.Domain.Results;

/// <summary>Gets or sets the result message.</summary>
/// <summary>Gets or sets the code.</summary>
/// <summary>Gets or sets the message.</summary>
/// <summary>Message payload published for the result.</summary>
public sealed record ResultMessage(string Code, string Message);