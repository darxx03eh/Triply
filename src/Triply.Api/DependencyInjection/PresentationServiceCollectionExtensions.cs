using System.Reflection;
using Microsoft.OpenApi;
using Triply.Api.Responses;
using Triply.Application.Interfaces.Services;
using Triply.Infrastructure.Services.Blacklist;

namespace Triply.Api.DependencyInjection;

public static class PresentationServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
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
                                  Supports user authentication, profile management, 
                                  and hotel search.
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
                c.CustomSchemaIds(type => type.FullName);
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                    c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
                c.OrderActionsBy(apiDesc => apiDesc.RelativePath);
            });
            return services;
        }

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
    }
    extension(IEndpointRouteBuilder app)
    {
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
    }
}