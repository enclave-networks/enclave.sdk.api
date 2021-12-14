namespace Enclave.Sdk.Api.Data.Dns;

/// <summary>
/// The model for creating a new zone.
/// </summary>
public class DnsZoneCreate
{
    /// <summary>
    /// The name of the zone.
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// Any notes for the zone.
    /// </summary>
    public string? Notes { get; set; }
}
