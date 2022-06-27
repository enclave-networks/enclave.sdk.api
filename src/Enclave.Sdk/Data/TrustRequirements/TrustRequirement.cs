using Enclave.Sdk.Api.Data.TrustRequirements.Enum;

namespace Enclave.Sdk.Api.Data.TrustRequirements;

/// <summary>
/// Trust requirement model.
/// </summary>
public class TrustRequirement
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
    /// Any notes stored against the trust requirement.
    /// </summary>
    public string? Notes { get; init; }

    /// <summary>
    /// The trust requirement settings (varies content by <see cref="Type"/>).
    /// </summary>
    public TrustRequirementSettings Settings { get; init; } = default!;
}