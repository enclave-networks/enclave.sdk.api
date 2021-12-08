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
    public IReadOnlyList<string>? SenderTags { get; set; }

    /// <summary>
    ///  A set of receiver tags.
    /// </summary>
    public IReadOnlyList<string>? ReceiverTags { get; set; }

    /// <summary>
    /// The set of ACLs for the policy.
    /// </summary>
    public IReadOnlyList<PolicyAcl>? Acls { get; set; }

    /// <summary>
    /// Notes for the policy.
    /// </summary>
    public string? Notes { get; set; }
}
