using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace Triply.Tests.UnitTests.Common.Fakes;

public static class FakeValidators
{
    public static IValidator<T>[] Passing<T>() => [Create<T>()];

    public static IValidator<T>[] Failing<T>(string property = "Field", string message = "Invalid")
        => [Create<T>(new ValidationFailure(property, message))];

    private static IValidator<T> Create<T>(params ValidationFailure[] failures)
    {
        var validator = new Mock<IValidator<T>>();
        validator.Setup(v => v.ValidateAsync(It.IsAny<IValidationContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));
        return validator.Object;
    }
}
