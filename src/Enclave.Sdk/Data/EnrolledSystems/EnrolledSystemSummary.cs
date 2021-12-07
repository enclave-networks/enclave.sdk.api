using Enclave.Sdk.Api.Data.EnrolledSystems.Enum;
using Enclave.Sdk.Api.Data.Tags;

namespace Enclave.Sdk.Api.Data.EnrolledSystems;

/// <summary>
/// A basic model representing a single system.
/// </summary>
public class EnrolledSystemSummary
{
    /// <summary>
    /// Unique ID for the System.
    /// </summary>
    public SystemId SystemId { get; init; } = default!;

    /// <summary>
    /// The configured description of the system.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// The type of the system.
    /// </summary>
    public SystemType Type { get; init; }

    /// <summary>
    /// A value indicating the current state of the system from Enclave's perspective.
    /// </summary>
    public SystemState State { get; init; }

    /// <summary>
    /// Indicates a UTC timestamp when the system connected to the Enclave SaaS.
    /// </summary>
    public DateTime? ConnectedAt { get; init; }

    /// <summary>
    /// Indicates a UTC timestamp when Enclave last interacted with the system.
    /// </summary>
    public DateTime? LastSeen { get; init; }

    /// <summary>
    /// Contains a timestamp indicating when the system was enrolled into the account.
    /// </summary>
    public DateTime EnrolledAt { get; init; }

    /// <summary>
    /// The ID of the enrolment key used to enrol the system.
    /// </summary>
    public int EnrolmentKeyId { get; init; }

    /// <summary>
    /// The description of the enrolment key used to enrol the system.
    /// </summary>
    public string EnrolmentKeyDescription { get; init; } = default!;

    /// <summary>
    /// Whether or not the system is enabled.
    /// </summary>
    public bool IsEnabled { get; init; }

    /// <summary>
    /// The IP address the system is connected from.
    /// </summary>
    public string? ConnectedFrom { get; init; }

    /// <summary>
    /// The locally-defined host name of the system.
    /// </summary>
    public string? Hostname { get; init; }

    /// <summary>
    /// The platform type for this system; possible values are Windows, Linux or Darwin.
    /// </summary>
    public string? PlatformType { get; init; }

    /// <summary>
    /// The version of the operating system.
    /// </summary>
    public string? OSVersion { get; init; }

    /// <summary>
    /// The Enclave product version.
    /// </summary>
    public string? EnclaveVersion { get; init; }

    /// <summary>
    /// The tags assigned to the system.
    /// </summary>
    public IReadOnlyList<TagReference>? Tags { get; init; }

    /// <summary>
    /// Defines the number of minutes this system will be retained after a non-graceful disconnect.
    /// </summary>
    public int? DisconnectedRetentionMinutes { get; init; }
}
