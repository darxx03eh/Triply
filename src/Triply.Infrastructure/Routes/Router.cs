namespace Triply.Infrastructure.Routes;

/// <summary>Defines route constants used by the API.</summary>
public static class Router
{
    private const string Root = "api";
    private const string Version = "v1";
    private const string Rule = $"{Root}/{Version}";
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
}