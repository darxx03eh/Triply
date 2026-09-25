using MessageQueue.IRabbitMQ;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Triply.Api;
using Triply.Application.Interfaces.Services;
using Triply.Infrastructure.DependencyInjection;
using Triply.Infrastructure.Db;

namespace Triply.Tests.IntegrationTests.Infrastructure;

/// <summary>
/// Hosts the real API in TestServer while replacing external infrastructure.
/// Integration tests use a SQL Server database reachable from the host.
/// </summary>
public sealed class TriplyWebApplicationFactory : WebApplicationFactory<Program>
{
    private static readonly string TestConnectionString =
        Environment.GetEnvironmentVariable("ConnectionStrings__TriplyTestDbLocalConnection")
        ?? throw new InvalidOperationException(
            "Environment variable 'ConnectionStrings__TriplyTestDbLocalConnection' is not set.");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_, configuration) => configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["JwtSettings:Issuer"] = "triply-integration-tests",
            ["JwtSettings:Audience"] = "triply-integration-tests",
            ["JwtSettings:SecretKey"] = "integration-tests-use-a-long-enough-signing-key",
            ["JwtSettings:ValidateIssuer"] = "true",
            ["JwtSettings:ValidateAudience"] = "true",
            ["JwtSettings:ValidateLifetime"] = "true",
            ["JwtSettings:ValidateIssuerSigningKey"] = "true",
            ["JwtSettings:ExpiryMinutes"] = "60",
            ["JwtSettings:RefreshTokenExpiryDays"] = "7",
            ["PaymentOptions:Provider"] = "Mock",
            ["PaymentOptions:Currency"] = "usd",
            ["BookingOptions:PendingExpiryMinutes"] = "45",
            ["BookingOptions:ExpiryCheckIntervalMinutes"] = "5",
            ["Elasticsearch:Enabled"] = "false"
        }));

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<TriplyDbContext>();
            services.RemoveAll<DbContextOptions<TriplyDbContext>>();
            services.RemoveAll<DbContextOptions>();
            services.RemoveAll<IDbContextOptionsConfiguration<TriplyDbContext>>();
            services.AddDbContext<TriplyDbContext>(options =>
                options.UseSqlServer(TestConnectionString));

            services.RemoveAll<ITokenBlacklistService>();
            services.AddSingleton<ITokenBlacklistService, InMemoryTokenBlacklistService>();

            services.RemoveAll<IMessagePublisher>();
            services.AddSingleton<IMessagePublisher, InMemoryMessagePublisher>();
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        using var scope = host.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TriplyDbContext>();
        context.Database.Migrate();
        host.Services.SeedAsync().GetAwaiter().GetResult();

        return host;
    }
}
