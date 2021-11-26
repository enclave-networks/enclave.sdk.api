namespace Enclave.Sdk.Api.Data.PatchModel;

/// <summary>
/// The patch model for a DNS record. Any values not provided will not be updated.
/// </summary>
public class DnsRecordPatch : IPatchModel
{
    /// <summary>
    /// The name of the record (without the zone).
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// The set of tags to which this DNS name is applied.
    /// </summary>
    public IReadOnlyList<string>? Tags { get; set; }

    /// <summary>
    /// The set of systems to which this DNS name is applied.
    /// </summary>
    public IReadOnlyList<string>? Systems { get; set; }

    /// <summary>
    /// Any notes or additional info for this record.
    /// </summary>
    public string? Notes { get; set; }
}