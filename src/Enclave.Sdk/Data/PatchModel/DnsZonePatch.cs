namespace Enclave.Sdk.Api.Data.PatchModel;

/// <summary>
/// Patch model for updating a zone.
/// </summary>
public class DnsZonePatch : IPatchModel
{
    /// <summary>
    /// The name of the zone; changing the name of a zone will change the fully-qualified name of every DNS record.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Update the notes for the zone.
    /// </summary>
    public string? Notes { get; set; }
}