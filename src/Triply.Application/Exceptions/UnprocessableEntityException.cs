using FluentValidation.Results;

namespace Triply.Application.Exceptions;

/// <summary>Thrown when the request unprocessable entity.</summary>
public class UnprocessableEntityException : Exception
{
    /// <summary>Initializes a new instance of the unprocessable entity exception.</summary>
    public UnprocessableEntityException(IEnumerable<ValidationFailure> failures)
    {
        Errors = failures.GroupBy(fail => fail.PropertyName)
            .ToDictionary(map => map.Key, map => 
                map.Select(fail => fail.ErrorMessage).ToList());
    }
    /// <summary>Gets or sets the errors.</summary>
    public Dictionary<string, List<string>> Errors { get; set; }
}