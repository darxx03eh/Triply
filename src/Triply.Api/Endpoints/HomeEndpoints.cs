using Triply.Api.Extensions;
using Triply.Application.Interfaces.Services;
using Triply.Infrastructure.Routes;

namespace Triply.Api.Endpoints;

public static class HomeEndpoints
{
    extension(IEndpointRouteBuilder app)
    {
        public void MapHomeEndpoints()
        {
            var group = app.MapGroup("")
                .WithTags("Home");

            group.MapGet(Router.DealRoutes.Featured, async (
                    int? count, IHomeService homeService, CancellationToken cancellationToken) =>
                {
                    var result = await homeService.GetFeaturedDealsAsync(count, cancellationToken);
                    return result.ToMinimalApiResult();
                })
                .WithName("GetFeaturedDeals")
                .WithDisplayName("Get Featured Deals")
                .WithSummary("Retrieves the featured deals for the home page")
                .WithDescription("""
                                 Retrieves the running featured deals (default 5, max 20) with the hotel
                                 thumbnail, location, star rating, original price and discounted price.
                                 """)
                .Produces(StatusCodes.Status200OK);

            group.MapGet(Router.CityRoutes.Trending, async (
                    int? count, IHomeService homeService, CancellationToken cancellationToken) =>
                {
                    var result = await homeService.GetTrendingCitiesAsync(count, cancellationToken);
                    return result.ToMinimalApiResult();
                })
                .WithName("GetTrendingCities")
                .WithDisplayName("Get Trending Cities")
                .WithSummary("Retrieves the most visited cities")
                .WithDescription("""
                                 Retrieves the most visited cities in the last 30 days (default 5, max 20)
                                 with their thumbnails, used for the trending destinations section.
                                 """)
                .Produces(StatusCodes.Status200OK);

            group.MapGet(Router.UserRoutes.RecentHotels, async (
                    int? count, ICurrentUserAccessor user, IHomeService homeService,
                    CancellationToken cancellationToken) =>
                {
                    var result = await homeService.GetRecentHotelsAsync(user.UserId, count, cancellationToken);
                    return result.ToMinimalApiResult();
                }).RequireAuthorization()
                .WithName("GetRecentHotels")
                .WithDisplayName("Get Recently Visited Hotels")
                .WithSummary("Retrieves the hotels the user visited recently")
                .WithDescription("""
                                 Retrieves the last hotels the signed-in user opened (default 5, max 20)
                                 with the thumbnail, city, star rating and the lowest room price.
                                 """)
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status401Unauthorized);
        }
    }
}