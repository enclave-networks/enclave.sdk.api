using Enclave.Sdk.Api.Data.TrustRequirements.Enum;

namespace Enclave.Sdk.Api.Data.TrustRequirements;

/// <summary>
/// Information about a trust requirement include in entities where it has been used.
/// </summary>
public class UsedTrustRequirement
{
    /// <summary>
    /// The trust requirement ID.
    /// </summary>
    public TrustRequirementId Id { get; init; }

    /// <summary>
    /// The trust requirement description.
    /// </summary>
    public string Description { get; init; } = default!;

    /// <summary>
    /// The type of trust requirement.
    /// </summary>
    public TrustRequirementType Type { get; init; }
}
