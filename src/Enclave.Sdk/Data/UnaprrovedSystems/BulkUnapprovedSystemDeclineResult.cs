namespace Enclave.Sdk.Api.Data.UnaprrovedSystems;

    /// <summary>
    /// The result of a bulk unapproved system decline operation.
    /// </summary>
public class BulkUnapprovedSystemDeclineResult
{
    /// <summary>
    /// The number of systems declined.
    /// </summary>
    public int SystemsDeclined { get; init; }
}
