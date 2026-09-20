using FluentValidation;
using Triply.Application.Extensions;
using Triply.Application.Interfaces.Repositories;
using Triply.Domain.Results;

namespace Triply.Application.Features.Cart.Commands.AddCartItem;

/// <summary>Validates add item to cart requests.</summary>
public class AddCartItemRequestValidator : AbstractValidator<AddCartItemRequest>
{
    /// <summary>max nights that can customer stay in</summary>
    private const int MaxNights = 30;
    /// <summary>Initializes a new instance of the add item to cart request validator.</summary>
    public AddCartItemRequestValidator(IRoomRepository roomRepository, ICartRepository cartRepository)
    {
        ApplyValidationRules();
        ApplyCustomValidationRules(roomRepository, cartRepository);
    }

    /// <summary>Initializes the add to cart validation rules.</summary>
    private void ApplyValidationRules()
    {
        RuleFor(x => x.RoomId)
            .NotEmpty().WithMessage(ResultResponseMessages.Cart.Validation.RoomIdRequired.Message);

        RuleFor(x => x.CheckIn)
            .GreaterThanOrEqualTo(_ => SearchExtensions.Today())
            .WithMessage(ResultResponseMessages.Cart.Validation.CheckInInPast.Message);

        RuleFor(x => x.CheckOut)
            .GreaterThan(x => x.CheckIn)
            .WithMessage(ResultResponseMessages.Cart.Validation.CheckOutBeforeCheckIn.Message)
            .Must((request, checkOut) => checkOut.DayNumber - request.CheckIn.DayNumber <= MaxNights)
            .WithMessage(ResultResponseMessages.Cart.Validation.StayTooLong.Message);

        RuleFor(x => x.Adults)
            .InclusiveBetween((short)1, (short)20)
            .WithMessage(ResultResponseMessages.Cart.Validation.AdultsInvalid.Message);

        RuleFor(x => x.Children)
            .InclusiveBetween((short)0, (short)20)
            .WithMessage(ResultResponseMessages.Cart.Validation.ChildrenInvalid.Message);
    }

    private void ApplyCustomValidationRules(IRoomRepository roomRepository, ICartRepository cartRepository)
    {
        RuleFor(x => x.RoomId)
            .MustAsync(async (roomId, cancellationToken) =>
            {
                var room = await roomRepository.GetByIdAsync(roomId, cancellationToken);
                return room is not null && !room.IsDeleted;
            }).WithMessage(ResultResponseMessages.Cart.Validation.RoomNotFound.Message)
            .When(x => x.RoomId != Guid.Empty);

        RuleFor(x => x)
            .MustAsync(async (item, cancellationToken) =>
            {
                return !await cartRepository.IsItemExistsAsync(item.UserId, item.RoomId,
                    item.CheckIn.ToDateTime(TimeOnly.MinValue), item.CheckOut.ToDateTime(TimeOnly.MinValue),
                    cancellationToken);
            }).WithMessage(ResultResponseMessages.Cart.Validation.AlreadyInCart.Message)
            .OverridePropertyName(nameof(AddCartItemRequest.RoomId));
    }
}