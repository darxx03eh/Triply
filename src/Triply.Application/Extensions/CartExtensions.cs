using Triply.Application.DTOs.Cart;
using Triply.Application.Features.Cart.Commands.AddCartItem;
using Triply.Domain.Entities;

namespace Triply.Application.Extensions;

/// <summary>Extension methods for cart.</summary>
public static class CartExtensions
{
    /// <summary>Gets the nights.</summary>
    public static int GetNights(this DateTime checkIn, DateTime checkOut)
        => DateOnly.FromDateTime(checkOut).DayNumber - DateOnly.FromDateTime(checkIn).DayNumber;
    
    /// <summary>Maps the cart item to a cart item response.</summary>
    public static CartItemResponse ToCartItemResponse(this CartItem item, decimal? discountPercentage,
        bool isAvailable)
    {
        var nights = item.CheckIn.GetNights(item.CheckOut);
        var originalPrice = item.Room.PricePerNight * nights;
        var totalPrice = discountPercentage.HasValue
            ? originalPrice.ApplyDiscount(discountPercentage.Value)
            : originalPrice;

        return new CartItemResponse()
        {
            CartItemId = item.CartItemId,
            RoomId = item.RoomId,
            RoomNumber = item.Room.Number,
            RoomType = item.Room.RoomType,
            HotelId = item.Room.HotelId,
            HotelName = item.Room.Hotel.Name,
            CityName = item.Room.Hotel.City.Name,
            ThumbnailUrl = item.Room.Hotel.Images.Select(i => i.Url).FirstOrDefault(),
            CheckIn = DateOnly.FromDateTime(item.CheckIn),
            CheckOut = DateOnly.FromDateTime(item.CheckOut),
            Nights = nights,
            Adults = item.Adults,
            Children = item.Children,
            PricePerNight = item.Room.PricePerNight,
            DiscountPercentage = discountPercentage,
            OriginalPrice = originalPrice,
            DiscountAmount = originalPrice - totalPrice,
            TotalPrice = totalPrice,
            IsAvailable = isAvailable
        };
    }
    
    /// <summary>Maps the list of cart items to a cart response.</summary>
    public static CartResponse ToCartResponse(this IReadOnlyList<CartItemResponse> items)
        => new CartResponse()
        {
            Items = items,
            ItemsCount = items.Count,
            OriginalPrice = items.Sum(i => i.OriginalPrice),
            DiscountAmount = items.Sum(i => i.DiscountAmount),
            TotalPrice = items.Sum(i => i.TotalPrice)
        };
    /// <summary>Maps the add cart item request to a cart item.</summary>
    public static CartItem ToCartItem(this AddCartItemRequest request, 
        DateTime checkIn, DateTime checkOut, Guid roomId)
        => new CartItem()
        {
            UserId = request.UserId,
            RoomId = roomId,
            CheckIn = checkIn,
            CheckOut = checkOut,
            Adults = request.Adults,
            Children = request.Children
        };
}