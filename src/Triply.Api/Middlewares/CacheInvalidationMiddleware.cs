using Microsoft.AspNetCore.Http.Metadata;
using Triply.Application.Interfaces.Services;
using Triply.Infrastructure.Caching;

namespace Triply.Api.Middlewares;

/// <summary>Invalidates related cache groups after a successful write endpoint response.</summary>
public sealed class CacheInvalidationMiddleware(RequestDelegate next, ILogger<CacheInvalidationMiddleware> logger)
{
    /// <summary>Runs the middleware for the current request.</summary>
    public async Task InvokeAsync(HttpContext context, ICacheService cache)
    {
        await next(context);

        if (context.Response.StatusCode is < 200 or >= 300)
            return;

        var endpointName = context.GetEndpoint()?.Metadata.GetMetadata<IEndpointNameMetadata>()?.EndpointName;
        string[] groups = endpointName switch
        {
            "CreateCity" or "UpdateCity" or "DeleteCity" or "UploadCityThumbnail" or "DeleteCityThumbnail"
                => [CacheGroups.Cities, CacheGroups.Hotels, CacheGroups.TrendingCities, CacheGroups.HotelSearch],
            "CreateHotel" or "UpdateHotel" or "DeleteHotel"
                => [CacheGroups.Hotels, CacheGroups.Cities, CacheGroups.Rooms, 
                    CacheGroups.Deals, CacheGroups.FeaturedDeals, CacheGroups.HotelSearch],
            "UploadHotelImage" or "DeleteHotelImage"
                => [CacheGroups.Hotels, CacheGroups.HotelImages],
            "CreateAmenity" or "UpdateAmenity" or "DeleteAmenity" or "SetHotelAmenities"
                => [CacheGroups.Amenities, CacheGroups.HotelAmenities, CacheGroups.Hotels, CacheGroups.HotelSearch],
            "CreateAttraction" or "UpdateAttraction" or "DeleteAttraction"
                => [CacheGroups.Attractions],
            "CreateDeal" or "UpdateDeal" or "DeleteDeal"
                => [CacheGroups.Deals, CacheGroups.FeaturedDeals, CacheGroups.Hotels, 
                    CacheGroups.Rooms, CacheGroups.HotelSearch],
            "CreateRoom" or "UpdateRoom" or "DeleteRoom" or "UploadRoomImage" or "DeleteRoomImage"
                => [CacheGroups.Rooms, CacheGroups.Hotels, CacheGroups.Deals, 
                    CacheGroups.FeaturedDeals, CacheGroups.HotelSearch],
            "CreateReview" or "UpdateReview" or "DeleteReview"
                => [CacheGroups.Reviews, CacheGroups.Hotels],
            _ => []
        };

        foreach (var group in groups)
            await cache.InvalidateGroupAsync(group, context.RequestAborted);

        if (groups.Length > 0)
            logger.LogInformation("Invalidated cache groups {CacheGroups} after {EndpointName}", groups, endpointName);
    }
}
