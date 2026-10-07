namespace Enclave.Sdk.Api.Partner.Models;

/// <summary>
/// The states of a partner's customer. The API writes them by name, and the names are the partner API's
/// (portal src/Enclave.Partner.Api/Modules/CustomerManagement/Customers/Models/CustomerStatusEnum.cs).
/// </summary>
public enum CustomerStatusEnum
{
    /// <summary>
    /// The proof of concept has not started: no system has enrolled yet.
    /// </summary>
    PendingPoC,

    /// <summary>
    /// The proof of concept is in progress.
    /// </summary>
    InPoC,

    /// <summary>
    /// The proof of concept ended without the customer converting to paid.
    /// </summary>
    ExpiredPoC,

    /// <summary>
    /// The customer has been converted to paid.
    /// </summary>
    Paying,

    /// <summary>
    /// The customer is paying, with a discount applied.
    /// </summary>
    PayingDiscounted,

    /// <summary>
    /// The customer is covered by a bulk partner licence.
    /// </summary>
    PayingBulkLicense,

    /// <summary>
    /// The customer is free, with no time limit.
    /// </summary>
    Free,
}
