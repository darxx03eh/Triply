using Triply.Domain.Contracts.Enums;

namespace Triply.Domain.Contracts;

/// <summary>Message payload used to request an email from the worker.</summary>
public class EmailMessage
{
    /// <summary>Gets or sets the email template type.</summary>
    public EmailType Type { get; set; }
    /// <summary>Gets or sets the recipient address.</summary>
    public string To { get; set; }
    /// <summary>Gets template placeholder values.</summary>
    public Dictionary<string, string> TemplateData { get; set; } = new();
}