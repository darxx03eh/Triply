using System.Reflection;
using Microsoft.OpenApi;
using Triply.Api.Endpoints;
using Triply.Api.Responses;
using Triply.Application.Interfaces.Services;
using Triply.Infrastructure.Services.Blacklist;

namespace Triply.Api.DependencyInjection;

/// <summary>Extension methods for presentation service collection.</summary>
public static class ApiServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>Registers the swagger services.</summary>
        public IServiceCollection AddSwagger()
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Version = "v1",
                    Title = "Triply API",
                    Description = """
                                  ASP.NET Core API for a Travel and Accommodation Booking Platform 
                                  Web Application.
                                  Supports user authentication and hotel search.
                                  """,
                    Contact = new OpenApiContact()
                    {
                        Name = "Support Team",
                        Email = "support@triply.com"
                    }
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    In = ParameterLocation.Header,
                    Description = "enter JWT token like this: Bearer {token}"
                });
                c.AddSecurityRequirement(document => new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecuritySchemeReference("Bearer", document),
                        new List<string>()
                    }
                });
                c.CustomSchemaIds(SchemaId);
                // The XML docs of every Triply assembly, so the schemas show the DTO comments too.
                foreach (var xmlPath in Directory.GetFiles(AppContext.BaseDirectory, "Triply.*.xml"))
                    c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
                c.OrderActionsBy(apiDesc => apiDesc.RelativePath);
            });
            return services;
        }

        /// <summary>Registers the Redis service services.</summary>
        public IServiceCollection AddRedisService(IConfiguration configuration)
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration["Redis:ConnectionString"];
                options.InstanceName = "hotelbooking:";
            });

            services.AddScoped<ITokenBlacklistService, RedisTokenBlacklistService>();
            return services;
        }

        /// <summary>Registers the frontend CORS policy.</summary>
        public IServiceCollection AllowFrontendCors(IConfiguration configuration)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("FrontendCORSPolicy", policy =>
                {
                    policy.WithOrigins(configuration.GetValue<string>("FrontendUrl"))
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                        .WithExposedHeaders("Content-Type", "Authorization", "Content-Length");
                });
            });
            return services;
        }
    }
    extension(IEndpointRouteBuilder app)
    {
        /// <summary>Maps the 404 not found endpoints.</summary>
        public void Map404NotFoundEndpoints()
        {
            app.MapFallback(async context =>
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                context.Response.ContentType = "application/json";
                var response = new ApiResponse<object>
                {
                    Message = ApiResponseMessages.Routing.EndpointNotFound.Message,
                    Code = ApiResponseMessages.Routing.EndpointNotFound.Code,
                    Errors = new ApiErrors
                    {
                        Fields = [],
                        General = []
                    }
                };
                await context.Response.WriteAsJsonAsync(response);
            });
        }

        /// <summary>Maps the endpoints.</summary>
        public void MapEndpoints()
        {
            app.MapAuthenticationEndpoints();
            app.MapCityEndpoints();
            app.MapHotelEndpoints();
            app.MapRoomEndpoints();
            app.MapAmenityEndpoints();
            app.MapSearchEndpoints();
            app.MapAttractionEndpoints();
            app.MapReviewEndpoints();
            app.MapDealEndpoints();
            app.MapHomeEndpoints();
            app.MapCartEndpoints();
            app.MapBookingEndpoints();
            app.MapPaymentEndpoints();
        }
    }

    /// <summary>Builds a readable schema id, so ApiResponse&lt;HotelResponse&gt; does not become a mangled generic name.</summary>
    private static string SchemaId(Type type)
        => type.IsGenericType
            ? $"{type.FullName!.Split('`')[0]}Of{string.Join("And", type.GetGenericArguments().Select(SchemaId))}"
            : type.FullName!;
}
