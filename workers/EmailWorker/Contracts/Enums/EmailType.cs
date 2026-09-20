namespace EmailWorker.Contracts.Enums;

/// <summary>The email type values.</summary>
public enum EmailType : byte
{
    /// <summary>The confirmation email.</summary>
    ConfirmationEmail = 0,
    /// <summary>The forgot password.</summary>
    ForgotPassword = 1,
    /// <summary>The booking confirmation.</summary>
    BookingConfirmation = 2
}