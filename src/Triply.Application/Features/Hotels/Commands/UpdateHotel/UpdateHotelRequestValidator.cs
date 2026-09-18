using FluentValidation;
using Triply.Application.Interfaces.Repositories;
using Triply.Domain.Results;

namespace Triply.Application.Features.Hotels.Commands.UpdateHotel;

public class UpdateHotelRequestValidator : AbstractValidator<UpdateHotelRequest>
{
    public UpdateHotelRequestValidator(IHotelRepository hotelRepository, ICityRepository cityRepository)
    {
        ApplyValidationRules();
        ApplyCustomValidationRules(hotelRepository, cityRepository);
    }

    private void ApplyValidationRules()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ResultResponseMessages.Hotels.Validation.NameRequired.Message)
            .MaximumLength(150).WithMessage(ResultResponseMessages.Hotels.Validation.NameMaxLength.Message);
        
        RuleFor(x => x.CityId)
            .NotEmpty().WithMessage(ResultResponseMessages.Hotels.Validation.CityIdRequired.Message);

        RuleFor(x => x.StarRating)
            .InclusiveBetween((byte)1, (byte)5)
            .WithMessage(ResultResponseMessages.Hotels.Validation.StarRatingInvalid.Message);

        RuleFor(x => x.HotelType)
            .IsInEnum().WithMessage(ResultResponseMessages.Hotels.Validation.HotelTypeInvalid.Message);

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage(ResultResponseMessages.Hotels.Validation.AddressRequired.Message)
            .MaximumLength(300).WithMessage(ResultResponseMessages.Hotels.Validation.AddressMaxLength.Message);
        
        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage(ResultResponseMessages.Hotels.Validation.DescriptionMaxLength.Message)
            .When(x => x.Description is not null);
        
        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90).WithMessage(ResultResponseMessages.Hotels.Validation.LatitudeInvalid.Message)
            .When(x => x.Latitude.HasValue);
        
        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180).WithMessage(ResultResponseMessages.Hotels.Validation.LongitudeInvalid.Message)
            .When(x => x.Longitude.HasValue);

        RuleFor(x => x.RowVersion)
            .NotEmpty().WithMessage(ResultResponseMessages.Hotels.Validation.RowVersionRequired.Message)
            .Must(rowVersion => rowVersion.Length == 8)
            .WithMessage(ResultResponseMessages.Hotels.Validation.RowVersionInvalidLength.Message)
            .When(x => x.RowVersion is { Length: > 0 });
    }

    private void ApplyCustomValidationRules(IHotelRepository hotelRepository, ICityRepository cityRepository)
    {
        RuleFor(x => x)
            .MustAsync(async (hotel, cancellationToken) =>
            {
                return !await hotelRepository.IsHotelExistsExcludeId(hotel.Name, hotel.CityId, hotel.HotelId,
                    cancellationToken);
            }).WithMessage(ResultResponseMessages.Hotels.Validation.HotelAlreadyExists.Message)
            .OverridePropertyName(nameof(UpdateHotelRequest.Name));

        RuleFor(x => x)
            .MustAsync(async (hotel, cancellationToken) =>
            {
                return !await hotelRepository.IsLocationExistsExcludeIdAsync(
                    hotel.Latitude, hotel.Longitude, hotel.HotelId, cancellationToken);
            }).WithMessage(ResultResponseMessages.Hotels.Validation.LocationAlreadyExists.Message)
            .OverridePropertyName(nameof(UpdateHotelRequest.Latitude))
            .When(x => x.Latitude.HasValue && x.Longitude.HasValue);

        RuleFor(x => x.CityId)
            .MustAsync(async (cityId, cancellationToken) =>
            {
                return await cityRepository.IsCityIdExistsAsync(cityId, cancellationToken);
            }).WithMessage(ResultResponseMessages.Cities.Validation.CityNotFound.Message);
    }
}