namespace Triply.Api.Routes;

public static class Router
{
    private const string Root = "api";
    private const string Version = "v1";
    private const string Rule = $"{Root}/{Version}";
    public static class AuthenticationRoutes
    {
        private const string Prefix = $"{Rule}/auth";
        public const string Register = $"{Prefix}/register";
    }
}