namespace EmailWorker.ISenders;

/// <summary>Defines the email sender operations.</summary>
public interface IEmailSender
{
    /// <summary>Sends an email.</summary>
    Task SendAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default);
}