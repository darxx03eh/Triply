namespace Triply.Infrastructure.Settings;

/// <summary>Configuration settings of database logging.</summary>
public class DatabaseLoggingSettings
{
    /// <summary>Queries slower than this are logged as warnings.</summary>
    public int SlowQueryThresholdMs { get; set; } = 500;
    /// <summary>Shows the parameter values in the logged SQL. Development only, it can expose user data.</summary>
    public bool EnableSensitiveDataLogging { get; set; }
}
