namespace Triply.Domain.Exceptions;

/// <summary>Thrown when the request not found.</summary>
public class NotFoundException : Exception
{
    /// <summary>Gets the code.</summary>
    public string Code { get; }
    /// <summary>Initializes a new instance of the not found exception.</summary>
    public NotFoundException(string message, string code) 
        : base(message) => Code = code;
}