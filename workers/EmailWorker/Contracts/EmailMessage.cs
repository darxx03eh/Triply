using EmailWorker.Contracts.Enums;

namespace EmailWorker.Contracts;

/// <summary>Message payload published for the email.</summary>
public class EmailMessage
{
    /// <summary>Gets or sets the type.</summary>
    public EmailType Type { get; set; }
    /// <summary>Gets or sets the to.</summary>
    public string To { get; set; }
    /// <summary>Gets or sets the template data.</summary>
    public Dictionary<string, string> TemplateData { get; set; } = new();
}