namespace Triply.Infrastructure.Routes;

public static class Router
{
    private const string Root = "api";
    private const string Version = "v1";
    private const string Rule = $"{Root}/{Version}";
    public static class AuthenticationRoutes
    {
        private const string Prefix = $"{Rule}/auth";
        public const string Register = $"{Prefix}/register";
        public const string Login = $"{Prefix}/login";
        public const string Refresh = $"{Prefix}/refresh";
        public const string EmailConfirmation = $"{Prefix}/confirm-email";
    }
}