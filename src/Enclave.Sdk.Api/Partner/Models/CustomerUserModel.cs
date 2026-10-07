using Enclave.Configuration.Data.Enums;
using Enclave.Configuration.Data.Identifiers;

namespace Enclave.Sdk.Api.Partner.Models;

/// <summary>
/// An owner or admin of a customer's organisation. It has the property names, types and nullability of the partner
/// API's model (portal src/Enclave.Partner.Api/Modules/CustomerManagement/Customers/Models/CustomerUserModel.cs).
/// </summary>
public class CustomerUserModel
{
    /// <summary>
    /// The user's account ID.
    /// </summary>
    public AccountGuid Id { get; init; }

    /// <summary>
    /// The user's email address.
    /// </summary>
    public string EmailAddress { get; init; } = string.Empty;

    /// <summary>
    /// The user's full name.
    /// </summary>
    public string FullName { get; init; } = string.Empty;

    /// <summary>
    /// When the user joined the organisation, in UTC.
    /// </summary>
    public DateTime JoinDate { get; init; }

    /// <summary>
    /// The user's role in the organisation.
    /// </summary>
    public UserOrganisationRole Role { get; init; }
}
