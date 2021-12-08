using System.ComponentModel;

namespace Enclave.Sdk.Api.Data.Dns;

/// <summary>
/// Record for creating a DNS model.
/// </summary>
public class DnsRecordCreate
{
    /// <summary>
    /// The name of the record to create. Use '@' for an apex record in the zone.
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// The ID of the DNS zone this record should be added to.
    /// </summary>
    public DnsZoneId ZoneId { get; set; }

    /// <summary>
    /// The type of the DNS record (currently only "ENCLAVE" is supported).
    /// </summary>
    [DefaultValue("ENCLAVE")]
    public string? Type { get; set; }

    /// <summary>
    /// The set of tags this name should apply to. All systems with one of these tags will get the name.
    /// </summary>
    public IReadOnlyList<string>? Tags { get; set; }

    /// <summary>
    /// The set of systems to directly apply this name to.
    /// </summary>
    public IReadOnlyList<string>? Systems { get; set; }

    /// <summary>
    /// Any notes or additional info for this record.
    /// </summary>
    public string? Notes { get; set; }
}
