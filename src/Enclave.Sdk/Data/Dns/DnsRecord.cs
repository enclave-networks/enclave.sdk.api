using Enclave.Sdk.Api.Data.SystemManagement;
using Enclave.Sdk.Api.Data.Tags;

namespace Enclave.Sdk.Api.Data.Dns;

/// <summary>
/// Detailed model representing a DNS record.
/// </summary>
public class DnsRecord
{
    /// <summary>
    /// The ID of the record.
    /// </summary>
    public DnsRecordId Id { get; init; }

    /// <summary>
    /// The name of the record (without the zone).
    /// </summary>
    public string Name { get; init; } = default!;

    /// <summary>
    /// The type of DNS record.
    /// </summary>
    public string Type { get; init; } = default!;

    /// <summary>
    /// The ID of the DNS Zone to which this record belongs.
    /// </summary>
    public DnsZoneId ZoneId { get; init; }

    /// <summary>
    /// The name of the DNS Zone to which this record belongs.
    /// </summary>
    public string ZoneName { get; init; } = default!;

    /// <summary>
    /// The fully-qualified domain name of the record, including the zone name.
    /// </summary>
    public string Fqdn { get; init; } = default!;

    /// <summary>
    /// The set of tags to which this DNS name is applied.
    /// </summary>
    public IReadOnlyList<TagReference> Tags { get; init; } = Array.Empty<TagReference>();

    /// <summary>
    /// The set of systems to which this DNS name is applied.
    /// </summary>
    public IReadOnlyList<SystemReference>? Systems { get; init; }

    /// <summary>
    /// Any provided notes for this record.
    /// </summary>
    public string? Notes { get; init; }
}
