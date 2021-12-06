using Enclave.Sdk.Api.Data.Policies;

namespace Enclave.Sdk.Api.Data.PatchModel;

/// <summary>
/// Defines the modifiable properties of a policy.
/// </summary>
public class PolicyPatch : IPatchModel
{
    /// <summary>
    /// The policy name/description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Whether or not the policy is enabled.
    /// </summary>
    public bool? IsEnabled { get; set; }

    /// <summary>
    /// A set of sender tags.
    /// </summary>
    public List<string> SenderTags { get; set; } = new List<string>();

    /// <summary>
    ///  A set of receiver tags.
    /// </summary>
    public List<string> ReceiverTags { get; set; } = new List<string>();

    /// <summary>
    /// The set of ACLs for the policy.
    /// </summary>
    public List<PolicyAcl> Acls { get; set; } = new List<PolicyAcl>();

    /// <summary>
    /// Notes for the policy.
    /// </summary>
    public string? Notes { get; set; }
}
