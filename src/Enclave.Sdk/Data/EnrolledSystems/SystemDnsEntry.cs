using Enclave.Sdk.Api.Data.Dns;
using Enclave.Sdk.Api.Data.Tags;

namespace Enclave.Sdk.Api.Data.EnrolledSystems;

/// <summary>
/// Represents a single system DNS entry.
/// </summary>
public class SystemDnsEntry
{
    /// <summary>
    /// The FQDN (Fully Qualified Domain Name) of the entry.
    /// </summary>
    public string Fqdn { get; init; } = default!;

    /// <summary>
    /// The zone ID the DNS entry came from.
    /// </summary>
    public DnsZoneId? FromZoneId { get; init; }

    /// <summary>
    /// The zone Name the DNS entry came from.
    /// </summary>
    public string? FromZoneName { get; init; }

    /// <summary>
    /// The DNS record ID the DNS entry came from.
    /// </summary>
    public DnsRecordId? FromRecordId { get; init; }

    /// <summary>
    /// The DNS record name the DNS entry came from.
    /// </summary>
    public string? FromRecordName { get; init; }

    /// <summary>
    /// Indicates whether the DNS entry was directly assigned to this specific system.
    /// </summary>
    public bool FromDirectAssignment { get; init; }

    /// <summary>
    /// Contains the set of tags on the system that caused it to be assigned this name.
    /// </summary>
    public IReadOnlyList<TagReference>? Tags { get; init; }
}
