namespace Triply.Domain.Results;

public static class ResultResponseMessages
{
    public static class Authentication
    {
        public static class Validation
        {
            public static readonly ResultMessage TokenRequired
                = new("TOKEN_REQUIRED", "Token is required.");
            public static readonly ResultMessage FirstNameRequired
                = new("FIRST_NAME_REQUIRED", "First name is required.");

            public static readonly ResultMessage FirstNameMinLength
                = new("FIRST_NAME_MIN_LENGTH", "First name must be at least 3 characters.");

            public static readonly ResultMessage FirstNameMaxLength
                = new("FIRST_NAME_MAX_LENGTH", "First name must not exceed 100 characters.");

            public static readonly ResultMessage LastNameRequired
                = new("LAST_NAME_REQUIRED", "Last name is required.");

            public static readonly ResultMessage LastNameMinLength
                = new("LAST_NAME_MIN_LENGTH", "Last name must be at least 3 characters.");

            public static readonly ResultMessage LastNameMaxLength
                = new("LAST_NAME_MAX_LENGTH", "Last name must not exceed 100 characters.");

            public static readonly ResultMessage EmailRequired
                = new("EMAIL_REQUIRED", "Email is required.");

            public static readonly ResultMessage InvalidEmail
                = new("INVALID_EMAIL", "A valid email address is required.");

            public static readonly ResultMessage EmailAlreadyExists
                = new("EMAIL_ALREADY_EXISTS", "A user with this email already exists.");

            public static readonly ResultMessage EmailNotExists
                = new("EMAIL_NOT_EXISTS", "A user with this email not exists.");

            public static readonly ResultMessage PasswordRequired
                = new("PASSWORD_REQUIRED", "Password is required.");

            public static readonly ResultMessage PasswordMinLength
                = new("PASSWORD_MIN_LENGTH", "Password must be at least 8 characters.");

            public static readonly ResultMessage ConfirmPasswordRequired
                = new("CONFIRM_PASSWORD_REQUIRED", "Confirm password is required.");

            public static readonly ResultMessage PasswordsDoNotMatch
                = new("PASSWORDS_DO_NOT_MATCH", "Passwords do not match.");

            public static readonly ResultMessage PhoneNumberRequired
                = new("PHONE_NUMBER_REQUIRED", "Phone number is required.");

            public static readonly ResultMessage InvalidPhoneNumber
                = new("INVALID_PHONE_NUMBER", "A valid phone number is required.");

            public static readonly ResultMessage InvalidUsername
                = new("INVALID_USERNAME", "A valid username is required.");

            public static readonly ResultMessage MinimumAge
                = new("MINIMUM_AGE", "You must be at least 18 years old to register.");

            public static readonly ResultMessage UsernameRequired
                = new("USERNAME_REQUIRED", "Username is required.");

            public static readonly ResultMessage UsernameMinLength
                = new("USERNAME_MIN_LENGTH", "Username must be at least 3 characters.");

            public static readonly ResultMessage UsernameCannotContainAtSymbol
                = new("USERNAME_CANNOT_CONTAIN_SYMBOLS", "Username cannot contain symbols.");

            public static readonly ResultMessage UsernameAlreadyExists
                = new("USERNAME_ALREADY_EXISTS", "a user with this username already exists.");

            public static readonly ResultMessage Identifier
                = new("IDENTIFIER_REQUIRED", "Email, username, or phone number is required.");
        }

        public static class Api
        {
            public static readonly ResultMessage LoginSucceeded
                = new("LOGIN_SUCCEEDED", "Login Succeeded.");

            public static readonly ResultMessage RegisterSucceeded
                = new("REGISTER_SUCCEEDED", "Register new user Succeeded.");

            public static readonly ResultMessage ConfirmationSucceeded
                = new("EMAIL_CONFIRMED", "Email confirmed successfully.");

            public static readonly ResultMessage TokenRegenerated
                = new("TOKEN_REGENERATED", "Token Regenerated successfully.");
        }
    }

    public static class Hotels
    {
        public static class Validation
        {
            public static readonly ResultMessage NameRequired
                = new("HOTEL_NAME_REQUIRED", "Hotel name is required.");

            public static readonly ResultMessage NameMaxLength
                = new("HOTEL_NAME_MAX_LENGTH", "Hotel name must not exceed 150 characters.");

            public static readonly ResultMessage CityIdRequired
                = new("CITY_ID_REQUIRED", "City Id is required.");
            
            public static readonly ResultMessage HotelAlreadyExists
                = new("HOTEL_ALREADY_EXISTS", "This hotel already exists int this specific city.");

            public static readonly ResultMessage LocationAlreadyExists
                = new("LOCATION_ALREADY_EXISTS", "This latitude and longitude already exists.");
            
            public static readonly ResultMessage StarRatingInvalid
                = new("STAR_RATING_INVALID", "Star ratings must be between 1 and 5.");

            public static readonly ResultMessage DescriptionMaxLength
                = new("DESCRIPTION_MAX_LENGTH", "Hotel description must not exceed 2000 characters.");

            public static readonly ResultMessage LatitudeInvalid
                = new("LATITUDE_INVALID", "Latitude must be between -90 and 90.");

            public static readonly ResultMessage LongitudeInvalid
                = new("LONGITUDE_INVALID", "Longitude must be between -180 and 180.");

            public static readonly ResultMessage RowVersionRequired
                = new("HOTEL_ROW_VERSION_REQUIRED", "RowVersion is required to detect concurrent edits.");

            public static readonly ResultMessage RowVersionInvalidLength
                = new("HOTEL_ROW_VERSION_INVALID_LENGTH", "RowVersion must contain exactly 8 bytes.");

            public static readonly ResultMessage HotelTypeInvalid
                = new("HOTEL_TYPE_INVALID", "Hotel type must be Budget, Boutique or Luxury.");

            public static readonly ResultMessage AddressRequired
                = new("HOTEL_ADDRESS_REQUIRED", "Hotel address is required.");

            public static readonly ResultMessage AddressMaxLength
                = new("HOTEL_ADDRESS_MAX_LENGTH", "Hotel address must not exceed 300 characters.");

            public static readonly ResultMessage AmenityIdsRequired
                = new("HOTEL_AMENITY_IDS_REQUIRED", "Amenity ids list is required.");

            public static readonly ResultMessage AmenityIdsDuplicated
                = new("HOTEL_AMENITY_IDS_DUPLICATED", "Amenity ids must not contain duplicates.");

            public static readonly ResultMessage AmenityNotFound
                = new("HOTEL_AMENITY_NOT_FOUND", "One or more amenities were not found.");

            public static readonly ResultMessage PageInvalid
                = new("HOTEL_PAGE_INVALID", "Page must be greater than or equal to 1.");

            public static readonly ResultMessage PageSizeInvalid
                = new("HOTEL_PAGE_SIZE_INVALID", "Page size must be between 1 and 50.");
        }
    }

    public static class Cities
    {
        public static class Validation
        {
            public static readonly ResultMessage NameRequired
                = new("CITY_NAME_REQUIRED", "City name is required.");

            public static readonly ResultMessage NameMaxLength
                = new("CITY_NAME_MAX_LENGTH", "City name must not exceed 100 characters.");

            public static readonly ResultMessage NameWhitespace
                = new("CITY_NAME_WHITESPACE", "City name cannot contain only whitespace.");

            public static readonly ResultMessage CountryRequired
                = new("CITY_COUNTRY_REQUIRED", "Country is required.");

            public static readonly ResultMessage CountryMaxLength
                = new("CITY_COUNTRY_MAX_LENGTH", "Country must not exceed 100 characters.");

            public static readonly ResultMessage CountryWhitespace
                = new("CITY_COUNTRY_WHITESPACE", "Country cannot contain only whitespace.");

            public static readonly ResultMessage PostOfficeMaxLength
                = new("CITY_POST_OFFICE_MAX_LENGTH", "Post office must not exceed 20 characters.");

            public static readonly ResultMessage RowVersionRequired
                = new("CITY_ROW_VERSION_REQUIRED", "RowVersion is required to detect concurrent edits.");

            public static readonly ResultMessage RowVersionInvalidLength
                = new("CITY_ROW_VERSION_INVALID_LENGTH", "RowVersion must contain exactly 8 bytes.");

            public static readonly ResultMessage PageInvalid
                = new("CITY_PAGE_INVALID", "Page must be greater than or equal to 1.");

            public static readonly ResultMessage PageSizeInvalid
                = new("CITY_PAGE_SIZE_INVALID", "Page size must be between 1 and 50.");

            public static readonly ResultMessage CityAlreadExistsInThisCountry
                = new("CITY_ALREADY_EXISTS", "This city already exists in this specific country.");

            public static readonly ResultMessage CityNotFound
                = new("NOT_FOUND", "City with this Id was not found.");
        }
    }

    public static class Amenities
    {
        public static class Validation
        {
            public static readonly ResultMessage NameRequired
                = new("AMENITY_NAME_REQUIRED", "Amenity name is required.");

            public static readonly ResultMessage NameWhitespace
                = new("AMENITY_NAME_WHITESPACE", "Amenity name cannot contain only whitespace.");

            public static readonly ResultMessage NameMaxLength
                = new("AMENITY_NAME_MAX_LENGTH", "Amenity name must not exceed 100 characters.");

            public static readonly ResultMessage AmenityAlreadyExists
                = new("AMENITY_ALREADY_EXISTS", "An amenity with this name already exists.");
        }
    }
    
    public static class Reviews
    {
        public static class Validation
        {
            public static readonly ResultMessage RatingInvalid
                = new("REVIEW_RATING_INVALID", "Rating must be between 1 and 5.");

            public static readonly ResultMessage TitleMaxLength
                = new("REVIEW_TITLE_MAX_LENGTH", "Review title must not exceed 100 characters.");

            public static readonly ResultMessage CommentRequired
                = new("REVIEW_COMMENT_REQUIRED", "Review comment is required.");

            public static readonly ResultMessage CommentLength
                = new("REVIEW_COMMENT_LENGTH", "Review comment must be between 10 and 1000 characters.");

            public static readonly ResultMessage HotelNotFound
                = new("HOTEL_NOT_FOUND", "Hotel with this Id was not found.");

            public static readonly ResultMessage AlreadyReviewed
                = new("REVIEW_ALREADY_EXISTS",
                    "You have already reviewed this hotel. Update your existing review instead.");

            public static readonly ResultMessage PageInvalid
                = new("REVIEW_PAGE_INVALID", "Page must be greater than or equal to 1.");

            public static readonly ResultMessage PageSizeInvalid
                = new("REVIEW_PAGE_SIZE_INVALID", "Page size must be between 1 and 50.");
        }
    }

    public static class Attractions
    {
        public static class Validation
        {
            public static readonly ResultMessage NameRequired
                = new("ATTRACTION_NAME_REQUIRED", "Attraction name is required.");

            public static readonly ResultMessage NameMaxLength
                = new("ATTRACTION_NAME_MAX_LENGTH", "Attraction name must not exceed 150 characters.");

            public static readonly ResultMessage CategoryRequired
                = new("ATTRACTION_CATEGORY_REQUIRED", "Attraction category is required.");

            public static readonly ResultMessage CategoryMaxLength
                = new("ATTRACTION_CATEGORY_MAX_LENGTH", "Attraction category must not exceed 50 characters.");

            public static readonly ResultMessage DistanceInvalid
                = new("ATTRACTION_DISTANCE_INVALID", "Distance must be between 0 and 100 km with at most 2 decimals.");
        }
    }

    public static class Search
    {
        public static class Validation
        {
            public static readonly ResultMessage QueryMaxLength
                = new("SEARCH_QUERY_MAX_LENGTH", "Search text must not exceed 100 characters.");

            public static readonly ResultMessage CheckInInPast
                = new("SEARCH_CHECK_IN_IN_PAST", "Check-in date cannot be in the past.");

            public static readonly ResultMessage CheckOutBeforeCheckIn
                = new("SEARCH_CHECK_OUT_BEFORE_CHECK_IN", "Check-out date must be after the check-in date.");

            public static readonly ResultMessage StayTooLong
                = new("SEARCH_STAY_TOO_LONG", "A stay cannot be longer than 30 nights.");

            public static readonly ResultMessage AdultsInvalid
                = new("SEARCH_ADULTS_INVALID", "Adults must be between 1 and 20.");

            public static readonly ResultMessage ChildrenInvalid
                = new("SEARCH_CHILDREN_INVALID", "Children must be between 0 and 20.");

            public static readonly ResultMessage RoomsInvalid
                = new("SEARCH_ROOMS_INVALID", "Rooms must be between 1 and 10.");

            public static readonly ResultMessage RoomsMoreThanAdults
                = new("SEARCH_ROOMS_MORE_THAN_ADULTS", "Each room needs at least one adult.");

            public static readonly ResultMessage PriceInvalid
                = new("SEARCH_PRICE_INVALID", "Price must be greater than or equal to 0.");

            public static readonly ResultMessage PriceRangeInvalid
                = new("SEARCH_PRICE_RANGE_INVALID",
                    "Maximum price must be greater than or equal to the minimum price.");

            public static readonly ResultMessage StarsInvalid
                = new("SEARCH_STARS_INVALID", "Star ratings must be between 1 and 5.");

            public static readonly ResultMessage HotelTypeInvalid
                = new("SEARCH_HOTEL_TYPE_INVALID", "Hotel type must be Budget, Boutique or Luxury.");

            public static readonly ResultMessage AmenitiesTooMany
                = new("SEARCH_AMENITIES_TOO_MANY", "You can filter by at most 20 amenities.");

            public static readonly ResultMessage SortInvalid
                = new("SEARCH_SORT_INVALID",
                    "Sort must be one of: recommended, price_asc, price_desc, stars_desc, stars_asc, name.");

            public static readonly ResultMessage PageInvalid
                = new("SEARCH_PAGE_INVALID", "Page must be greater than or equal to 1.");

            public static readonly ResultMessage PageSizeInvalid
                = new("SEARCH_PAGE_SIZE_INVALID", "Page size must be between 1 and 50.");
        }
    }

    public static class Rooms
    {
        public static class Validation
        {
            public static readonly ResultMessage HotelIdRequired
                = new("HOTEL_ID_REQUIRED", "Hotel Id is required.");

            public static readonly ResultMessage HotelNotFound
                = new("HOTEL_NOT_FOUND", "Hotel with this Id was not found.");

            public static readonly ResultMessage NumberRequired
                = new("ROOM_NUMBER_REQUIRED", "Room number is required.");

            public static readonly ResultMessage NumberWhitespace
                = new("ROOM_NUMBER_WHITESPACE", "Room number cannot contain only whitespace.");

            public static readonly ResultMessage NumberMaxLength
                = new("ROOM_NUMBER_MAX_LENGTH", "Room number must not exceed 20 characters.");

            public static readonly ResultMessage RoomTypeInvalid
                = new("ROOM_TYPE_INVALID", "Room type must be Single, Double or Suite.");

            public static readonly ResultMessage AdultCapacityInvalid
                = new("ROOM_ADULT_CAPACITY_INVALID", "Adult capacity must be between 1 and 10.");

            public static readonly ResultMessage ChildCapacityInvalid
                = new("ROOM_CHILD_CAPACITY_INVALID", "Child capacity must be between 0 and 10.");

            public static readonly ResultMessage PricePerNightInvalid
                = new("ROOM_PRICE_INVALID", "Price per night must be greater than 0.");

            public static readonly ResultMessage PricePerNightPrecision
                = new("ROOM_PRICE_PRECISION", "Price per night must not exceed 8 digits and 2 decimals.");

            public static readonly ResultMessage DescriptionMaxLength
                = new("ROOM_DESCRIPTION_MAX_LENGTH", "Room description must not exceed 1000 characters.");

            public static readonly ResultMessage RoomAlreadyExists
                = new("ROOM_ALREADY_EXISTS", "This room number already exists in this hotel.");

            public static readonly ResultMessage RowVersionRequired
                = new("ROOM_ROW_VERSION_REQUIRED", "RowVersion is required to detect concurrent edits.");

            public static readonly ResultMessage RowVersionInvalidLength
                = new("ROOM_ROW_VERSION_INVALID_LENGTH", "RowVersion must contain exactly 8 bytes.");

            public static readonly ResultMessage PageInvalid
                = new("ROOM_PAGE_INVALID", "Page must be greater than or equal to 1.");

            public static readonly ResultMessage PageSizeInvalid
                = new("ROOM_PAGE_SIZE_INVALID", "Page size must be between 1 and 50.");
        }
    }

    public static class Deals
    {
        public static class Validation
        {
            public static readonly ResultMessage RoomIdRequired
                = new("DEAL_ROOM_ID_REQUIRED", "Room id is required.");
    
            public static readonly ResultMessage TitleRequired
                = new("DEAL_TITLE_REQUIRED", "Deal title is required.");
    
            public static readonly ResultMessage TitleMaxLength
                = new("DEAL_TITLE_MAX_LENGTH", "Deal title must not exceed 100 characters.");
    
            public static readonly ResultMessage DiscountInvalid
                = new("DEAL_DISCOUNT_INVALID", "Discount must be between 1 and 90 percent with at most 2 decimals.");
    
            public static readonly ResultMessage EndsBeforeStarts
                = new("DEAL_ENDS_BEFORE_STARTS", "Deal end date must be after its start date.");
    
            public static readonly ResultMessage EndsInPast
                = new("DEAL_ENDS_IN_PAST", "Deal end date cannot be in the past.");
    
            public static readonly ResultMessage PageInvalid
                = new("DEAL_PAGE_INVALID", "Page must be greater than or equal to 1.");
    
            public static readonly ResultMessage PageSizeInvalid
                = new("DEAL_PAGE_SIZE_INVALID", "Page size must be between 1 and 50.");
        }
    }

    public static class Success
    {
        public static readonly ResultMessage Created = new("CREATED", "Resource created successfully.");
        public static readonly ResultMessage Ok = new("SUCCESS", "Operation completed successfully.");
        public static readonly ResultMessage NoContent = new("DELETED", "Resource deleted successfully.");
    }
}