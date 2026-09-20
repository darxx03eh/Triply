using EmailWorker.IProviders;
using EmailWorker.ISenders;
using EmailWorker.Options;
using EmailWorker.Providers;
using EmailWorker.Senders;
using Logging.DependencyInjection;
using MessageQueue.DependencyInjection;

namespace EmailWorker;
/// <summary>Represents the program.</summary>
public static class Program
{
    /// <summary>Mains.</summary>
    public static void Main(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

        builder.AddTriplyLogging("email-worker");

        builder.Services.AddRabbitMqMessaging(builder.Configuration);
        builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("Smtp"));
        builder.Services.AddSingleton<IEmailTemplateProvider, ScribanEmailTemplateProvider>();
        builder.Services.AddSingleton<IEmailSender, SmtpEmailSender>();
        builder.Services.AddHostedService<EmailConsumerHostedService>();

        IHost host = builder.Build();

        host.Run();
    }
}