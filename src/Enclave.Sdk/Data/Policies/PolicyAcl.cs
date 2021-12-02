namespace Enclave.Sdk.Api.Data.Policies;

/// <summary>
/// Model representing a single ACL entry for a policy.
/// </summary>
public class PolicyAcl
{
    /// <summary>
    /// The protocol.
    /// </summary>
    public PolicyAclProtocol Protocol { get; init; }

    /// <summary>
    /// The port range (or single port) for the ACL.
    /// </summary>
    public string? Ports { get; init; }

    /// <summary>
    /// An optional description.
    /// </summary>
    public string? Description { get; init; }
}
