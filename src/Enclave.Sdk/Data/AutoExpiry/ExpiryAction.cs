namespace Enclave.Sdk.Api.Data.AutoExpiry;

/// <summary>
/// Enum denoting the type of possible expiry actions.
/// </summary>
public enum ExpiryAction
{
    /// <summary>
    /// Disable item upon expiry.
    /// </summary>
    Disable,

    /// <summary>
    /// Delete item up expiry.
    /// </summary>
    Delete,
}
