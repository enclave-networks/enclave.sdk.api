namespace Enclave.Sdk.Api.Data.TrustRequirements;

/// <summary>
/// Model for the result of a trust requirement bulk delete operation.
/// </summary>
public class BulkTrustRequirementDeleteResult
{
    /// <summary>
    /// The number of requirements successfully deleted.
    /// </summary>
    public int RequirementsDeleted { get; init; }
}
