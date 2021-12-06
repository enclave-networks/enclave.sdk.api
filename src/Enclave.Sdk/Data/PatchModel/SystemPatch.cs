namespace Enclave.Sdk.Api.Data.PatchModel;

/// <summary>
/// Defines the modifiable properties of a system.
/// </summary>
public class SystemPatch : IPatchModel
{
    /// <summary>
    /// The system description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Whether or not the system is enabled (and available for use).
    /// </summary>
    public bool? IsEnabled { get; set; }

    /// <summary>
    /// The set of tags applied to the system.
    /// </summary>
    public List<string> Tags { get; set; } = new List<string>();

    /// <summary>
    /// Any notes or additional info for this system.
    /// </summary>
    public string? Notes { get; set; }
}