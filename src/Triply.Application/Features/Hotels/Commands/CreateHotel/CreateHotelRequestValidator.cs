using System.Data;
using FluentValidation;
using Triply.Application.Interfaces.Repositories;
using Triply.Domain.Results;

namespace Triply.Application.Features.Hotels.Commands.CreateHotel;

public class CreateHotelRequestValidator : AbstractValidator<CreateHotelRequest>
{
    public CreateHotelRequestValidator(IHotelRepository hotleRepository, ICityRepository cityRepository)
    {
        ApplyValidationRules();
        ApplyCustomValidationRules(hotleRepository, cityRepository);
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
    }
    private void ApplyCustomValidationRules(IHotelRepository hotelRepository, ICityRepository cityRepository)
    {
        RuleFor(x => x)
            .MustAsync(async (hotel, cancellationToken) =>
            {
                return !await hotelRepository.IsHotelExistsAsync(hotel.Name, hotel.CityId, cancellationToken);
            }).WithMessage(ResultResponseMessages.Hotels.Validation.HotelAlreadyExists.Message)
            .OverridePropertyName(nameof(CreateHotelRequest.Name));

        RuleFor(x => x)
            .MustAsync(async (hotel, cancellationToken) =>
            {
                return !await hotelRepository.IsLocationsExistsAsync(hotel.Latitude, hotel.Longitude,
                    cancellationToken);
            }).WithMessage(ResultResponseMessages.Hotels.Validation.LocationAlreadyExists.Message)
            .OverridePropertyName(nameof(CreateHotelRequest.Latitude))
            .When(x => x.Latitude.HasValue && x.Longitude.HasValue);

        RuleFor(x => x.CityId)
            .MustAsync(async (cityId, cancellationToken) =>
            {
                return await cityRepository.IsCityIdExistsAsync(cityId, cancellationToken);
            }).WithMessage(ResultResponseMessages.Cities.Validation.CityNotFound.Message);
    }
}