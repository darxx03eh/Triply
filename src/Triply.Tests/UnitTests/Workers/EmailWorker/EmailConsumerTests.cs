using EmailWorker;
using EmailWorker.Contracts;
using EmailWorker.Contracts.Enums;
using EmailWorker.IProviders;
using EmailWorker.ISenders;
using MessageQueue.IRabbitMQ;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Triply.Tests.UnitTests.Workers.EmailWorker;

public class EmailConsumerTests
{
    private readonly Mock<IMessageConsumer> _consumer = new();
    private readonly Mock<IEmailTemplateProvider> _templates = new();
    private readonly Mock<IEmailSender> _sender = new();
    private Func<EmailMessage, string, CancellationToken, Task> _handle = null!;

    public EmailConsumerTests()
    {
        _consumer.Setup(c => 
                c.SubscribeAsync("email.send", 
                    It.IsAny<Func<EmailMessage, string, CancellationToken, Task>>(), 
                    It.IsAny<CancellationToken>()))
            .Callback<string, Func<EmailMessage, string, CancellationToken, Task>, CancellationToken>((_, handle, _) => _handle = handle)
            .Returns(Task.CompletedTask);
        _templates.Setup(t => t.RenderAsync(EmailType.ConfirmationEmail, 
                It.IsAny<Dictionary<string, string>>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(("Subject", "<p>Body</p>"));
        var worker = new EmailConsumerHostedService(_consumer.Object, _templates.Object, _sender.Object,
            NullLogger<EmailConsumerHostedService>.Instance);
        worker.StartAsync(CancellationToken.None).GetAwaiter().GetResult();
        worker.ExecuteTask!.GetAwaiter().GetResult();
    }

    [Fact]
    public async Task Handle_RendersTemplateAndSendsToRecipient()
    {
        var data = new Dictionary<string, string> { ["user_name"] = "mahmoud" };

        await _handle(new EmailMessage 
            { Type = EmailType.ConfirmationEmail, To = "m@triply.com", TemplateData = data }, 
            "email.send", CancellationToken.None);

        _templates.Verify(t => t.RenderAsync(EmailType.ConfirmationEmail, data, 
            It.IsAny<CancellationToken>()), Times.Once);
        _sender.Verify(s => s.SendAsync("m@triply.com", "Subject", "<p>Body</p>", 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_SenderFails_ThrowsSoTheMessageIsRetried()
    {
        _sender.Setup(s => s.SendAsync(It.IsAny<string>(), It.IsAny<string>(), 
                It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("smtp down"));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handle(new EmailMessage { Type = EmailType.ConfirmationEmail, To = "m@triply.com" }, 
                "email.send", CancellationToken.None));
    }
}
