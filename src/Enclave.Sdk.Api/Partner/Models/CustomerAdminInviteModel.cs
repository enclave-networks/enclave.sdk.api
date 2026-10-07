using Enclave.Configuration.Data.Identifiers;
using Enclave.Sdk.Api.Clients.Interfaces;

namespace Enclave.Sdk.Api.Partner.Models;

/// <summary>
/// An invite for a user to be an admin of a customer's organisation. It has the property names, types and nullability
/// of the partner API's model (portal src/Enclave.Partner.Api/Modules/CustomerManagement/Customers/Models/CustomerAdminInviteModel.cs).
/// </summary>
public class CustomerAdminInviteModel
{
    /// <summary>
    /// The invite's ID: the organisation ID and a number, joined by a hyphen. For <see cref="ICustomersClient.CancelInviteAsync"/>,
    /// whose API response has no ID, it is the ID the call was given.
    /// </summary>
    public OrganisationInviteId Id { get; init; }

    /// <summary>
    /// The email address of the invited user.
    /// </summary>
    public string EmailAddress { get; init; } = string.Empty;
}
