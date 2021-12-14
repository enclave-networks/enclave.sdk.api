namespace Enclave.Sdk.Api.Data.Policies;

/// <summary>
/// Model for the result of a policy bulk delete operation.
/// </summary>
public class BulkPolicyDeleteResult
{
    /// <summary>
    /// The number of policies successfully deleted.
    /// </summary>
    public int PoliciesDeleted { get; init; }
}
