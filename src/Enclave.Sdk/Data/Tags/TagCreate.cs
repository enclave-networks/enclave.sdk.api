namespace Enclave.Sdk.Api.Data.Tags;

/// <summary>
/// Data required to create a new tag.
/// </summary>
public class TagCreate
{
    /// <summary>
    /// The tag name.
    /// </summary>
    public string Tag { get; init; } = default!;

    /// <summary>
    /// An optional custom tag colour.
    /// </summary>
    public string? Colour { get; init; }

    /// <summary>
    /// Optional notes for the tag.
    /// </summary>
    public string? Notes { get; init; }

    ///// <summary>
    ///// Any trust requirements to apply to the tag.
    ///// </summary>
    //public TrustRequirementId[] TrustRequirements { get; init; } = Array.Empty<TrustRequirementId>();
}
