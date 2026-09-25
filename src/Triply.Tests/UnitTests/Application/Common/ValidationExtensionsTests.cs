using FluentValidation;
using Triply.Application.Exceptions;
using Triply.Application.Extensions;

namespace Triply.Tests.UnitTests.Application.Common;

public class ValidationExtensionsTests
{
    private sealed record Sample(string Name, int Age);

    private sealed class NameValidator : AbstractValidator<Sample>
    {
        public NameValidator() => RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
    }

    private sealed class AgeValidator : AbstractValidator<Sample>
    {
        public AgeValidator() => RuleFor(x => x.Age).GreaterThan(0).WithMessage("Age must be positive.");
    }

    [Fact]
    public async Task ValidateAndThrowAsync_NoValidators_DoesNotThrow()
    {
        await Enumerable.Empty<IValidator<Sample>>().ValidateAndThrowAsync(new Sample("", 0), 
            CancellationToken.None);
    }

    [Fact]
    public async Task ValidateAndThrowAsync_ValidRequest_DoesNotThrow()
    {
        IValidator<Sample>[] validators = [new NameValidator(), new AgeValidator()];

        await validators.ValidateAndThrowAsync(new Sample("Mahmoud", 23), CancellationToken.None);
    }

    [Fact]
    public async Task ValidateAndThrowAsync_Invalid_ThrowsWithFailuresFromAllValidators()
    {
        IValidator<Sample>[] validators = [new NameValidator(), new AgeValidator()];

        var exception = await Assert.ThrowsAsync<UnprocessableEntityException>(
            () => validators.ValidateAndThrowAsync(new Sample("", 0), CancellationToken.None));

        Assert.Equal("Name is required.", exception.Errors["Name"].Single());
        Assert.Equal("Age must be positive.", exception.Errors["Age"].Single());
    }
}
