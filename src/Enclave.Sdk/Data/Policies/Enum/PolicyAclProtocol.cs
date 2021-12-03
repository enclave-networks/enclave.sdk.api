using System.Text.Json.Serialization;

namespace Enclave.Sdk.Api.Data.Policies;

/// <summary>
/// Defines the known protocols enforced in policy ACLs.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PolicyAclProtocol
{
    Any,
    Tcp,
    Udp,
    Icmp
}
