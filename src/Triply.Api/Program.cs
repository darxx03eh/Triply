using Triply.Infrastructure.DependencyInjection;

namespace Triply.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // Add Car Rental DbContext
        builder.Services.AddTriplyDbContext(builder.Configuration);
        
        // Add Dependencies
        builder.Services.AddInfrastructureDependencies();
        
        // Add Identity Settings
        builder.Services.AddIdentityServices();
        
        // Add JWT Authentication Settings
        builder.Services.AddJwtAuthentication(builder.Configuration);
        
        // Add Health Checks Service
        builder.Services.AddHealthChecks();

        // Add services to the container.
        builder.Services.AddAuthorization();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var app = builder.Build();

        await app.Services.SeedAsync();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();
        app.MapHealthChecks("/health");
        app.UseAuthorization();

        await app.RunAsync();
    }
}