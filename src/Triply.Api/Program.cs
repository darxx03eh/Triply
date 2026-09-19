using Triply.Api.DependencyInjection;
using Triply.Api.Middlewares;
using Triply.Infrastructure.DependencyInjection;
using MessageQueue.DependencyInjection;
using Triply.Api.Endpoints;

namespace Triply.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // Add Triply DbContext
        builder.Services.AddTriplyDbContext(builder.Configuration);
        
        // Add Redis Service
        builder.Services.AddRedisService(builder.Configuration);
        
        // Add Sieve Service
        builder.Services.AddSieveService(builder.Configuration);
        
        // Add Dependencies
        builder.Services.AddInfrastructureDependencies();
        
        // Add Decorators
        builder.Services.ApplyDecorators();
        
        // Add RabbitMqMessaging Service
        builder.Services.AddRabbitMqMessaging(builder.Configuration);
        
        // Add Identity Settings
        builder.Services.AddIdentityServices();
        
        // Add JWT Authentication Settings
        builder.Services.AddJwtAuthentication(builder.Configuration);
        
        // Add Health Checks Service
        builder.Services.AddHealthChecks();

        // Add services to the container.
        builder.Services.AddAuthorization();
        
        // Add Swagger
        builder.Services.AddSwagger();
        
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

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

        app.UseHttpsRedirection();
        app.UseMiddleware<ErrorHandlerMiddleware>();
        
        app.MapHealthChecks("/health");
        app.UseAuthorization();

        // Map all endpoints
        app.MapEndpoints();
        // When route not found return 404 Not Found
        app.Map404NotFoundEndpoints();
        await app.RunAsync();
    }
}