using EmailWorker.IProviders;
using EmailWorker.ISenders;
using EmailWorker.Options;
using EmailWorker.Providers;
using EmailWorker.Senders;
using MessageQueue.DependencyInjection;

namespace EmailWorker;
public static class Program
{
    public static void Main(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

        builder.Services.AddRabbitMqMessaging(builder.Configuration);
        builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("Smtp"));
        builder.Services.AddSingleton<IEmailTemplateProvider, ScribanEmailTemplateProvider>();
        builder.Services.AddSingleton<IEmailSender, SmtpEmailSender>();
        builder.Services.AddHostedService<EmailConsumerHostedService>();

        IHost host = builder.Build();

        host.Run();
    }
}