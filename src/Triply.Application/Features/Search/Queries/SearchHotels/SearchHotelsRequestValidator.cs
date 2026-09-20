using FluentValidation;
using Triply.Application.Extensions;
using Triply.Domain.Enums.Hotels;
using Triply.Domain.Results;

namespace Triply.Application.Features.Search.Queries.SearchHotels;

/// <summary>Validates the search hotels request.</summary>
public class SearchHotelsRequestValidator : AbstractValidator<SearchHotelsRequest>
{
    private const int MaxNights = 30;

    /// <summary>Initializes a new instance of the search hotels request validator.</summary>
    public SearchHotelsRequestValidator() => ApplyValidationRules();

    private void ApplyValidationRules()
    {
        RuleFor(x => x.Q)
            .MaximumLength(100).WithMessage(ResultResponseMessages.Search.Validation.QueryMaxLength.Message)
            .When(x => x.Q is not null);

        RuleFor(x => x.CheckIn)
            .Must(checkIn => checkIn >= SearchExtensions.Today())
            .WithMessage(ResultResponseMessages.Search.Validation.CheckInInPast.Message)
            .When(x => x.CheckIn.HasValue);

        RuleFor(x => x.CheckOut)
            .Must((request, _) => request.GetCheckOut() > request.GetCheckIn())
            .WithMessage(ResultResponseMessages.Search.Validation.CheckOutBeforeCheckIn.Message)
            .Must((request, _) => request.GetCheckOut().DayNumber - request.GetCheckIn().DayNumber <= MaxNights)
            .WithMessage(ResultResponseMessages.Search.Validation.StayTooLong.Message)
            .When(x => x.CheckOut.HasValue);

        RuleFor(x => x.Adults)
            .InclusiveBetween(1, 20).WithMessage(ResultResponseMessages.Search.Validation.AdultsInvalid.Message)
            .When(x => x.Adults.HasValue);

        RuleFor(x => x.Children)
            .InclusiveBetween(0, 20).WithMessage(ResultResponseMessages.Search.Validation.ChildrenInvalid.Message)
            .When(x => x.Children.HasValue);

        RuleFor(x => x.Rooms)
            .InclusiveBetween(1, 10).WithMessage(ResultResponseMessages.Search.Validation.RoomsInvalid.Message)
            .Must((request, rooms) => rooms <= request.GetAdults())
            .WithMessage(ResultResponseMessages.Search.Validation.RoomsMoreThanAdults.Message)
            .When(x => x.Rooms.HasValue);

        RuleFor(x => x.MinPrice)
            .GreaterThanOrEqualTo(0).WithMessage(ResultResponseMessages.Search.Validation.PriceInvalid.Message)
            .When(x => x.MinPrice.HasValue);

        RuleFor(x => x.MaxPrice)
            .GreaterThanOrEqualTo(0).WithMessage(ResultResponseMessages.Search.Validation.PriceInvalid.Message)
            .GreaterThanOrEqualTo(x => x.MinPrice ?? 0)
            .WithMessage(ResultResponseMessages.Search.Validation.PriceRangeInvalid.Message)
            .When(x => x.MaxPrice.HasValue);

        RuleForEach(x => x.Stars)
            .InclusiveBetween(1, 5).WithMessage(ResultResponseMessages.Search.Validation.StarsInvalid.Message);

        RuleForEach(x => x.Types)
            .Must(type => Enum.TryParse<HotelType>(type, ignoreCase: true, out _) && !int.TryParse(type, out _))
            .WithMessage(ResultResponseMessages.Search.Validation.HotelTypeInvalid.Message);

        RuleFor(x => x.Amenities)
            .Must(amenities => amenities!.Length <= 20)
            .WithMessage(ResultResponseMessages.Search.Validation.AmenitiesTooMany.Message)
            .When(x => x.Amenities is not null);

        RuleFor(x => x.Sort)
            .Must(sort => SearchSorts.All.Contains(sort!))
            .WithMessage(ResultResponseMessages.Search.Validation.SortInvalid.Message)
            .When(x => !string.IsNullOrWhiteSpace(x.Sort));

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage(ResultResponseMessages.Search.Validation.PageInvalid.Message)
            .When(x => x.Page.HasValue);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 50).WithMessage(ResultResponseMessages.Search.Validation.PageSizeInvalid.Message)
            .When(x => x.PageSize.HasValue);
    }
}
