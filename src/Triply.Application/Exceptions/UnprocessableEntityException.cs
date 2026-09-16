using FluentValidation.Results;

namespace Triply.Application.Exceptions;

public class UnprocessableEntityException : Exception
{
    public UnprocessableEntityException(IEnumerable<ValidationFailure> failures)
    {
        Errors = failures.GroupBy(fail => fail.PropertyName)
            .ToDictionary(map => map.Key, map => 
                map.Select(fail => fail.ErrorMessage).ToList());
    }
    public Dictionary<string, List<string>> Errors { get; set; }
}