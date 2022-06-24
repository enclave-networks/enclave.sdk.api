using Enclave.Sdk.Api.Data.TrustRequirements.Enum;

namespace Enclave.Sdk.Api.Data.TrustRequirements;

/// <summary>
/// Model used when creating a new trust requirement.
/// </summary>
public class TrustRequirementCreate
{
    /// <summary>
    /// The description of the trust requirement.
    /// </summary>
    public string Description { get; init; } = default!;

    /// <summary>
    /// The trust requirement type.
    /// </summary>
    public TrustRequirementType Type { get; init; }

    /// <summary>
    /// Any notes to store against the requirement.
    /// </summary>
    public string? Notes { get; init; }

    /// <summary>
    /// The settings structure (that varies content depending on the <see cref="Type"/>).
    /// </summary>
    public TrustRequirementSettingsModel Settings { get; init; } = default!;
}
