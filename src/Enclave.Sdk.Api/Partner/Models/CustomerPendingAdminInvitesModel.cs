namespace Enclave.Sdk.Api.Partner.Models;

/// <summary>
/// The body of the partner API's customer invites response, which <see cref="Clients.CustomersClient"/> returns the list of
/// (portal src/Enclave.Partner.Api/Modules/CustomerManagement/Customers/Models/CustomerPendingAdminInvitesModel.cs).
/// </summary>
internal sealed class CustomerPendingAdminInvitesModel
{
    /// <summary>
    /// The customer's pending admin invites.
    /// </summary>
    public IReadOnlyList<CustomerAdminInviteModel> Invites { get; init; } = Array.Empty<CustomerAdminInviteModel>();
}
