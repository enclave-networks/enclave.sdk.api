using Enclave.Sdk.Api.Data.Tags;

namespace Enclave.Sdk.Api.Data.UnaprrovedSystems;

/// <summary>
/// Model representing an unapproved system.
/// </summary>
public class UnapprovedSystem
{
    /// <summary>
    /// The system ID.
    /// </summary>
    public string SystemId { get; init; } = default!;

    /// <summary>
    /// The system type.
    /// </summary>
    public SystemType Type { get; init; }

    /// <summary>
    /// The system description (if one has been provided).
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// The IP address the system was enrolled from.
    /// </summary>
    public string EnrolledFrom { get; init; } = default!;

    /// <summary>
    /// The UTC timestamp when the system was enrolled.
    /// </summary>
    public DateTime EnrolledAt { get; init; }

    /// <summary>
    /// The set of tags assigned to the system (either manually or automatically from the key).
    /// </summary>
    public IReadOnlyList<TagReference>? Tags { get; init; }

    /// <summary>
    /// The ID of the enrolment key used to enrol the system.
    /// </summary>
    public int EnrolmentKeyId { get; init; }

    /// <summary>
    /// The description of the enrolment key used to enrol the system.
    /// </summary>
    public string? EnrolmentKeyDescription { get; init; }

    /// <summary>
    /// The locally-defined host name of the system.
    /// </summary>
    public string? Hostname { get; init; }

    /// <summary>
    /// The platform type for this system; possible values are Windows, Linux or MacOSX.
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
    /// The IP address the system is connected from.
    /// </summary>
    public string? ConnectedFrom { get; init; }
}
