namespace EmailWorker.Options;

/// <summary>Configuration options of SMTP.</summary>
public class SmtpOptions
{
    /// <summary>Gets or sets the host.</summary>
    public string Host { get; set; } = default!;
    /// <summary>Gets or sets the port.</summary>
    public int Port { get; set; } = 587;
    /// <summary>Gets or sets the user name.</summary>
    public string UserName { get; set; } = default!;
    /// <summary>Gets or sets the password.</summary>
    public string Password { get; set; } = default!;
    /// <summary>Gets or sets the from address.</summary>
    public string FromAddress { get; set; } = default!;
    /// <summary>Gets or sets the from name.</summary>
    public string FromName { get; set; } = "Hotel Booking";
    /// <summary>Gets or sets the use ssl.</summary>
    public bool UseSsl { get; set; } = true;
}