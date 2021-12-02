namespace Enclave.Sdk.Api.Data.UnaprrovedSystems;

/// <summary>
/// The result of a bulk unapproved system approve operation.
/// </summary>
public class BulkUnapprovedSystemApproveResult
{
    /// <summary>
    /// The number of systems approved.
    /// </summary>
    public int SystemsApproved { get; init; }
}
