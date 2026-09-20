using EmailWorker.Contracts.Enums;

namespace EmailWorker.IProviders;

/// <summary>Defines the email template provider operations.</summary>
public interface IEmailTemplateProvider
{
    /// <summary>Renders a specific email template from templates.</summary>
    Task<(string Subject, string HtmlBody)> RenderAsync(EmailType type, Dictionary<string, string> data,
        CancellationToken cancellationToken = default);
}