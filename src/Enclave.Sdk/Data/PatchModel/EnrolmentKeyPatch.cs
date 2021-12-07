using Enclave.Sdk.Api.Data.EnrolmentKeys;
using Enclave.Sdk.Api.Data.EnrolmentKeys.Enum;

namespace Enclave.Sdk.Api.Data.PatchModel;

/// <summary>
/// Defines the modifiable properties of an enrolment key.
/// </summary>
public class EnrolmentKeyPatchModel : IPatchModel
{
    /// <summary>
    /// The description for the key.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Whether the key is enabled (meaning it can be used) or disabled.
    /// </summary>
    public bool? IsEnabled { get; set; }

    /// <summary>
    /// The approval mode for the key, manual (default) or automatic.
    /// </summary>
    public ApprovalMode? ApprovalMode { get; set; }

    /// <summary>
    /// The number of uses remaining the key should have. A value of -1 indicates no limit on the number of uses.
    /// </summary>
    public int? UsesRemaining { get; set; }

    /// <summary>
    /// The set of IP Address constraints on the key.
    /// </summary>
    public List<EnrolmentKeyIpConstraintInput>? IpConstraints { get; set; }

    /// <summary>
    /// A set of tags automatically applied to systems enrolled with this key.
    /// </summary>
    public string[]? Tags { get; set; }

    /// <summary>
    /// Defines the number of minutes an ephemeral system enrolled with this key will be retained after a non-graceful disconnect.
    /// Only valid when key type is 'Ephemeral'.
    /// </summary>
    public int? DisconnectedRetentionMinutes { get; set; }

    /// <summary>
    /// Any notes or additional info for this key.
    /// </summary>
    public string? Notes { get; set; }
}
