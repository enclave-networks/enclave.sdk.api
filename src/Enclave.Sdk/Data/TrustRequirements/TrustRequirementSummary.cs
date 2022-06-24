using Enclave.Sdk.Api.Data.TrustRequirements.Enum;

namespace Enclave.Sdk.Api.Data.TrustRequirements;

/// <summary>
/// Defines the basic properties of a trust requirement.
/// </summary>
public class TrustRequirementSummary
{
    /// <summary>
    /// The ID of the trust requirement.
    /// </summary>
    public TrustRequirementId Id { get; init; }

    /// <summary>
    /// The user-provided description of the trust requirement.
    /// </summary>
    public string Description { get; init; } = default!;

    /// <summary>
    /// The time (in UTC) when the trust requirement was created.
    /// </summary>
    public DateTime Created { get; init; }

    /// <summary>
    /// The time (in UTC) when the trust requirement was modified.
    /// </summary>
    public DateTime Modified { get; init; }

    /// <summary>
    /// The type of trust requirement.
    /// </summary>
    public TrustRequirementType Type { get; init; }

    /// <summary>
    /// The number of tags referencing this trust requirement.
    /// </summary>
    public int UsedInTags { get; init; }

    /// <summary>
    /// The number of policies directly referencing this trust requirement.
    /// </summary>
    public int UsedInPolicies { get; init; }

    /// <summary>
    /// A generated summary of the trust requirement properties.
    /// </summary>
    public string Summary { get; init; } = default!;
}
