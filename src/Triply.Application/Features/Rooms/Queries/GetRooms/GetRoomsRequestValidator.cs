using FluentValidation;
using Triply.Domain.Results;

namespace Triply.Application.Features.Rooms.Queries.GetRooms;

public class GetRoomsRequestValidator : AbstractValidator<GetRoomsRequest>
{
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
    }
}
