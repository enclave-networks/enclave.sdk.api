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
    public List<string> SenderTags { get; set; } = new List<string>();

    /// <summary>
    /// A set of receiver tags.
    /// </summary>
    public List<string> ReceiverTags { get; set; } = new List<string>();

    /// <summary>
    /// The set of ACLs for the policy.
    /// </summary>
    public List<PolicyAcl> Acls { get; set; } = new List<PolicyAcl>();

    /// <summary>
    /// Optional notes for the policy.
    /// </summary>
    public string? Notes { get; set; }
}
