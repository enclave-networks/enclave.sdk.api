namespace Enclave.Sdk.Api.Partner.Models;

/// <summary>
/// The body of the partner API's customer admins response, which <see cref="Clients.CustomersClient"/> returns the list of
/// (portal src/Enclave.Partner.Api/Modules/CustomerManagement/Customers/Models/CustomerUsersModel.cs).
/// </summary>
internal sealed class CustomerUsersModel
{
    /// <summary>
    /// The customer's owners and admins.
    /// </summary>
    public IReadOnlyList<CustomerUserModel> Users { get; init; } = Array.Empty<CustomerUserModel>();
}
