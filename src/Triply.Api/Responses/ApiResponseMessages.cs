namespace Triply.Api.Responses;

/// <summary>The messages of the API response messages feature.</summary>
public static class ApiResponseMessages
{
    /// <summary>The authentication messages of the API response messages.</summary>
    public static class Authentication
    {
        /// <summary>The not authenticated.</summary>
        public static readonly ApiMessage NotAuthenticated
            = new("NOT_AUTHENTICATED", "You are not authorized to perform this action.");

        /// <summary>The access denied.</summary>
        public static readonly ApiMessage AccessDenied
            = new("ACCESS_DENIED", "You do not have permission to access this resource.");

        /// <summary>The forbidden resource.</summary>
        public static readonly ApiMessage ForbiddenResource
            = new("ACCESS_DENIED", "Access to this resource is forbidden.");

        /// <summary>The token expired.</summary>
        public static readonly ApiMessage TokenExpired
            = new("TOKEN_EXPIRED", "Your session has expired. Please sign in again.");

        /// <summary>The login limited.</summary>
        public static readonly ApiMessage LoginRateLimited
            = new(
                "LOGIN_RATE_LIMITED",
                "Too many login requests. Please try again in a few minutes.");

        /// <summary>The register limited.</summary>
        public static readonly ApiMessage RegisterRateLimited
            = new(
                "REGISTER_RATE_LIMITED",
                "Too many register requests. Please try again in a few minutes.");

        /// <summary>The email confirmation limited.</summary>
        public static readonly ApiMessage EmailConfirmationRateLimited
            = new(
                "EMAIL_CONFIRMATION_RATE_LIMITED",
                "Too many email-confirmation requests. Please try again in a few minutes.");

        /// <summary>The refresh limited.</summary>
        public static readonly ApiMessage RefreshRateLimited
            = new(
                "REFRESH_RATE_LIMITED",
                "Too many refresh requests. Please try again in a few minutes.");

        /// <summary>The logout limited.</summary>
        public static readonly ApiMessage LogoutRateLimited
            = new(
                "LOGOUT_RATE_LIMITED",
                "Too many logout requests. Please try again in a few minutes.");
    }

    /// <summary>The validation messages of the API response messages feature.</summary>
    public static class Validation
    {
        /// <summary>The validation error.</summary>
        public static readonly ApiMessage ValidationError
            = new("VALIDATION_ERROR", "One or more validation errors occurred.");

        /// <summary>The invalid operation.</summary>
        public static readonly ApiMessage InvalidOperation
            = new("INVALID_OPERATION", "Operation is invalid.");

        /// <summary>The invalid request body.</summary>
        public static readonly ApiMessage InvalidRequestBody
            = new(
                "INVALID_REQUEST_BODY",
                "The request is malformed or contains invalid values.");
    }

    /// <summary>The messages of the routing feature.</summary>
    public static class Routing
    {
        /// <summary>The endpoint not found.</summary>
        public static readonly ApiMessage EndpointNotFound
            = new(
                "ENDPOINT_NOT_FOUND",
                "The endpoint you're looking for does not exist.");
    }

    /// <summary>The database messages of the API response messages.</summary>
    public static class Database
    {
        /// <summary>The update error.</summary>
        public static readonly ApiMessage UpdateError =
            new(
                "DB_UPDATE_ERROR",
                "An error occurred while updating the database.");
    }

    /// <summary>The general messages of the API response messages.</summary>
    public static class General
    {
        /// <summary>The unknown error.</summary>
        public static readonly ApiMessage UnknownError
            = new(
                "UNKNOWN_ERROR",
                "An unexpected error occurred. Please try again later.");
    }

    /// <summary>The amenity messages of the API response messages.</summary>
    public static class Amenity
    {
        /// <summary>The create amenity rate limited.</summary>
        public static readonly ApiMessage CreateRateLimited
            = new(
                "CREATE_AMENITY_RATE_LIMITED",
                "Too many amenity creation requests. Please try again in a few minutes.");

        /// <summary>The get all amenities rate limited.</summary>
        public static readonly ApiMessage GetAllRateLimited
            = new(
                "GET_ALL_AMENITIES_RATE_LIMITED",
                "Too many requests to retrieve amenities. Please try again in a few minutes.");

        /// <summary>The get amenity by id rate limited.</summary>
        public static readonly ApiMessage GetByIdRateLimited
            = new(
                "GET_AMENITY_RATE_LIMITED",
                "Too many requests to retrieve the amenity. Please try again in a few minutes.");

        /// <summary>The update amenity rate limited.</summary>
        public static readonly ApiMessage UpdateRateLimited
            = new(
                "UPDATE_AMENITY_RATE_LIMITED",
                "Too many amenity update requests. Please try again in a few minutes.");

        /// <summary>The delete amenity rate limited.</summary>
        public static readonly ApiMessage DeleteRateLimited
            = new(
                "DELETE_AMENITY_RATE_LIMITED",
                "Too many amenity deletion requests. Please try again in a few minutes.");

        /// <summary>The get hotel amenities rate limited.</summary>
        public static readonly ApiMessage GetHotelAmenitiesRateLimited
            = new(
                "GET_HOTEL_AMENITIES_RATE_LIMITED",
                "Too many requests to retrieve hotel amenities. Please try again in a few minutes.");

        /// <summary>The set hotel amenities rate limited.</summary>
        public static readonly ApiMessage SetHotelAmenitiesRateLimited
            = new(
                "SET_HOTEL_AMENITIES_RATE_LIMITED",
                "Too many hotel amenity update requests. Please try again in a few minutes.");
    }
    /// <summary>The attraction messages of the API response messages.</summary>
    public static class Attraction
    {
        /// <summary>The get hotel attractions rate limited.</summary>
        public static readonly ApiMessage GetHotelAttractionsRateLimited
            = new(
                "GET_HOTEL_ATTRACTIONS_RATE_LIMITED",
                "Too many requests to retrieve hotel attractions. Please try again in a few minutes.");

        /// <summary>The create attraction rate limited.</summary>
        public static readonly ApiMessage CreateRateLimited
            = new(
                "CREATE_ATTRACTION_RATE_LIMITED",
                "Too many attraction creation requests. Please try again in a few minutes.");

        /// <summary>The update attraction rate limited.</summary>
        public static readonly ApiMessage UpdateRateLimited
            = new(
                "UPDATE_ATTRACTION_RATE_LIMITED",
                "Too many attraction update requests. Please try again in a few minutes.");

        /// <summary>The delete attraction rate limited.</summary>
        public static readonly ApiMessage DeleteRateLimited
            = new(
                "DELETE_ATTRACTION_RATE_LIMITED",
                "Too many attraction deletion requests. Please try again in a few minutes.");
    }
    /// <summary>The booking messages of the API response messages.</summary>
    public static class Booking
    {
        /// <summary>The checkout rate limited.</summary>
        public static readonly ApiMessage CheckoutRateLimited
            = new(
                "CHECKOUT_RATE_LIMITED",
                "Too many checkout requests. Please try again in a few minutes.");

        /// <summary>The get my bookings rate limited.</summary>
        public static readonly ApiMessage GetMyBookingsRateLimited
            = new(
                "GET_MY_BOOKINGS_RATE_LIMITED",
                "Too many requests to retrieve your bookings. Please try again in a few minutes.");

        /// <summary>The get booking by confirmation number rate limited.</summary>
        public static readonly ApiMessage GetByConfirmationNumberRateLimited
            = new(
                "GET_BOOKING_RATE_LIMITED",
                "Too many requests to retrieve the booking. Please try again in a few minutes.");

        /// <summary>The cancel booking rate limited.</summary>
        public static readonly ApiMessage CancelRateLimited
            = new(
                "CANCEL_BOOKING_RATE_LIMITED",
                "Too many booking cancellation requests. Please try again in a few minutes.");
    }
    /// <summary>The cart messages of the API response messages.</summary>
    public static class Cart
    {
        /// <summary>The get cart rate limited.</summary>
        public static readonly ApiMessage GetRateLimited
            = new(
                "GET_CART_RATE_LIMITED",
                "Too many requests to retrieve the cart. Please try again in a few minutes.");

        /// <summary>The add cart item rate limited.</summary>
        public static readonly ApiMessage AddItemRateLimited
            = new(
                "ADD_CART_ITEM_RATE_LIMITED",
                "Too many requests to add items to the cart. Please try again in a few minutes.");

        /// <summary>The remove cart item rate limited.</summary>
        public static readonly ApiMessage RemoveItemRateLimited
            = new(
                "REMOVE_CART_ITEM_RATE_LIMITED",
                "Too many requests to remove items from the cart. Please try again in a few minutes.");

        /// <summary>The clear cart rate limited.</summary>
        public static readonly ApiMessage ClearRateLimited
            = new(
                "CLEAR_CART_RATE_LIMITED",
                "Too many cart clearing requests. Please try again in a few minutes.");
    }

    /// <summary>Contains city-related API response messages.</summary>
    public static class City
    {
        /// <summary>Message returned when city creation requests are rate limited.</summary>
        public static readonly ApiMessage CreateRateLimited
            = new(
                "CREATE_CITY_RATE_LIMITED",
                "Too many city creation requests. Please try again in a few minutes.");

        /// <summary>Message returned when retrieving all cities is rate limited.</summary>
        public static readonly ApiMessage GetAllRateLimited
            = new(
                "GET_ALL_CITIES_RATE_LIMITED",
                "Too many requests to retrieve cities. Please try again in a few minutes.");

        /// <summary>Message returned when retrieving a city by ID is rate limited.</summary>
        public static readonly ApiMessage GetByIdRateLimited
            = new(
                "GET_CITY_RATE_LIMITED",
                "Too many requests to retrieve the city. Please try again in a few minutes.");

        /// <summary>Message returned when city update requests are rate limited.</summary>
        public static readonly ApiMessage UpdateRateLimited
            = new(
                "UPDATE_CITY_RATE_LIMITED",
                "Too many city update requests. Please try again in a few minutes.");

        /// <summary>Message returned when city deletion requests are rate limited.</summary>
        public static readonly ApiMessage DeleteRateLimited
            = new(
                "DELETE_CITY_RATE_LIMITED",
                "Too many city deletion requests. Please try again in a few minutes.");

        /// <summary>Message returned when city thumbnail upload requests are rate limited.</summary>
        public static readonly ApiMessage UploadThumbnailRateLimited
            = new(
                "UPLOAD_CITY_THUMBNAIL_RATE_LIMITED",
                "Too many city thumbnail upload requests. Please try again in a few minutes.");

        /// <summary>Message returned when city thumbnail deletion requests are rate limited.</summary>
        public static readonly ApiMessage DeleteThumbnailRateLimited
            = new(
                "DELETE_CITY_THUMBNAIL_RATE_LIMITED",
                "Too many city thumbnail deletion requests. Please try again in a few minutes.");
    }
    /// <summary>Contains deal-related API response messages.</summary>
    public static class Deal
    {
        /// <summary>Message returned when deal creation requests are rate limited.</summary>
        public static readonly ApiMessage CreateRateLimited
            = new(
                "CREATE_DEAL_RATE_LIMITED",
                "Too many deal creation requests. Please try again in a few minutes.");

        /// <summary>Message returned when retrieving deals is rate limited.</summary>
        public static readonly ApiMessage GetAllRateLimited
            = new(
                "GET_DEALS_RATE_LIMITED",
                "Too many requests to retrieve deals. Please try again in a few minutes.");

        /// <summary>Message returned when retrieving a deal by ID is rate limited.</summary>
        public static readonly ApiMessage GetByIdRateLimited
            = new(
                "GET_DEAL_RATE_LIMITED",
                "Too many requests to retrieve the deal. Please try again in a few minutes.");

        /// <summary>Message returned when deal update requests are rate limited.</summary>
        public static readonly ApiMessage UpdateRateLimited
            = new(
                "UPDATE_DEAL_RATE_LIMITED",
                "Too many deal update requests. Please try again in a few minutes.");

        /// <summary>Message returned when deal deletion requests are rate limited.</summary>
        public static readonly ApiMessage DeleteRateLimited
            = new(
                "DELETE_DEAL_RATE_LIMITED",
                "Too many deal deletion requests. Please try again in a few minutes.");
    }
    /// <summary>Contains home-related API response messages.</summary>
    public static class Home
    {
        /// <summary>Message returned when retrieving featured deals is rate limited.</summary>
        public static readonly ApiMessage GetFeaturedDealsRateLimited
            = new(
                "GET_FEATURED_DEALS_RATE_LIMITED",
                "Too many requests to retrieve featured deals. Please try again in a few minutes.");

        /// <summary>Message returned when retrieving trending cities is rate limited.</summary>
        public static readonly ApiMessage GetTrendingCitiesRateLimited
            = new(
                "GET_TRENDING_CITIES_RATE_LIMITED",
                "Too many requests to retrieve trending cities. Please try again in a few minutes.");

        /// <summary>Message returned when retrieving recently visited hotels is rate limited.</summary>
        public static readonly ApiMessage GetRecentHotelsRateLimited
            = new(
                "GET_RECENT_HOTELS_RATE_LIMITED",
                "Too many requests to retrieve recently visited hotels. Please try again in a few minutes.");
    }

    /// <summary>Contains hotel-related API response messages.</summary>
    public static class Hotel
    {
        /// <summary>Message returned when hotel creation requests are rate limited.</summary>
        public static readonly ApiMessage CreateRateLimited
            = new(
                "CREATE_HOTEL_RATE_LIMITED",
                "Too many hotel creation requests. Please try again in a few minutes.");

        /// <summary>Message returned when retrieving all hotels is rate limited.</summary>
        public static readonly ApiMessage GetAllRateLimited
            = new(
                "GET_ALL_HOTELS_RATE_LIMITED",
                "Too many requests to retrieve hotels. Please try again in a few minutes.");

        /// <summary>Message returned when retrieving a hotel by ID is rate limited.</summary>
        public static readonly ApiMessage GetByIdRateLimited
            = new(
                "GET_HOTEL_RATE_LIMITED",
                "Too many requests to retrieve the hotel. Please try again in a few minutes.");

        /// <summary>Message returned when hotel update requests are rate limited.</summary>
        public static readonly ApiMessage UpdateRateLimited
            = new(
                "UPDATE_HOTEL_RATE_LIMITED",
                "Too many hotel update requests. Please try again in a few minutes.");

        /// <summary>Message returned when hotel deletion requests are rate limited.</summary>
        public static readonly ApiMessage DeleteRateLimited
            = new(
                "DELETE_HOTEL_RATE_LIMITED",
                "Too many hotel deletion requests. Please try again in a few minutes.");

        /// <summary>Message returned when hotel image upload requests are rate limited.</summary>
        public static readonly ApiMessage UploadImageRateLimited
            = new(
                "UPLOAD_HOTEL_IMAGE_RATE_LIMITED",
                "Too many hotel image upload requests. Please try again in a few minutes.");

        /// <summary>Message returned when retrieving hotel images is rate limited.</summary>
        public static readonly ApiMessage GetImagesRateLimited
            = new(
                "GET_HOTEL_IMAGES_RATE_LIMITED",
                "Too many requests to retrieve hotel images. Please try again in a few minutes.");

        /// <summary>Message returned when hotel image deletion requests are rate limited.</summary>
        public static readonly ApiMessage DeleteImageRateLimited
            = new(
                "DELETE_HOTEL_IMAGE_RATE_LIMITED",
                "Too many hotel image deletion requests. Please try again in a few minutes.");
    }
    /// <summary>Contains payment-related API response messages.</summary>
    public static class Payment
    {
        /// <summary>Message returned when booking payment requests are rate limited.</summary>
        public static readonly ApiMessage PayBookingRateLimited
            = new(
                "PAY_BOOKING_RATE_LIMITED",
                "Too many payment requests. Please try again in a few minutes.");

        /// <summary>Message returned when payment webhook requests are rate limited.</summary>
        public static readonly ApiMessage WebhookRateLimited
            = new(
                "PAYMENT_WEBHOOK_RATE_LIMITED",
                "Too many payment webhook requests. Please try again in a few minutes.");

        /// <summary>Message returned when booking invoice requests are rate limited.</summary>
        public static readonly ApiMessage GetBookingInvoiceRateLimited
            = new(
                "GET_BOOKING_INVOICE_RATE_LIMITED",
                "Too many invoice requests. Please try again in a few minutes.");
    }
    /// <summary>Contains review-related API response messages.</summary>
    public static class Review
    {
        /// <summary>Message returned when retrieving hotel reviews is rate limited.</summary>
        public static readonly ApiMessage GetHotelReviewsRateLimited
            = new(
                "GET_HOTEL_REVIEWS_RATE_LIMITED",
                "Too many requests to retrieve hotel reviews. Please try again in a few minutes.");

        /// <summary>Message returned when review creation requests are rate limited.</summary>
        public static readonly ApiMessage CreateRateLimited
            = new(
                "CREATE_REVIEW_RATE_LIMITED",
                "Too many review creation requests. Please try again in a few minutes.");

        /// <summary>Message returned when review update requests are rate limited.</summary>
        public static readonly ApiMessage UpdateRateLimited
            = new(
                "UPDATE_REVIEW_RATE_LIMITED",
                "Too many review update requests. Please try again in a few minutes.");

        /// <summary>Message returned when review deletion requests are rate limited.</summary>
        public static readonly ApiMessage DeleteRateLimited
            = new(
                "DELETE_REVIEW_RATE_LIMITED",
                "Too many review deletion requests. Please try again in a few minutes.");
    }

    /// <summary>Contains room-related API response messages.</summary>
    public static class Room
    {
        /// <summary>Message returned when room creation requests are rate limited.</summary>
        public static readonly ApiMessage CreateRateLimited
            = new(
                "CREATE_ROOM_RATE_LIMITED",
                "Too many room creation requests. Please try again in a few minutes.");

        /// <summary>Message returned when retrieving all rooms is rate limited.</summary>
        public static readonly ApiMessage GetAllRateLimited
            = new(
                "GET_ALL_ROOMS_RATE_LIMITED",
                "Too many requests to retrieve rooms. Please try again in a few minutes.");

        /// <summary>Message returned when retrieving the rooms of a hotel is rate limited.</summary>
        public static readonly ApiMessage GetHotelRoomsRateLimited
            = new(
                "GET_HOTEL_ROOMS_RATE_LIMITED",
                "Too many requests to retrieve hotel rooms. Please try again in a few minutes.");

        /// <summary>Message returned when retrieving a room by ID is rate limited.</summary>
        public static readonly ApiMessage GetByIdRateLimited
            = new(
                "GET_ROOM_RATE_LIMITED",
                "Too many requests to retrieve the room. Please try again in a few minutes.");

        /// <summary>Message returned when room update requests are rate limited.</summary>
        public static readonly ApiMessage UpdateRateLimited
            = new(
                "UPDATE_ROOM_RATE_LIMITED",
                "Too many room update requests. Please try again in a few minutes.");

        /// <summary>Message returned when room deletion requests are rate limited.</summary>
        public static readonly ApiMessage DeleteRateLimited
            = new(
                "DELETE_ROOM_RATE_LIMITED",
                "Too many room deletion requests. Please try again in a few minutes.");

        /// <summary>Message returned when room image upload requests are rate limited.</summary>
        public static readonly ApiMessage UploadImageRateLimited
            = new(
                "UPLOAD_ROOM_IMAGE_RATE_LIMITED",
                "Too many room image upload requests. Please try again in a few minutes.");

        /// <summary>Message returned when retrieving room images is rate limited.</summary>
        public static readonly ApiMessage GetImagesRateLimited
            = new(
                "GET_ROOM_IMAGES_RATE_LIMITED",
                "Too many requests to retrieve room images. Please try again in a few minutes.");

        /// <summary>Message returned when room image deletion requests are rate limited.</summary>
        public static readonly ApiMessage DeleteImageRateLimited
            = new(
                "DELETE_ROOM_IMAGE_RATE_LIMITED",
                "Too many room image deletion requests. Please try again in a few minutes.");
    }
    /// <summary>Contains search-related API response messages.</summary>
    public static class Search
    {
        /// <summary>Message returned when hotel search requests are rate limited.</summary>
        public static readonly ApiMessage HotelsRateLimited
            = new(
                "SEARCH_HOTELS_RATE_LIMITED",
                "Too many hotel search requests. Please try again in a few minutes.");
    }
}