using Triply.Api.DependencyInjection;
using Triply.Api.Middlewares;
using Triply.Infrastructure.DependencyInjection;
using MessageQueue.DependencyInjection;
using Triply.Api.Endpoints;
using Logging.DependencyInjection;
using Serilog;
using Microsoft.Extensions.Hosting;
using Serilog.Extensions.Hosting;

namespace Triply.Api;

/// <summary>Represents the program.</summary>
public class Program
{
    /// <summary>
    /// Creates a conventional host for in-process integration tests.
    /// Production starts through <see cref="Main"/>, while the integration-test host discovers this method
    /// and replaces external infrastructure with test doubles before the host is built.
    /// </summary>
    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.ConfigureServices((context, services) =>
                {
                    ConfigureServices(services, context.Configuration);
                    services.AddSingleton<DiagnosticContext>(_ => new DiagnosticContext(Log.Logger));
                    services.AddSingleton<IDiagnosticContext>(services =>
                        services.GetRequiredService<DiagnosticContext>());
                });
                webBuilder.Configure((context, app) =>
                    ConfigureConventionalPipeline(app, context.HostingEnvironment));
            });

    /// <summary>Mains.</summary>
    public static async Task Main(string[] args)
    {
        TriplyLoggingExtensions.CreateBootstrapLogger();
        try
        {
            Log.Information("Starting Triply API");
            await RunAsync(args);
        }
        catch (Exception exception) when (exception is not HostAbortedException)
        {
            Log.Fatal(exception, "Triply API terminated unexpectedly");
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }

    private static async Task RunAsync(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // Add Serilog (Console + Elasticsearch)
        builder.AddTriplyLogging("api");
        
        ConfigureServices(builder.Services, builder.Configuration);

        var app = builder.Build();

        await app.Services.SeedAsync();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Triply API v1");
                options.RoutePrefix = "swagger";
                options.DisplayRequestDuration();
            });
        }

        // Correlation id + one log per request
        app.UseTriplyRequestLogging();
        app.UseHttpsRedirection();
        // Use registered policy
        app.UseCors("FrontendCORSPolicy");
        // Middleware to handles error
        app.UseMiddleware<ErrorHandlerMiddleware>();
        // Middleware to handles rate limit for login
        app.UseMiddleware<RateLimitMiddleware>();
        // Endpoint to check if the backend is healthy
        app.MapHealthChecks("/health");
        app.UseAuthentication();
        app.UseAuthorization();

        // Map all endpoints
        app.MapEndpoints();
        // When route not found return 404 Not Found
        app.Map404NotFoundEndpoints();
        await app.RunAsync();
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Add booking options.
        services.AddBookingOptions(configuration);
        // Add Triply DbContext
        services.AddTriplyDbContext(configuration);
        // Add Redis Service
        services.AddRedisService(configuration);
        // Add Sieve Service
        services.AddSieveService(configuration);
        // Add Infrastructure Dependencies
        services.AddInfrastructureDependencies();
        // Add Decorators
        services.ApplyDecorators();
        // Add RabbitMqMessaging Service
        services.AddRabbitMqMessaging(configuration);
        // Add Identity Settings
        services.AddIdentityServices();
        // Add JWT Authentication Settings
        services.AddJwtAuthentication(configuration);
        // Add frontend CORS
        services.AllowFrontendCors(configuration);
        // Add Generate Invoices PDF Configuration
        services.AddGenerateInvoiceConfigurations();
        // Add Payment Gateway
        services.AddPaymentServices(configuration);
        // Add Health Checks Service
        services.AddHealthChecks();
        // Add services to the container.
        services.AddAuthorization();
        // Add Swagger
        services.AddSwagger();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        services.AddOpenApi();
    }

    private static void ConfigureConventionalPipeline(IApplicationBuilder app, IHostEnvironment environment)
    {
        if (environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Triply API v1");
                options.RoutePrefix = "swagger";
                options.DisplayRequestDuration();
            });
        }

        app.UseTriplyRequestLogging();
        app.UseHttpsRedirection();
        app.UseMiddleware<ErrorHandlerMiddleware>();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            if (environment.IsDevelopment())
                endpoints.MapOpenApi();

            endpoints.MapHealthChecks("/health");
            endpoints.MapEndpoints();
            endpoints.Map404NotFoundEndpoints();
        });
    }
}
