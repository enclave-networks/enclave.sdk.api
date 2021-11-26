namespace Enclave.Sdk.Api.Data.Policy;

/// <summary>
/// Model representing a single ACL entry for a policy.
/// </summary>
public class PolicyAclModel
{
    /// <summary>
    /// The protocol.
    /// </summary>
    public PolicyAclProtocol Protocol { get; set; }

    /// <summary>
    /// The port range (or single port) for the ACL.
    /// </summary>
    public string? Ports { get; set; }

    /// <summary>
    /// An optional description.
    /// </summary>
    public string? Description { get; set; }
}
