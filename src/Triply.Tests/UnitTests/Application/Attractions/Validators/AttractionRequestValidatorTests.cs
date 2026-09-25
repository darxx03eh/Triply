using FluentValidation.TestHelper;
using Moq;
using Triply.Application.Features.Attractions.Commands.CreateAttraction;
using Triply.Application.Features.Attractions.Commands.UpdateAttraction;
using Triply.Application.Interfaces.Repositories;
using Triply.Domain.Results;

namespace Triply.Tests.UnitTests.Application.Attractions.Validators;

public class AttractionRequestValidatorTests
{
    [Fact]
    public async Task Validate_CreateAttractionForExistingHotel_HasNoErrors()
    {
        var hotels = new Mock<IHotelRepository>();
        hotels.Setup(x => x.IsHotelIdExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var request = new CreateAttractionRequest
        { HotelId = Guid.NewGuid(), Name = "Roman Theatre", Category = "Historic", DistanceKm = 2.5m };

        var result = await new CreateAttractionRequestValidator(hotels.Object).TestValidateAsync(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_UpdateAttractionWithInvalidDistance_HasDistanceError()
    {
        var result = new UpdateAttractionRequestValidator().TestValidate(new UpdateAttractionRequest
        { Name = "Roman Theatre", Category = "Historic", DistanceKm = 100.01m });

        result.ShouldHaveValidationErrorFor(x => x.DistanceKm)
            .WithErrorMessage(ResultResponseMessages.Attractions.Validation.DistanceInvalid.Message);
    }
}
