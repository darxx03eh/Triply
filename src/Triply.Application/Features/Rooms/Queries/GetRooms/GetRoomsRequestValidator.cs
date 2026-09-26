using FluentValidation;
using Triply.Domain.Results;

namespace Triply.Application.Features.Rooms.Queries.GetRooms;

/// <summary>Validates the get rooms request.</summary>
public class GetRoomsRequestValidator : AbstractValidator<GetRoomsRequest>
{
    /// <summary>Initializes a new instance of the get rooms request validator.</summary>
    public GetRoomsRequestValidator() => ApplyValidationRules();

    private void ApplyValidationRules()
    {
        RuleFor(request => request.Page)
            .GreaterThanOrEqualTo(1)
            .When(request => request.Page.HasValue)
            .WithMessage(ResultResponseMessages.Rooms.Validation.PageInvalid.Message);

        RuleFor(request => request.PageSize)
            .InclusiveBetween(1, 50)
            .When(request => request.PageSize.HasValue)
            .WithMessage(ResultResponseMessages.Rooms.Validation.PageSizeInvalid.Message);

        RuleFor(request => request)
            .Must(request => (!request.CheckIn.HasValue && !request.CheckOut.HasValue) ||
                             (request.CheckIn.HasValue && request.CheckOut.HasValue &&
                              request.CheckOut.Value > request.CheckIn.Value))
            .WithMessage("Check-in and check-out must both be supplied, and check-out must be after check-in.")
            .OverridePropertyName(nameof(GetRoomsRequest.CheckOut));
    }
}
