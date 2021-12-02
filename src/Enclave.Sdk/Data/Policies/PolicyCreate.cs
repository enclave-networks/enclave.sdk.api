using System.ComponentModel;

namespace Enclave.Sdk.Api.Data.Policies;

/// <summary>
/// Data required to create a new policy.
/// </summary>
public class PolicyCreate
{
    /// <summary>
    /// A description for the policy you are creating.
    /// </summary>
    public string Description { get; set; } = default!;

    /// <summary>
    /// Whether or not the policy is initially enabled.
    /// </summary>
    [DefaultValue(true)]
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// A set of sender tags.
    /// </summary>
    public string[] SenderTags { get; set; } = Array.Empty<string>();

    /// <summary>
    /// A set of receiver tags.
    /// </summary>
    public string[] ReceiverTags { get; set; } = Array.Empty<string>();

    /// <summary>
    /// The set of ACLs for the policy.
    /// </summary>
    public PolicyAcl[] Acls { get; set; } = Array.Empty<PolicyAcl>();

    /// <summary>
    /// Optional notes for the policy.
    /// </summary>
    public string? Notes { get; set; }
}
