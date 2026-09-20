using FluentValidation;
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
        if (!validators.Any()) return;
        var context = new ValidationContext<TRequest>(request);
        var results = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        var failures = results.SelectMany(r => r.Errors).Where(e => e is not null).ToList();

        if (failures.Count != 0)
            throw new UnprocessableEntityException(failures);
    }
}