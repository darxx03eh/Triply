namespace Triply.Api.Responses;

public static class ApiResponseMessages
{
    public static class Authentication
    {
        public static readonly ApiMessage NotAuthenticated
            = new("NOT_AUTHENTICATED", "You are not authorized to perform this action.");

        public static readonly ApiMessage AccessDenied
            = new("ACCESS_DENIED", "You do not have permission to access this resource.");

        public static readonly ApiMessage ForbiddenResource
            = new("ACCESS_DENIED", "Access to this resource is forbidden.");

        public static readonly ApiMessage TokenExpired
            = new("TOKEN_EXPIRED", "Your session has expired. Please sign in again.");
    }
    
    public static class Validation
    {
        public static readonly ApiMessage ValidationError
            = new("VALIDATION_ERROR", "One or more validation errors occurred.");

        public static readonly ApiMessage InvalidOperation
            = new("INVALID_OPERATION", "Operation is invalid.");
    }
    public static class Routing
    {
        public static readonly ApiMessage EndpointNotFound
            = new("ENDPOINT_NOT_FOUND", "The endpoint you're looking for does not exist.");
    }
    public static class Database
    {
        public static readonly ApiMessage UpdateError =
            new("DB_UPDATE_ERROR", "An error occurred while updating the database.");
    }
    
    public static class General
    {
        public static readonly ApiMessage UnknownError
            = new("UNKNOWN_ERROR", "An unexpected error occurred. Please try again later.");
    }
}