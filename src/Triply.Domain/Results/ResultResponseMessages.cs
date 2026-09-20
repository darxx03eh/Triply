namespace Triply.Domain.Results;

/// <summary>The messages of the result response messages feature.</summary>
public static class ResultResponseMessages
{
    /// <summary>The authentication messages of the result response messages.</summary>
    public static class Authentication
    {
        /// <summary>The validation messages.</summary>
        public static class Validation
        {
            /// <summary>Message returned when the token is missing.</summary>
            public static readonly ResultMessage TokenRequired
                = new("TOKEN_REQUIRED", "Token is required.");
            /// <summary>Message returned when the first name is missing.</summary>
            public static readonly ResultMessage FirstNameRequired
                = new("FIRST_NAME_REQUIRED", "First name is required.");

            /// <summary>Message returned when the first name is shorter than allowed.</summary>
            public static readonly ResultMessage FirstNameMinLength
                = new("FIRST_NAME_MIN_LENGTH", "First name must be at least 3 characters.");

            /// <summary>Message returned when the first name is longer than allowed.</summary>
            public static readonly ResultMessage FirstNameMaxLength
                = new("FIRST_NAME_MAX_LENGTH", "First name must not exceed 100 characters.");

            /// <summary>Message returned when the last name is missing.</summary>
            public static readonly ResultMessage LastNameRequired
                = new("LAST_NAME_REQUIRED", "Last name is required.");

            /// <summary>Message returned when the last name is shorter than allowed.</summary>
            public static readonly ResultMessage LastNameMinLength
                = new("LAST_NAME_MIN_LENGTH", "Last name must be at least 3 characters.");

            /// <summary>Message returned when the last name is longer than allowed.</summary>
            public static readonly ResultMessage LastNameMaxLength
                = new("LAST_NAME_MAX_LENGTH", "Last name must not exceed 100 characters.");

            /// <summary>Message returned when the email is missing.</summary>
            public static readonly ResultMessage EmailRequired
                = new("EMAIL_REQUIRED", "Email is required.");

            /// <summary>Message returned when the email is invalid.</summary>
            public static readonly ResultMessage InvalidEmail
                = new("INVALID_EMAIL", "A valid email address is required.");

            /// <summary>Message returned when the email already exists.</summary>
            public static readonly ResultMessage EmailAlreadyExists
                = new("EMAIL_ALREADY_EXISTS", "A user with this email already exists.");

            /// <summary>Message returned when the email not already exists.</summary>
            public static readonly ResultMessage EmailNotExists
                = new("EMAIL_NOT_EXISTS", "A user with this email not exists.");

            /// <summary>Message returned when the password is missing.</summary>
            public static readonly ResultMessage PasswordRequired
                = new("PASSWORD_REQUIRED", "Password is required.");

            /// <summary>Message returned when the password is shorter than allowed.</summary>
            public static readonly ResultMessage PasswordMinLength
                = new("PASSWORD_MIN_LENGTH", "Password must be at least 8 characters.");

            /// <summary>Message returned when the confirm password is missing.</summary>
            public static readonly ResultMessage ConfirmPasswordRequired
                = new("CONFIRM_PASSWORD_REQUIRED", "Confirm password is required.");

            /// <summary>Message returned for the passwords do not match.</summary>
            public static readonly ResultMessage PasswordsDoNotMatch
                = new("PASSWORDS_DO_NOT_MATCH", "Passwords do not match.");

            /// <summary>Message returned when the phone number is missing.</summary>
            public static readonly ResultMessage PhoneNumberRequired
                = new("PHONE_NUMBER_REQUIRED", "Phone number is required.");

            /// <summary>Message returned when the phone number is invalid.</summary>
            public static readonly ResultMessage InvalidPhoneNumber
                = new("INVALID_PHONE_NUMBER", "A valid phone number is required.");

            /// <summary>Message returned when the username is invalid.</summary>
            public static readonly ResultMessage InvalidUsername
                = new("INVALID_USERNAME", "A valid username is required.");

            /// <summary>Message returned for the minimum age.</summary>
            public static readonly ResultMessage MinimumAge
                = new("MINIMUM_AGE", "You must be at least 18 years old to register.");

            /// <summary>Message returned when the username is missing.</summary>
            public static readonly ResultMessage UsernameRequired
                = new("USERNAME_REQUIRED", "Username is required.");

            /// <summary>Message returned when the username is shorter than allowed.</summary>
            public static readonly ResultMessage UsernameMinLength
                = new("USERNAME_MIN_LENGTH", "Username must be at least 3 characters.");

            /// <summary>Message returned for the username cannot contain at symbol.</summary>
            public static readonly ResultMessage UsernameCannotContainAtSymbol
                = new("USERNAME_CANNOT_CONTAIN_SYMBOLS", "Username cannot contain symbols.");

            /// <summary>Message returned when the username already exists.</summary>
            public static readonly ResultMessage UsernameAlreadyExists
                = new("USERNAME_ALREADY_EXISTS", "a user with this username already exists.");

            /// <summary>Message returned for the identifier.</summary>
            public static readonly ResultMessage Identifier
                = new("IDENTIFIER_REQUIRED", "Email, username, or phone number is required.");
        }

        /// <summary>The API response messages.</summary>
        public static class Api
        {
            /// <summary>Message returned when the login succeeds.</summary>
            public static readonly ResultMessage LoginSucceeded
                = new("LOGIN_SUCCEEDED", "Login Succeeded.");

            /// <summary>Message returned when the register succeeds.</summary>
            public static readonly ResultMessage RegisterSucceeded
                = new("REGISTER_SUCCEEDED", "Register new user Succeeded.");

            /// <summary>Message returned when the confirmation succeeds.</summary>
            public static readonly ResultMessage ConfirmationSucceeded
                = new("EMAIL_CONFIRMED", "Email confirmed successfully.");

            /// <summary>Message returned for the token regenerated.</summary>
            public static readonly ResultMessage TokenRegenerated
                = new("TOKEN_REGENERATED", "Token Regenerated successfully.");
        }
    }

    /// <summary>The messages of the hotels feature.</summary>
    public static class Hotels
    {
        /// <summary>The validation messages of the hotels.</summary>
        public static class Validation
        {
            /// <summary>Message returned when the name is missing.</summary>
            public static readonly ResultMessage NameRequired
                = new("HOTEL_NAME_REQUIRED", "Hotel name is required.");

            /// <summary>Message returned when the name is longer than allowed.</summary>
            public static readonly ResultMessage NameMaxLength
                = new("HOTEL_NAME_MAX_LENGTH", "Hotel name must not exceed 150 characters.");

            /// <summary>Message returned when the city identifier is missing.</summary>
            public static readonly ResultMessage CityIdRequired
                = new("CITY_ID_REQUIRED", "City Id is required.");
            
            /// <summary>Message returned when the hotel already exists.</summary>
            public static readonly ResultMessage HotelAlreadyExists
                = new("HOTEL_ALREADY_EXISTS", "This hotel already exists int this specific city.");

            /// <summary>Message returned when the location already exists.</summary>
            public static readonly ResultMessage LocationAlreadyExists
                = new("LOCATION_ALREADY_EXISTS", "This latitude and longitude already exists.");
            
            /// <summary>Message returned when the star rating is invalid.</summary>
            public static readonly ResultMessage StarRatingInvalid
                = new("STAR_RATING_INVALID", "Star ratings must be between 1 and 5.");

            /// <summary>Message returned when the description is longer than allowed.</summary>
            public static readonly ResultMessage DescriptionMaxLength
                = new("DESCRIPTION_MAX_LENGTH", "Hotel description must not exceed 2000 characters.");

            /// <summary>Message returned when the latitude is invalid.</summary>
            public static readonly ResultMessage LatitudeInvalid
                = new("LATITUDE_INVALID", "Latitude must be between -90 and 90.");

            /// <summary>Message returned when the longitude is invalid.</summary>
            public static readonly ResultMessage LongitudeInvalid
                = new("LONGITUDE_INVALID", "Longitude must be between -180 and 180.");

            /// <summary>Message returned when the row version is missing.</summary>
            public static readonly ResultMessage RowVersionRequired
                = new("HOTEL_ROW_VERSION_REQUIRED", "RowVersion is required to detect concurrent edits.");

            /// <summary>Message returned when the length of the row version invalid is out of range.</summary>
            public static readonly ResultMessage RowVersionInvalidLength
                = new("HOTEL_ROW_VERSION_INVALID_LENGTH", "RowVersion must contain exactly 8 bytes.");

            /// <summary>Message returned when the hotel type is invalid.</summary>
            public static readonly ResultMessage HotelTypeInvalid
                = new("HOTEL_TYPE_INVALID", "Hotel type must be Budget, Boutique or Luxury.");

            /// <summary>Message returned when the address is missing.</summary>
            public static readonly ResultMessage AddressRequired
                = new("HOTEL_ADDRESS_REQUIRED", "Hotel address is required.");

            /// <summary>Message returned when the address is longer than allowed.</summary>
            public static readonly ResultMessage AddressMaxLength
                = new("HOTEL_ADDRESS_MAX_LENGTH", "Hotel address must not exceed 300 characters.");

            /// <summary>Message returned when the amenity identifiers is missing.</summary>
            public static readonly ResultMessage AmenityIdsRequired
                = new("HOTEL_AMENITY_IDS_REQUIRED", "Amenity ids list is required.");

            /// <summary>Message returned for the amenity identifiers duplicated.</summary>
            public static readonly ResultMessage AmenityIdsDuplicated
                = new("HOTEL_AMENITY_IDS_DUPLICATED", "Amenity ids must not contain duplicates.");

            /// <summary>Message returned when the amenity is not found.</summary>
            public static readonly ResultMessage AmenityNotFound
                = new("HOTEL_AMENITY_NOT_FOUND", "One or more amenities were not found.");

            /// <summary>Message returned when the page is invalid.</summary>
            public static readonly ResultMessage PageInvalid
                = new("HOTEL_PAGE_INVALID", "Page must be greater than or equal to 1.");

            /// <summary>Message returned when the page size is invalid.</summary>
            public static readonly ResultMessage PageSizeInvalid
                = new("HOTEL_PAGE_SIZE_INVALID", "Page size must be between 1 and 50.");
        }
    }

    /// <summary>The messages of the cities feature.</summary>
    public static class Cities
    {
        /// <summary>The validation messages of the cities.</summary>
        public static class Validation
        {
            /// <summary>Message returned when the name is missing.</summary>
            public static readonly ResultMessage NameRequired
                = new("CITY_NAME_REQUIRED", "City name is required.");

            /// <summary>Message returned when the name is longer than allowed.</summary>
            public static readonly ResultMessage NameMaxLength
                = new("CITY_NAME_MAX_LENGTH", "City name must not exceed 100 characters.");

            /// <summary>Message returned for the name whitespace.</summary>
            public static readonly ResultMessage NameWhitespace
                = new("CITY_NAME_WHITESPACE", "City name cannot contain only whitespace.");

            /// <summary>Message returned when the country is missing.</summary>
            public static readonly ResultMessage CountryRequired
                = new("CITY_COUNTRY_REQUIRED", "Country is required.");

            /// <summary>Message returned when the country is longer than allowed.</summary>
            public static readonly ResultMessage CountryMaxLength
                = new("CITY_COUNTRY_MAX_LENGTH", "Country must not exceed 100 characters.");

            /// <summary>Message returned for the country whitespace.</summary>
            public static readonly ResultMessage CountryWhitespace
                = new("CITY_COUNTRY_WHITESPACE", "Country cannot contain only whitespace.");

            /// <summary>Message returned when the post office is longer than allowed.</summary>
            public static readonly ResultMessage PostOfficeMaxLength
                = new("CITY_POST_OFFICE_MAX_LENGTH", "Post office must not exceed 20 characters.");

            /// <summary>Message returned when the row version is missing.</summary>
            public static readonly ResultMessage RowVersionRequired
                = new("CITY_ROW_VERSION_REQUIRED", "RowVersion is required to detect concurrent edits.");

            /// <summary>Message returned when the length of the row version invalid is out of range.</summary>
            public static readonly ResultMessage RowVersionInvalidLength
                = new("CITY_ROW_VERSION_INVALID_LENGTH", "RowVersion must contain exactly 8 bytes.");

            /// <summary>Message returned when the page is invalid.</summary>
            public static readonly ResultMessage PageInvalid
                = new("CITY_PAGE_INVALID", "Page must be greater than or equal to 1.");

            /// <summary>Message returned when the page size is invalid.</summary>
            public static readonly ResultMessage PageSizeInvalid
                = new("CITY_PAGE_SIZE_INVALID", "Page size must be between 1 and 50.");

            /// <summary>Message returned for the city alread exists in this country.</summary>
            public static readonly ResultMessage CityAlreadExistsInThisCountry
                = new("CITY_ALREADY_EXISTS", "This city already exists in this specific country.");

            /// <summary>Message returned when the city is not found.</summary>
            public static readonly ResultMessage CityNotFound
                = new("NOT_FOUND", "City with this Id was not found.");
        }
    }

    /// <summary>The messages of the amenities feature.</summary>
    public static class Amenities
    {
        /// <summary>The validation messages of the amenities.</summary>
        public static class Validation
        {
            /// <summary>Message returned when the name is missing.</summary>
            public static readonly ResultMessage NameRequired
                = new("AMENITY_NAME_REQUIRED", "Amenity name is required.");

            /// <summary>Message returned for the name whitespace.</summary>
            public static readonly ResultMessage NameWhitespace
                = new("AMENITY_NAME_WHITESPACE", "Amenity name cannot contain only whitespace.");

            /// <summary>Message returned when the name is longer than allowed.</summary>
            public static readonly ResultMessage NameMaxLength
                = new("AMENITY_NAME_MAX_LENGTH", "Amenity name must not exceed 100 characters.");

            /// <summary>Message returned when the amenity already exists.</summary>
            public static readonly ResultMessage AmenityAlreadyExists
                = new("AMENITY_ALREADY_EXISTS", "An amenity with this name already exists.");
        }
    }
    
    /// <summary>The messages of the reviews feature.</summary>
    public static class Reviews
    {
        /// <summary>The validation messages of the reviews.</summary>
        public static class Validation
        {
            /// <summary>Message returned when the rating is invalid.</summary>
            public static readonly ResultMessage RatingInvalid
                = new("REVIEW_RATING_INVALID", "Rating must be between 1 and 5.");

            /// <summary>Message returned when the title is longer than allowed.</summary>
            public static readonly ResultMessage TitleMaxLength
                = new("REVIEW_TITLE_MAX_LENGTH", "Review title must not exceed 100 characters.");

            /// <summary>Message returned when the comment is missing.</summary>
            public static readonly ResultMessage CommentRequired
                = new("REVIEW_COMMENT_REQUIRED", "Review comment is required.");

            /// <summary>Message returned when the length of the comment is out of range.</summary>
            public static readonly ResultMessage CommentLength
                = new("REVIEW_COMMENT_LENGTH", "Review comment must be between 10 and 1000 characters.");

            /// <summary>Message returned when the hotel is not found.</summary>
            public static readonly ResultMessage HotelNotFound
                = new("HOTEL_NOT_FOUND", "Hotel with this Id was not found.");

            /// <summary>Message returned for the already reviewed.</summary>
            public static readonly ResultMessage AlreadyReviewed
                = new("REVIEW_ALREADY_EXISTS",
                    "You have already reviewed this hotel. Update your existing review instead.");

            /// <summary>Message returned when the page is invalid.</summary>
            public static readonly ResultMessage PageInvalid
                = new("REVIEW_PAGE_INVALID", "Page must be greater than or equal to 1.");

            /// <summary>Message returned when the page size is invalid.</summary>
            public static readonly ResultMessage PageSizeInvalid
                = new("REVIEW_PAGE_SIZE_INVALID", "Page size must be between 1 and 50.");
        }
    }

    /// <summary>The messages of the attractions feature.</summary>
    public static class Attractions
    {
        /// <summary>The validation messages of the attractions.</summary>
        public static class Validation
        {
            /// <summary>Message returned when the name is missing.</summary>
            public static readonly ResultMessage NameRequired
                = new("ATTRACTION_NAME_REQUIRED", "Attraction name is required.");

            /// <summary>Message returned when the name is longer than allowed.</summary>
            public static readonly ResultMessage NameMaxLength
                = new("ATTRACTION_NAME_MAX_LENGTH", "Attraction name must not exceed 150 characters.");

            /// <summary>Message returned when the category is missing.</summary>
            public static readonly ResultMessage CategoryRequired
                = new("ATTRACTION_CATEGORY_REQUIRED", "Attraction category is required.");

            /// <summary>Message returned when the category is longer than allowed.</summary>
            public static readonly ResultMessage CategoryMaxLength
                = new("ATTRACTION_CATEGORY_MAX_LENGTH", "Attraction category must not exceed 50 characters.");

            /// <summary>Message returned when the distance is invalid.</summary>
            public static readonly ResultMessage DistanceInvalid
                = new("ATTRACTION_DISTANCE_INVALID", "Distance must be between 0 and 100 km with at most 2 decimals.");
        }
    }

    /// <summary>The messages of the search feature.</summary>
    public static class Search
    {
        /// <summary>The validation messages of the search.</summary>
        public static class Validation
        {
            /// <summary>Message returned when the query is longer than allowed.</summary>
            public static readonly ResultMessage QueryMaxLength
                = new("SEARCH_QUERY_MAX_LENGTH", "Search text must not exceed 100 characters.");

            /// <summary>Message returned when the check in is in the past.</summary>
            public static readonly ResultMessage CheckInInPast
                = new("SEARCH_CHECK_IN_IN_PAST", "Check-in date cannot be in the past.");

            /// <summary>Message returned for the check out before check in.</summary>
            public static readonly ResultMessage CheckOutBeforeCheckIn
                = new("SEARCH_CHECK_OUT_BEFORE_CHECK_IN", "Check-out date must be after the check-in date.");

            /// <summary>Message returned for the stay too long.</summary>
            public static readonly ResultMessage StayTooLong
                = new("SEARCH_STAY_TOO_LONG", "A stay cannot be longer than 30 nights.");

            /// <summary>Message returned when the adults is invalid.</summary>
            public static readonly ResultMessage AdultsInvalid
                = new("SEARCH_ADULTS_INVALID", "Adults must be between 1 and 20.");

            /// <summary>Message returned when the children is invalid.</summary>
            public static readonly ResultMessage ChildrenInvalid
                = new("SEARCH_CHILDREN_INVALID", "Children must be between 0 and 20.");

            /// <summary>Message returned when the rooms is invalid.</summary>
            public static readonly ResultMessage RoomsInvalid
                = new("SEARCH_ROOMS_INVALID", "Rooms must be between 1 and 10.");

            /// <summary>Message returned for the rooms more than adults.</summary>
            public static readonly ResultMessage RoomsMoreThanAdults
                = new("SEARCH_ROOMS_MORE_THAN_ADULTS", "Each room needs at least one adult.");

            /// <summary>Message returned when the price is invalid.</summary>
            public static readonly ResultMessage PriceInvalid
                = new("SEARCH_PRICE_INVALID", "Price must be greater than or equal to 0.");

            /// <summary>Message returned when the price range is invalid.</summary>
            public static readonly ResultMessage PriceRangeInvalid
                = new("SEARCH_PRICE_RANGE_INVALID",
                    "Maximum price must be greater than or equal to the minimum price.");

            /// <summary>Message returned when the stars is invalid.</summary>
            public static readonly ResultMessage StarsInvalid
                = new("SEARCH_STARS_INVALID", "Star ratings must be between 1 and 5.");

            /// <summary>Message returned when the hotel type is invalid.</summary>
            public static readonly ResultMessage HotelTypeInvalid
                = new("SEARCH_HOTEL_TYPE_INVALID", "Hotel type must be Budget, Boutique or Luxury.");

            /// <summary>Message returned for the amenities too many.</summary>
            public static readonly ResultMessage AmenitiesTooMany
                = new("SEARCH_AMENITIES_TOO_MANY", "You can filter by at most 20 amenities.");

            /// <summary>Message returned when the sort is invalid.</summary>
            public static readonly ResultMessage SortInvalid
                = new("SEARCH_SORT_INVALID",
                    "Sort must be one of: recommended, price_asc, price_desc, stars_desc, stars_asc, name.");

            /// <summary>Message returned when the page is invalid.</summary>
            public static readonly ResultMessage PageInvalid
                = new("SEARCH_PAGE_INVALID", "Page must be greater than or equal to 1.");

            /// <summary>Message returned when the page size is invalid.</summary>
            public static readonly ResultMessage PageSizeInvalid
                = new("SEARCH_PAGE_SIZE_INVALID", "Page size must be between 1 and 50.");
        }
    }

    /// <summary>The messages of the rooms feature.</summary>
    public static class Rooms
    {
        /// <summary>The validation messages of the rooms.</summary>
        public static class Validation
        {
            /// <summary>Message returned when the hotel identifier is missing.</summary>
            public static readonly ResultMessage HotelIdRequired
                = new("HOTEL_ID_REQUIRED", "Hotel Id is required.");

            /// <summary>Message returned when the hotel is not found.</summary>
            public static readonly ResultMessage HotelNotFound
                = new("HOTEL_NOT_FOUND", "Hotel with this Id was not found.");

            /// <summary>Message returned when the number is missing.</summary>
            public static readonly ResultMessage NumberRequired
                = new("ROOM_NUMBER_REQUIRED", "Room number is required.");

            /// <summary>Message returned for the number whitespace.</summary>
            public static readonly ResultMessage NumberWhitespace
                = new("ROOM_NUMBER_WHITESPACE", "Room number cannot contain only whitespace.");

            /// <summary>Message returned when the number is longer than allowed.</summary>
            public static readonly ResultMessage NumberMaxLength
                = new("ROOM_NUMBER_MAX_LENGTH", "Room number must not exceed 20 characters.");

            /// <summary>Message returned when the room type is invalid.</summary>
            public static readonly ResultMessage RoomTypeInvalid
                = new("ROOM_TYPE_INVALID", "Room type must be Single, Double or Suite.");

            /// <summary>Message returned when the adult capacity is invalid.</summary>
            public static readonly ResultMessage AdultCapacityInvalid
                = new("ROOM_ADULT_CAPACITY_INVALID", "Adult capacity must be between 1 and 10.");

            /// <summary>Message returned when the child capacity is invalid.</summary>
            public static readonly ResultMessage ChildCapacityInvalid
                = new("ROOM_CHILD_CAPACITY_INVALID", "Child capacity must be between 0 and 10.");

            /// <summary>Message returned when the price per night is invalid.</summary>
            public static readonly ResultMessage PricePerNightInvalid
                = new("ROOM_PRICE_INVALID", "Price per night must be greater than 0.");

            /// <summary>Message returned when the price per night has too many decimals.</summary>
            public static readonly ResultMessage PricePerNightPrecision
                = new("ROOM_PRICE_PRECISION", "Price per night must not exceed 8 digits and 2 decimals.");

            /// <summary>Message returned when the description is longer than allowed.</summary>
            public static readonly ResultMessage DescriptionMaxLength
                = new("ROOM_DESCRIPTION_MAX_LENGTH", "Room description must not exceed 1000 characters.");

            /// <summary>Message returned when the room already exists.</summary>
            public static readonly ResultMessage RoomAlreadyExists
                = new("ROOM_ALREADY_EXISTS", "This room number already exists in this hotel.");

            /// <summary>Message returned when the row version is missing.</summary>
            public static readonly ResultMessage RowVersionRequired
                = new("ROOM_ROW_VERSION_REQUIRED", "RowVersion is required to detect concurrent edits.");

            /// <summary>Message returned when the length of the row version invalid is out of range.</summary>
            public static readonly ResultMessage RowVersionInvalidLength
                = new("ROOM_ROW_VERSION_INVALID_LENGTH", "RowVersion must contain exactly 8 bytes.");

            /// <summary>Message returned when the page is invalid.</summary>
            public static readonly ResultMessage PageInvalid
                = new("ROOM_PAGE_INVALID", "Page must be greater than or equal to 1.");

            /// <summary>Message returned when the page size is invalid.</summary>
            public static readonly ResultMessage PageSizeInvalid
                = new("ROOM_PAGE_SIZE_INVALID", "Page size must be between 1 and 50.");
        }
    }

    /// <summary>The messages of the deals feature.</summary>
    public static class Deals
    {
        /// <summary>The validation messages of the deals.</summary>
        public static class Validation
        {
            /// <summary>Message returned when the room identifier is missing.</summary>
            public static readonly ResultMessage RoomIdRequired
                = new("DEAL_ROOM_ID_REQUIRED", "Room id is required.");
    
            /// <summary>Message returned when the title is missing.</summary>
            public static readonly ResultMessage TitleRequired
                = new("DEAL_TITLE_REQUIRED", "Deal title is required.");
    
            /// <summary>Message returned when the title is longer than allowed.</summary>
            public static readonly ResultMessage TitleMaxLength
                = new("DEAL_TITLE_MAX_LENGTH", "Deal title must not exceed 100 characters.");
    
            /// <summary>Message returned when the discount is invalid.</summary>
            public static readonly ResultMessage DiscountInvalid
                = new("DEAL_DISCOUNT_INVALID", "Discount must be between 1 and 90 percent with at most 2 decimals.");
    
            /// <summary>Message returned for the ends before starts.</summary>
            public static readonly ResultMessage EndsBeforeStarts
                = new("DEAL_ENDS_BEFORE_STARTS", "Deal end date must be after its start date.");
    
            /// <summary>Message returned when the ends is in the past.</summary>
            public static readonly ResultMessage EndsInPast
                = new("DEAL_ENDS_IN_PAST", "Deal end date cannot be in the past.");
    
            /// <summary>Message returned when the page is invalid.</summary>
            public static readonly ResultMessage PageInvalid
                = new("DEAL_PAGE_INVALID", "Page must be greater than or equal to 1.");
    
            /// <summary>Message returned when the page size is invalid.</summary>
            public static readonly ResultMessage PageSizeInvalid
                = new("DEAL_PAGE_SIZE_INVALID", "Page size must be between 1 and 50.");
        }
    }

    /// <summary>The success messages of the result response messages.</summary>
    public static class Success
    {
        /// <summary>Message returned for the created.</summary>
        public static readonly ResultMessage Created = new("CREATED", "Resource created successfully.");
        /// <summary>Message returned for the ok.</summary>
        public static readonly ResultMessage Ok = new("SUCCESS", "Operation completed successfully.");
        /// <summary>Message returned for the no content.</summary>
        public static readonly ResultMessage NoContent = new("DELETED", "Resource deleted successfully.");
    }
}