namespace Enclave.Sdk.Api.Partner.Models;

/// <summary>
/// The body of the partner API's invite request, which <see cref="Clients.CustomersClient"/> builds from the email address
/// it is given (portal src/Enclave.Partner.Api/Modules/CustomerManagement/Customers/Models/CreateCustomerAdminInviteModel.cs).
/// </summary>
internal sealed class CreateCustomerAdminInviteModel
{
    /// <summary>
    /// The email address of the user to invite.
    /// </summary>
    public string EmailAddress { get; init; } = string.Empty;
}
