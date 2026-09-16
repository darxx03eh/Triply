namespace Triply.Domain.Exceptions;

public class InvalidFormatException : Exception
{
    public string Code { get; }
    public InvalidFormatException(string message, string code) 
        : base(message) => Code = code;
}