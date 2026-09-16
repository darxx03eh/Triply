using EmailWorker.Contracts.Enums;

namespace EmailWorker.Contracts;

public class EmailMessage
{
    public EmailType Type { get; set; }
    public string To { get; set; }
    public Dictionary<string, string> TemplateData { get; set; } = new();
}