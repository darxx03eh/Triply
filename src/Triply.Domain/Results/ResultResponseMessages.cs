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

    public static class Success
    {
        public static readonly ResultMessage Created = new("CREATED", "Resource created successfully.");
        public static readonly ResultMessage Ok = new("SUCCESS", "Operation completed successfully.");
        public static readonly ResultMessage NoContent = new("DELETED", "Resource deleted successfully.");
    }
}