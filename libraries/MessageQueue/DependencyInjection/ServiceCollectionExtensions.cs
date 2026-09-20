using MessageQueue.IRabbitMQ;
using MessageQueue.Options;
using MessageQueue.RabbitMQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MessageQueue.DependencyInjection;

/// <summary>Extension methods for service collection.</summary>
public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>Registers the RabbitMQ MQ messaging services.</summary>
        public IServiceCollection AddRabbitMqMessaging(IConfiguration configuration, string sectionName = "RabbitMq")
        {
            services.Configure<RabbitMqOptions>(configuration.GetSection(sectionName));

            services.AddSingleton<IMessagePublisher>(sp =>
                new RabbitMqPublisher(sp.GetRequiredService<IOptions<RabbitMqOptions>>(),
                    sp.GetService<ILogger<RabbitMqPublisher>>()));

            services.AddSingleton<IMessageConsumer>(sp =>
                new RabbitMqConsumer(sp.GetRequiredService<IOptions<RabbitMqOptions>>(),
                    sp.GetService<ILogger<RabbitMqConsumer>>()));
            return services;
        }
    }
}