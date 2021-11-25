using Enclave.Sdk.Api.Data.Policy;

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
    public string[]? SenderTags { get; set; }

    /// <summary>
    ///  A set of receiver tags.
    /// </summary>
    public string[]? ReceiverTags { get; set; }

    /// <summary>
    /// The set of ACLs for the policy.
    /// </summary>
    public PolicyAclModel[]? Acls { get; set; }

    /// <summary>
    /// Notes for the policy.
    /// </summary>
    public string? Notes { get; set; }
}
