using System.Reflection;
using EmailWorker.Contracts.Enums;
using EmailWorker.IProviders;
using Scriban;
using Scriban.Runtime;

namespace EmailWorker.Providers;

public class ScribanEmailTemplateProvider : IEmailTemplateProvider
{
    private static readonly Dictionary<EmailType, (string File, string Subject)> _templateMap = new()
    {
        [EmailType.ConfirmationEmail] = ("confirmation-email.html", "Confirm your account"),
        [EmailType.ForgotPassword] = ("forgot-password.html", "Reset your password"),
        [EmailType.BookingConfirmation] = ("booking-confirmation.html", "Booking Confirmed - {{ hotel_name }}")
    };

    public async Task<(string Subject, string HtmlBody)> RenderAsync(EmailType type, Dictionary<string, string> data,
        CancellationToken cancellationToken = default)
    {
        if (!_templateMap.TryGetValue(type, out var info))
            throw new InvalidOperationException($"No template registered for email type '{type}'.");

        string rawBody = await LoadEmbeddedTemplateAsync(info.File);

        var scriptObject = new ScriptObject();
        foreach (var kv in data)
            scriptObject.Add(kv.Key, kv.Value);

        var context = new TemplateContext();
        context.PushGlobal(scriptObject);

        string body = await Template.Parse(rawBody).RenderAsync(context);
        string subject = await Template.Parse(info.Subject).RenderAsync(context);

        return (subject, body);
    }
    
    private static async Task<string> LoadEmbeddedTemplateAsync(string fileName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        string? resourceName = assembly.GetManifestResourceNames()
            .FirstOrDefault(n => n.EndsWith(fileName, StringComparison.OrdinalIgnoreCase));

        if (resourceName is null)
            throw new FileNotFoundException($"Template '{fileName}' not found as an embedded resource.");

        await using var stream = assembly.GetManifestResourceStream(resourceName)!;
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }
}