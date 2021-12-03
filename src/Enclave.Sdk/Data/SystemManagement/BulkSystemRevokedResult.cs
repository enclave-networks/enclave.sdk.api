namespace Enclave.Sdk.Api.Data.SystemManagement;

/// <summary>
/// The result of a bulk system revocation.
/// </summary>
public class BulkSystemRevokedResult
{
    /// <summary>
    /// How many systems were actually revoked.
    /// </summary>
    public int SystemsRevoked { get; set; }
}
