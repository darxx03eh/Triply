namespace Triply.Domain.Exceptions;

/// <summary>Thrown when the request invalid format.</summary>
public class InvalidFormatException : Exception
{
    /// <summary>Gets the code.</summary>
    public string Code { get; }
    /// <summary>Initializes a new instance of the invalid format exception.</summary>
    public InvalidFormatException(string message, string code) 
        : base(message) => Code = code;
}