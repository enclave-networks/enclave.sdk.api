using Enclave.Sdk.Api.Data.TrustRequirements;

namespace Enclave.Sdk.Api.Data.PatchModel;

/// <summary>
/// Defines the modifiable properties of a tag.
/// </summary>
public class TagPatch : IPatchModel
{
    /// <summary>
    /// The tag name.
    /// </summary>
    public string? Tag { get; set; }

    /// <summary>
    /// An optional custom tag colour.
    /// </summary>
    public string? Colour { get; set; }

    /// <summary>
    /// Any notes or additional info for this tag.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// An array of associated Trust rRquirements.
    /// </summary>
    public TrustRequirementId[]? TrustRequirements { get; set; }
}
