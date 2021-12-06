using Enclave.Sdk.Api.Data.EnrolmentKeys.Enum;
using System.ComponentModel;

namespace Enclave.Sdk.Api.Data.EnrolmentKeys;

/// <summary>
/// Data required to create a new enrolment key.
/// </summary>
public class EnrolmentKeyCreate
{

    /// <summary>
    /// Defines the type of key being created, general purpose (default) or ephemeral (enrolled systems are automatically removed).
    /// </summary>
    [DefaultValue(EnrolmentKeyType.GeneralPurpose)]
    public EnrolmentKeyType Type { get; set; } = EnrolmentKeyType.GeneralPurpose;

    /// <summary>
    /// The approval mode for the key, manual (default) or automatic.
    /// </summary>
    [DefaultValue(Enum.ApprovalMode.Manual)]
    public ApprovalMode? ApprovalMode { get; set; } = Enum.ApprovalMode.Manual;

    /// <summary>
    /// A description for the key you are creating.
    /// </summary>
    public string Description { get; set; } = default!;

    /// <summary>
    /// The number of uses to start the key with. A value of -1 indicates no limit on the number of uses.
    /// </summary>
    [DefaultValue(-1)]
    public int? UsesRemaining { get; set; } = -1;

    /// <summary>
    /// The set of IP Address constraints on the key.
    /// </summary>
    public List<EnrolmentKeyIpConstraintInput>? IpConstraints { get; set; } = new List<EnrolmentKeyIpConstraintInput>();

    /// <summary>
    /// A set of tags automatically applied to systems enrolled with this key.
    /// </summary>
    public List<string>? Tags { get; set; } = new List<string>();

    /// <summary>
    /// Defines the number of minutes an ephemeral system enrolled with this key will be retained after a non-graceful disconnect.
    /// Only valid when key type is 'Ephemeral'. Default of 15 minutes.
    /// </summary>
    public int? DisconnectedRetentionMinutes { get; set; }

    /// <summary>
    /// Any additional notes to attach to the key.
    /// </summary>
    public string? Notes { get; set; }
}
