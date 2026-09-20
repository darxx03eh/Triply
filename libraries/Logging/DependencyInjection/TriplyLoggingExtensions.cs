using Elastic.CommonSchema.Serilog;
using Elastic.Ingest.Elasticsearch;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using Logging.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Logging.DependencyInjection;

/// <summary>Extension methods for Triply logging.</summary>
public static class TriplyLoggingExtensions
{
    private const string ConsoleTemplate =
        "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}{NewLine}      {Message:lj}{NewLine}{Exception}";

    /// <summary>Console logger used until the host is built, so startup failures are still written.</summary>
    public static void CreateBootstrapLogger()
        => Log.Logger = new LoggerConfiguration()
            .WriteTo.Console(outputTemplate: ConsoleTemplate)
            .CreateBootstrapLogger();

    extension(IHostApplicationBuilder builder)
    {
        /// <summary>
        /// Replaces the default logging with Serilog: levels from the "Serilog" section,
        /// console output and the Elasticsearch data stream shown in Kibana.
        /// </summary>
        public IHostApplicationBuilder AddTriplyLogging(string serviceName)
        {
            var elasticsearch = builder.Configuration.GetSection("Elasticsearch").Get<ElasticsearchOptions>()
                                ?? new ElasticsearchOptions();

            builder.Services.AddSerilog((services, logger) =>
            {
                logger.ReadFrom.Configuration(builder.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .Enrich.WithEnvironmentName()
                    .Enrich.WithThreadId()
                    .Enrich.WithProperty("Application", serviceName)
                    .WriteTo.Console(outputTemplate: ConsoleTemplate);

                if (elasticsearch.Enabled)
                    logger.WriteTo.Elasticsearch([new Uri(elasticsearch.Uri)], options =>
                    {
                        options.DataStream = new DataStreamName(
                            "logs", $"{elasticsearch.DataSetPrefix}.{serviceName.Replace('-', '_')}",
                            builder.Environment.EnvironmentName.ToLowerInvariant());
                        options.BootstrapMethod = BootstrapMethod.Silent;
                        options.TextFormatting = new EcsTextFormatterConfiguration<LogEventEcsDocument>
                        {
                            // user.* is filled from the UserId/UserName of the request log only, so it always
                            // means the application user and not the OS user of the container.
                            IncludeUser = false,
                            MapCustom = (document, _) =>
                            {
                                document.Service ??= new Elastic.CommonSchema.Service();
                                document.Service.Name = serviceName;
                                document.Service.Environment = builder.Environment.EnvironmentName;
                                return document;
                            }
                        };
                    });
            });

            return builder;
        }
    }
}
