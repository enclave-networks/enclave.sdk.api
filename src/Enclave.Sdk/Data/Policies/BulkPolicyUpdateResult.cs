namespace Enclave.Sdk.Api.Data.Policies;

/// <summary>
/// Defines the result of a bulk policy update.
/// </summary>
public class BulkPolicyUpdateResult
{
    /// <summary>
    /// The number of policies successfully updated.
    /// </summary>
    public int PoliciesUpdated { get; init; }
}
