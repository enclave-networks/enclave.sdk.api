namespace Enclave.Sdk.Api.Data.Policies;

/// <summary>
/// Defines the known protocols enforced in policy ACLs.
/// </summary>
public enum PolicyAclProtocol
{
    Any,
    Tcp,
    Udp,
    Icmp
}
