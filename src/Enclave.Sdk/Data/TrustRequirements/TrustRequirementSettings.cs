namespace Enclave.Sdk.Api.Data.TrustRequirements;

/// <summary>
/// The variable-structure settings model used in trust requirements.
/// </summary>
public class TrustRequirementSettings
{
    /// <summary>
    /// Configuration of the trust requirement.
    /// </summary>
    public IReadOnlyDictionary<string, string?> Configuration { get; init; } = default!;

    /// <summary>
    /// Conditions to apply to the trust requirement when evaluating.
    /// </summary>
    public IReadOnlyList<IReadOnlyDictionary<string, string?>>? Conditions { get; init; }
}
