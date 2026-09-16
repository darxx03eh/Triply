using EmailWorker.Contracts.Enums;

namespace EmailWorker.IProviders;

public interface IEmailTemplateProvider
{
    Task<(string Subject, string HtmlBody)> RenderAsync(EmailType type, Dictionary<string, string> data,
        CancellationToken cancellationToken = default);
}