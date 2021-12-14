namespace Enclave.Sdk.Api.Data.Dns;

/// <summary>
/// Model representing a summary of a DNS record.
/// </summary>
public class DnsZoneSummary
{
    /// <summary>
    /// The ID of the zone.
    /// </summary>
    public DnsZoneId Id { get; init; }

    /// <summary>
    /// The name of the zone.
    /// </summary>
    public string Name { get; init; } = default!;

    /// <summary>
    /// The point in time this zone was created.
    /// </summary>
    public DateTime Created { get; init; }

    /// <summary>
    /// The number of records in the zone.
    /// </summary>
    public int RecordCount { get; init; }

    /// <summary>
    /// The record counts broken down by type.
    /// </summary>
    // Public-facing APIs use the display name of the record type rather than the actual enum name. SeeDnsRecordTypeFormatConverter
    public IReadOnlyDictionary<string, int>? RecordTypeCounts { get; init; }
}
