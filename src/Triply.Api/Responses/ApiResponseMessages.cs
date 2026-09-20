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
    }
    
    /// <summary>The validation messages of the API response messages.</summary>
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
            = new("INVALID_REQUEST_BODY", "The request is malformed or contains invalid values.");
    }
    /// <summary>The messages of the routing feature.</summary>
    public static class Routing
    {
        /// <summary>The endpoint not found.</summary>
        public static readonly ApiMessage EndpointNotFound
            = new("ENDPOINT_NOT_FOUND", "The endpoint you're looking for does not exist.");
    }
    /// <summary>The database messages of the API response messages.</summary>
    public static class Database
    {
        /// <summary>The update error.</summary>
        public static readonly ApiMessage UpdateError =
            new("DB_UPDATE_ERROR", "An error occurred while updating the database.");
    }
    
    /// <summary>The general messages of the API response messages.</summary>
    public static class General
    {
        /// <summary>The unknown error.</summary>
        public static readonly ApiMessage UnknownError
            = new("UNKNOWN_ERROR", "An unexpected error occurred. Please try again later.");
    }
}