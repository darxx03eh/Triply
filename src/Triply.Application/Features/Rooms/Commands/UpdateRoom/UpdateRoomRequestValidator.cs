using FluentValidation;
using Triply.Application.Interfaces.Repositories;
using Triply.Domain.Results;

namespace Triply.Application.Features.Rooms.Commands.UpdateRoom;

public class UpdateRoomRequestValidator : AbstractValidator<UpdateRoomRequest>
{
    public UpdateRoomRequestValidator(IRoomRepository roomRepository)
    {
        ApplyValidationRules();
        ApplyCustomValidationRules(roomRepository);
    }

    private void ApplyValidationRules()
    {
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

        RuleFor(x => x.RowVersion)
            .NotEmpty().WithMessage(ResultResponseMessages.Rooms.Validation.RowVersionRequired.Message)
            .Must(rowVersion => rowVersion.Length == 8)
            .WithMessage(ResultResponseMessages.Rooms.Validation.RowVersionInvalidLength.Message)
            .When(x => x.RowVersion is { Length: > 0 });
    }

    private void ApplyCustomValidationRules(IRoomRepository roomRepository)
    {
        RuleFor(x => x)
            .MustAsync(async (room, cancellationToken) =>
            {
                return !await roomRepository.IsRoomNumberExistsExcludeIdAsync(room.Number, room.RoomId,
                    cancellationToken);
            }).WithMessage(ResultResponseMessages.Rooms.Validation.RoomAlreadyExists.Message)
            .OverridePropertyName(nameof(UpdateRoomRequest.Number))
            .When(x => !string.IsNullOrWhiteSpace(x.Number));
    }
}
