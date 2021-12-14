using Enclave.Sdk.Api.Data.Logging.Enum;

namespace Enclave.Sdk.Api.Data.Logging;

/// <summary>
/// Model for a single log entry.
/// </summary>
public class LogEntry
{
    /// <summary>
    /// The UTC timestamp of the log event.
    /// </summary>
    public DateTime TimeStamp { get; set; }

    /// <summary>
    /// The log level.
    /// </summary>
    public ActivityLogLevel Level { get; set; }

    /// <summary>
    /// The log message.
    /// </summary>
    public string Message { get; set; } = default!;

    /// <summary>
    /// The user responsible for the event (if known).
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// The IP address from which the action was carried out.
    /// </summary>
    public string? IpAddress { get; set; }
}