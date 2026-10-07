namespace Enclave.Sdk.Api.Partner.Models;

/// <summary>
/// The body of the partner API's convert request, which <see cref="Clients.CustomersClient"/> builds from the billing period
/// it is given (portal src/Enclave.Partner.Api/Modules/CustomerManagement/Customers/Models/ConvertCustomerModel.cs).
/// </summary>
internal sealed class ConvertCustomerModel
{
    /// <summary>
    /// The billing period in months: 1, 12, 24 or 36. The API takes 1 when the body leaves it out; this model always sends it.
    /// </summary>
    public int BillingPeriodMonths { get; init; }
}
