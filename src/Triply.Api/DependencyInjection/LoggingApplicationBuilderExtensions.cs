using System.Security.Claims;
using Serilog;
using Serilog.Events;
using Triply.Api.Middlewares;
using Triply.Domain.Constants;

namespace Triply.Api.DependencyInjection;

/// <summary>Extension methods for logging application builder.</summary>
public static class LoggingApplicationBuilderExtensions
{
    extension(IApplicationBuilder app)
    {
        /// <summary>
        /// One log per request (method, path, status code, elapsed time) enriched with the user, client and
        /// correlation id. 5xx are errors, 4xx are warnings and the docker health checks are hidden.
        /// </summary>
        public IApplicationBuilder UseTriplyRequestLogging()
        {
            app.UseMiddleware<CorrelationIdMiddleware>();
            app.UseSerilogRequestLogging(options =>
            {
                options.MessageTemplate =
                    "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

                options.GetLevel = (context, _, exception) =>
                    exception is not null || context.Response.StatusCode >= 500 ? LogEventLevel.Error
                    : context.Request.Path.StartsWithSegments("/health") ? LogEventLevel.Verbose
                    : context.Response.StatusCode >= 400 ? LogEventLevel.Warning
                    : LogEventLevel.Information;

                options.EnrichDiagnosticContext = (diagnostic, context) =>
                {
                    diagnostic.Set("RequestHost", context.Request.Host.Value);
                    diagnostic.Set("RequestScheme", context.Request.Scheme);
                    diagnostic.Set("QueryString", context.Request.QueryString.Value);
                    diagnostic.Set("ClientIp", context.Connection.RemoteIpAddress?.ToString());
                    diagnostic.Set("UserAgent", context.Request.Headers.UserAgent.ToString());
                    diagnostic.Set("Endpoint", context.GetEndpoint()?.DisplayName);
                    diagnostic.Set("ResponseContentType", context.Response.ContentType);

                    if (context.User.Identity?.IsAuthenticated == true)
                    {
                        diagnostic.Set("UserId", context.User.FindFirstValue(TokenClaims.Id));
                        diagnostic.Set("UserName", context.User.FindFirstValue(TokenClaims.Username));
                        diagnostic.Set("UserRoles",
                            context.User.FindAll(TokenClaims.Role).Select(c => c.Value).ToArray());
                    }
                };
            });

            return app;
        }
    }
}
