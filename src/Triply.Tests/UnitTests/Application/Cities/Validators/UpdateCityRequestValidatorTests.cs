using FluentValidation.TestHelper;
using Moq;
using Triply.Application.Features.Cities.Commands.UpdateCity;
using Triply.Application.Interfaces.Repositories;
using Triply.Domain.Results;
using Triply.Tests.UnitTests.Common.Builders;

namespace Triply.Tests.UnitTests.Application.Cities.Validators;

public class UpdateCityRequestValidatorTests
{
    private readonly Mock<ICityRepository> _cityRepository = new();
    private readonly UpdateCityRequestValidator _validator;
    private readonly Guid _cityId = Guid.NewGuid();

    public UpdateCityRequestValidatorTests()
    {
        _cityRepository.Setup(r => r.IsCityExistsExcludeId(It.IsAny<string>(), 
                It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _validator = new UpdateCityRequestValidator(_cityRepository.Object);
    }

    private UpdateCityRequest ValidRequest() => new()
    {
        CityId = _cityId,
        Name = "Nablus",
        Country = "Palestine",
        PostOffice = "P400",
        RowVersion = TestData.RowVersion
    };

    [Fact]
    public async Task Validate_ValidRequest_HasNoErrors()
    {
        var result = await _validator.TestValidateAsync(ValidRequest());

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_DuplicateNameInCountry_HasErrorOnName()
    {
        _cityRepository.Setup(r => r.IsCityExistsExcludeId("Nablus", 
                "Palestine", _cityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _validator.TestValidateAsync(ValidRequest());

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage(ResultResponseMessages.Cities.Validation.CityAlreadyExistsInThisCountry.Message);
    }

    [Fact]
    public async Task Validate_DuplicateCheck_ExcludesTheCityBeingUpdated()
    {
        await _validator.TestValidateAsync(ValidRequest());

        _cityRepository.Verify(r => r.IsCityExistsExcludeId("Nablus", 
            "Palestine", _cityId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Validate_MissingRowVersion_HasError()
    {
        var request = ValidRequest();
        request.RowVersion = [];

        var result = await _validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(x => x.RowVersion)
            .WithErrorMessage(ResultResponseMessages.Cities.Validation.RowVersionRequired.Message);
    }

    [Fact]
    public async Task Validate_RowVersionWrongLength_HasError()
    {
        var request = ValidRequest();
        request.RowVersion = [1, 2];

        var result = await _validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(x => x.RowVersion)
            .WithErrorMessage(ResultResponseMessages.Cities.Validation.RowVersionInvalidLength.Message);
    }

    [Theory]
    [InlineData("", "Palestine")]
    [InlineData("Nablus", "")]
    [InlineData("   ", "Palestine")]
    public async Task Validate_MissingNameOrCountry_HasErrors(string name, string country)
    {
        var request = ValidRequest();
        request.Name = name;
        request.Country = country;

        var result = await _validator.TestValidateAsync(request);

        Assert.False(result.IsValid);
    }
}
