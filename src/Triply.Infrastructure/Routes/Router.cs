namespace Triply.Infrastructure.Routes;

/// <summary>Defines route constants used by the API.</summary>
public static class Router
{
    private const string Root = "api";
    private const string Version = "v1";
    private const string Rule = $"{Root}/{Version}";
    private const string Id = "{id:guid}";
    /// <summary>Defines authentication route paths.</summary>
    public static class AuthenticationRoutes
    {
        private const string Prefix = $"{Rule}/auth";
        /// <summary>Route for user registration.</summary>
        public const string Register = $"{Prefix}/register";
        /// <summary>Route for user login.</summary>
        public const string Login = $"{Prefix}/login";
        /// <summary>Route for user logout.</summary>
        public const string Logout = $"{Prefix}/logout";
        /// <summary>Route for access-token refresh.</summary>
        public const string Refresh = $"{Prefix}/refresh";
        /// <summary>Route for email confirmation.</summary>
        public const string EmailConfirmation = $"{Prefix}/confirm-email";
    }

    /// <summary>Represents the city routes.</summary>
    public static class CityRoutes
    {
        private const string Prefix = $"{Rule}/cities";
        /// <summary>The create.</summary>
        public const string Create = Prefix;
        /// <summary>The get all.</summary>
        public const string GetAll = Prefix;
        /// <summary>The get by identifier.</summary>
        public const string GetById = $"{Prefix}/{Id}";
        /// <summary>The update.</summary>
        public const string Update = $"{Prefix}/{Id}";
        /// <summary>The delete.</summary>
        public const string Delete = $"{Prefix}/{Id}";
        /// <summary>The thumbnail.</summary>
        public const string Thumbnail = $"{Prefix}/{Id}/thumbnail";
        /// <summary>The trending.</summary>
        public const string Trending = $"{Prefix}/trending";
    }
    
    /// <summary>Represents the hotel routes.</summary>
    public static class HotelRoutes
    {
        private const string Prefix = $"{Rule}/hotels";
        /// <summary>The create.</summary>
        public const string Create = Prefix;
        /// <summary>The get all.</summary>
        public const string GetAll = Prefix;
        /// <summary>The get by identifier.</summary>
        public const string GetById = $"{Prefix}/{Id}";
        /// <summary>The update.</summary>
        public const string Update = $"{Prefix}/{Id}";
        /// <summary>The delete.</summary>
        public const string Delete = $"{Prefix}/{Id}";
        /// <summary>The add image.</summary>
        public const string AddImage = $"{Prefix}/{Id}/images";
        /// <summary>The get images.</summary>
        public const string GetImages = $"{Prefix}/{Id}/images";
        /// <summary>The delete image.</summary>
        public const string DeleteImage = $"{Prefix}/{Id}/images/{{imageId:guid}}";
        /// <summary>The get rooms.</summary>
        public const string GetRooms = $"{Prefix}/{Id}/rooms";
        /// <summary>The amenities.</summary>
        public const string Amenities = $"{Prefix}/{Id}/amenities";
        /// <summary>The reviews.</summary>
        public const string Reviews = $"{Prefix}/{Id}/reviews";
        /// <summary>The attractions.</summary>
        public const string Attractions = $"{Prefix}/{Id}/attractions";
    }

    /// <summary>Represents the room routes.</summary>
    public static class RoomRoutes
    {
        private const string Prefix = $"{Rule}/rooms";
        /// <summary>The create.</summary>
        public const string Create = Prefix;
        /// <summary>The get all.</summary>
        public const string GetAll = Prefix;
        /// <summary>The get by identifier.</summary>
        public const string GetById = $"{Prefix}/{Id}";
        /// <summary>The update.</summary>
        public const string Update = $"{Prefix}/{Id}";
        /// <summary>The delete.</summary>
        public const string Delete = $"{Prefix}/{Id}";
    }

    /// <summary>Represents the amenity routes.</summary>
    public static class AmenityRoutes
    {
        private const string Prefix = $"{Rule}/amenities";
        /// <summary>The create.</summary>
        public const string Create = Prefix;
        /// <summary>The get all.</summary>
        public const string GetAll = Prefix;
        /// <summary>The get by identifier.</summary>
        public const string GetById = $"{Prefix}/{Id}";
        /// <summary>The update.</summary>
        public const string Update = $"{Prefix}/{Id}";
        /// <summary>The delete.</summary>
        public const string Delete = $"{Prefix}/{Id}";
    }

    /// <summary>Represents the search routes.</summary>
    public static class SearchRoutes
    {
        private const string Prefix = $"{Rule}/search";
        /// <summary>The hotels.</summary>
        public const string Hotels = Prefix;
    }

    /// <summary>Represents the review routes.</summary>
    public static class ReviewRoutes
    {
        private const string Prefix = $"{Rule}/reviews";
        /// <summary>The update.</summary>
        public const string Update = $"{Prefix}/{Id}";
        /// <summary>The delete.</summary>
        public const string Delete = $"{Prefix}/{Id}";
    }

    /// <summary>Represents the attraction routes.</summary>
    public static class AttractionRoutes
    {
        private const string Prefix = $"{Rule}/attractions";
        /// <summary>The update.</summary>
        public const string Update = $"{Prefix}/{Id}";
        /// <summary>The delete.</summary>
        public const string Delete = $"{Prefix}/{Id}";
    }

    /// <summary>Represents the deal routes.</summary>
    public static class DealRoutes
    {
        private const string Prefix = $"{Rule}/deals";
        /// <summary>The create.</summary>
        public const string Create = Prefix;
        /// <summary>The get all.</summary>
        public const string GetAll = Prefix;
        /// <summary>The featured.</summary>
        public const string Featured = $"{Prefix}/featured";
        /// <summary>The get by identifier.</summary>
        public const string GetById = $"{Prefix}/{Id}";
        /// <summary>The update.</summary>
        public const string Update = $"{Prefix}/{Id}";
        /// <summary>The delete.</summary>
        public const string Delete = $"{Prefix}/{Id}";
    }

    /// <summary>Represents the user routes.</summary>
    public static class UserRoutes
    {
        private const string Prefix = $"{Rule}/users/me";
        /// <summary>The recent hotels.</summary>
        public const string RecentHotels = $"{Prefix}/recent-hotels";
        /// <summary>The get bookings.</summary>
        public const string Bookings = $"{Prefix}/bookings";
    }

    /// <summary>Represents the cart routes.</summary>
    public static class CartRoutes
    {
        private const string Prefix = $"{Rule}/cart";
        /// <summary>The get cart.</summary>
        public const string Get = Prefix;
        /// <summary>The clear cart.</summary>
        public const string Clear = Prefix;
        /// <summary>The add item to cart.</summary>
        public const string AddItem = $"{Prefix}/items";
        /// <summary>The remove item from cart.</summary>
        public const string RemoveItem = $"{Prefix}/items/{Id}";
    }

    /// <summary>Represents the booking routes.</summary>
    public static class BookingRoutes
    {
        private const string Prefix = $"{Rule}/bookings";
        private const string ConfirmationNumber = "{confirmationNumber}";
        /// <summary>The checkout.</summary>
        public const string Checkout = Prefix;
        /// <summary>The get booking by it confirmation number.</summary>
        public const string GetByConfirmationNumber = $"{Prefix}/{ConfirmationNumber}";
        /// <summary>The cancel booking.</summary>
        public const string Cancel = $"{Prefix}/{ConfirmationNumber}/cancel";
    }
}
