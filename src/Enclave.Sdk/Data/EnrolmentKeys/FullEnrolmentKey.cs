using Enclave.Sdk.Api.Data.EnrolmentKeys.Enum;
using Enclave.Sdk.Api.Data.Tags;

namespace Enclave.Sdk.Api.Data.EnrolmentKeys;

/// <summary>
/// Represents a single Enclave Enrolment Key.
/// </summary>
public class FullEnrolmentKey
{
    /// <summary>
    /// The ID of the enrolment key.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// The UTC timestamp when the key was created.
    /// </summary>
    public DateTime Created { get; init; }

    /// <summary>
    /// The UTC timestamp when the key was last used to enrol a system (null if never used).
    /// </summary>
    public DateTime? LastUsed { get; init; }

    /// <summary>
    /// The type of key; general purpose (default) or ephemeral (enrolled systems are automatically removed).
    /// </summary>
    public EnrolmentKeyType Type { get; init; }

    /// <summary>
    /// The approval mode for the key.
    /// </summary>
    public ApprovalMode ApprovalMode { get; init; }

    /// <summary>
    /// The status of the key.
    /// </summary>
    public EnrolmentKeyStatus Status => IsEnabled ?
            UsesRemaining == 0 ?
             EnrolmentKeyStatus.NoUsesRemaining : EnrolmentKeyStatus.Enabled
        : EnrolmentKeyStatus.Disabled;

    /// <summary>
    /// The key value that can be used to enrol systems.
    /// </summary>
    public string Key { get; init; }

    /// <summary>
    /// The provided description of the key.
    /// </summary>
    public string Description { get; init; }

    /// <summary>
    /// Whether or not this key is enabled.
    /// </summary>
    public bool IsEnabled { get; init; }

    /// <summary>
    /// The number of uses remaining.
    /// </summary>
    public long UsesRemaining { get; init; }

    /// <summary>
    /// The number of systems enrolled with this key.
    /// </summary>
    public long EnrolledCount { get; init; }

    /// <summary>
    /// The number of unapproved systems enrolled with this key.
    /// </summary>
    public long UnapprovedCount { get; init; }

    /// <summary>
    /// The set of tags applied to the key.
    /// </summary>
    public IReadOnlyList<TagReference> Tags { get; init; }

    /// <summary>
    /// Defines the number of minutes an ephemeral system enrolled with this key will be retained after a non-graceful disconnect.
    /// Only has a value when the type is 'Ephemeral'.
    /// </summary>
    public int? DisconnectedRetentionMinutes { get; set; }

    /// <summary>
    /// The set of IP constraints for the key.
    /// </summary>
    public IReadOnlyList<EnrolmentKeyIpConstraint> IpConstraints { get; init; } = Array.Empty<EnrolmentKeyIpConstraint>();

    /// <summary>
    /// Any notes or additional info for this key.
    /// </summary>
    public string? Notes { get; init; }
}