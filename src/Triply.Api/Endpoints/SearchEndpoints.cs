using Triply.Api.Extensions;
using Triply.Application.Features.Search.Queries.SearchHotels;
using Triply.Application.Interfaces.Services;
using Triply.Infrastructure.Routes;
using Triply.Api.Responses;
using Triply.Application.DTOs.Search;

namespace Triply.Api.Endpoints;

/// <summary>Maps the search endpoints.</summary>
public static class SearchEndpoints
{
    extension(IEndpointRouteBuilder app)
    {
        /// <summary>Maps the search endpoints.</summary>
        public void MapSearchEndpoints()
        {
            var group = app.MapGroup("")
                .WithTags("Search");

            group.MapGet(Router.SearchRoutes.Hotels, async (
                    [AsParameters] SearchHotelsRequest request,
                    ISearchService searchService,
                    CancellationToken cancellationToken) =>
                {
                    var result = await searchService.SearchHotelsAsync(request, cancellationToken);
                    return result.ToMinimalApiResult();
                })
                .WithName("SearchHotels")
                .WithDisplayName("Search Hotels")
                .WithSummary("Searches for hotels with rooms available for the given stay")
                .WithDescription("""
                                 Searches hotels by name, city or country and returns only hotels that have
                                 enough available rooms for the requested dates, guests and number of rooms.
                                 A room is available when it is marked available, fits the guests, is within the
                                 price range and has no pending or confirmed booking overlapping the stay.
                                 Filters: stars (repeatable), types (Budget, Boutique, Luxury, repeatable),
                                 amenities (amenity ids, a hotel must offer all of them), minPrice, maxPrice.
                                 Sort: recommended, price_asc, price_desc, stars_desc, stars_asc, name.
                                 Defaults: check-in today, check-out tomorrow, 2 adults, 0 children, 1 room.
                                 Each result contains the lowest price per night and the total price of the stay.
                                 """)
                .Produces<ApiResponse<SearchHotelsResponse>>(StatusCodes.Status200OK)
                .Produces<ApiResponse<object>>(StatusCodes.Status422UnprocessableEntity);
        }
    }
}
