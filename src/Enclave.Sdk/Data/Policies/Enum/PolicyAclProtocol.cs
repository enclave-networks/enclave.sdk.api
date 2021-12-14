namespace Enclave.Sdk.Api.Data.Policies.Enum;

/// <summary>
/// Defines the known protocols enforced in policy ACLs.
/// </summary>
public enum PolicyAclProtocol
{
    /// <summary>
    /// Any Protocol.
    /// </summary>
    Any,

    /// <summary>
    /// Tcp.
    /// </summary>
    Tcp,

    /// <summary>
    /// Udp.
    /// </summary>
    Udp,

    /// <summary>
    /// Icmp.
    /// </summary>
    Icmp,
}
