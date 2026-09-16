namespace Triply.Domain.Results;

public static class ResultResponseMessages
{
    public static class Authentication
    {
        public static class Validation
        {
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
        }
    }
    public static class Success
    {
        public static readonly ResultMessage Created = new("CREATED", "Resource created successfully.");
        public static readonly ResultMessage Ok = new("SUCCESS", "Operation completed successfully.");
        public static readonly ResultMessage NoContent = new("DELETED", "Resource deleted successfully.");
    }
}