using FluentValidation;
using FluentValidation.Results;
using Triply.Application.Exceptions;

namespace Triply.Application.Extensions;

/// <summary>Extension methods for validation.</summary>
public static class ValidationExtensions
{
    /// <summary>Validates the and throw async t request.</summary>
    public static async Task ValidateAndThrowAsync<TRequest>(
        this IEnumerable<IValidator<TRequest>> validators,
        TRequest request,
        CancellationToken cancellationToken)
    {
        var failures = new List<ValidationFailure>();
        foreach (var validator in validators)
        {
            var result = await validator.ValidateAsync(new ValidationContext<TRequest>(request), cancellationToken);
            failures.AddRange(result.Errors.Where(e => e is not null));
        }

        if (failures.Count != 0)
            throw new UnprocessableEntityException(failures);
    }
}
