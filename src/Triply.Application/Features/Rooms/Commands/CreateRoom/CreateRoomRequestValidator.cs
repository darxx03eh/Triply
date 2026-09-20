using FluentValidation;
using Triply.Application.Interfaces.Repositories;
using Triply.Domain.Results;

namespace Triply.Application.Features.Rooms.Commands.CreateRoom;

/// <summary>Validates the create room request.</summary>
public class CreateRoomRequestValidator : AbstractValidator<CreateRoomRequest>
{
    /// <summary>Initializes a new instance of the create room request validator.</summary>
    public CreateRoomRequestValidator(IRoomRepository roomRepository, IHotelRepository hotelRepository)
    {
        ApplyValidationRules();
        ApplyCustomValidationRules(roomRepository, hotelRepository);
    }

    private void ApplyValidationRules()
    {
        RuleFor(x => x.HotelId)
            .NotEmpty().WithMessage(ResultResponseMessages.Rooms.Validation.HotelIdRequired.Message);

        RuleFor(x => x.Number)
            .NotEmpty().WithMessage(ResultResponseMessages.Rooms.Validation.NumberRequired.Message)
            .Must(number => !string.IsNullOrWhiteSpace(number))
            .WithMessage(ResultResponseMessages.Rooms.Validation.NumberWhitespace.Message)
            .MaximumLength(20).WithMessage(ResultResponseMessages.Rooms.Validation.NumberMaxLength.Message);

        RuleFor(x => x.RoomType)
            .IsInEnum().WithMessage(ResultResponseMessages.Rooms.Validation.RoomTypeInvalid.Message);

        RuleFor(x => x.AdultCapacity)
            .InclusiveBetween((short)1, (short)10)
            .WithMessage(ResultResponseMessages.Rooms.Validation.AdultCapacityInvalid.Message);

        RuleFor(x => x.ChildCapacity)
            .InclusiveBetween((short)0, (short)10)
            .WithMessage(ResultResponseMessages.Rooms.Validation.ChildCapacityInvalid.Message);

        RuleFor(x => x.PricePerNight)
            .GreaterThan(0).WithMessage(ResultResponseMessages.Rooms.Validation.PricePerNightInvalid.Message)
            .PrecisionScale(10, 2, true)
            .WithMessage(ResultResponseMessages.Rooms.Validation.PricePerNightPrecision.Message);

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage(ResultResponseMessages.Rooms.Validation.DescriptionMaxLength.Message)
            .When(x => x.Description is not null);
    }

    private void ApplyCustomValidationRules(IRoomRepository roomRepository, IHotelRepository hotelRepository)
    {
        RuleFor(x => x.HotelId)
            .MustAsync(async (hotelId, cancellationToken) =>
            {
                return await hotelRepository.IsHotelIdExistsAsync(hotelId, cancellationToken);
            }).WithMessage(ResultResponseMessages.Rooms.Validation.HotelNotFound.Message)
            .When(x => x.HotelId != Guid.Empty);

        RuleFor(x => x)
            .MustAsync(async (room, cancellationToken) =>
            {
                return !await roomRepository.IsRoomNumberExistsAsync(room.HotelId, room.Number, cancellationToken);
            }).WithMessage(ResultResponseMessages.Rooms.Validation.RoomAlreadyExists.Message)
            .OverridePropertyName(nameof(CreateRoomRequest.Number))
            .When(x => !string.IsNullOrWhiteSpace(x.Number));
    }
}
