using Enclave.Sdk.Api.Clients.Interfaces;

namespace Enclave.Sdk.Api.Partner.Models;

/// <summary>
/// A new customer, for <see cref="ICustomersClient.CreateAsync"/>. Every field is sent. It has the property names, types
/// and nullability of the partner API's model, and the limits given are the API's
/// (portal src/Enclave.Partner.Api/Modules/CustomerManagement/Customers/Models/CustomerCreateModel.cs and
/// Validators/CustomerCreateModelValidator.cs).
/// </summary>
public class CustomerCreateModel
{
    /// <summary>
    /// The customer's name. Required.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The email address of the user who will own the customer's new organisation.
    /// </summary>
    public string? OwnerEmail { get; set; }

    /// <summary>
    /// The customer's business domain name. When set, the API checks that it resolves in DNS.
    /// </summary>
    public string? Domain { get; set; }

    /// <summary>
    /// Whether to request an industry discount for the customer.
    /// </summary>
    public bool IndustryDiscount { get; set; }

    /// <summary>
    /// The number of systems the customer is expected to start with. The API requires at least 2.
    /// </summary>
    public int InitialSystemsCount { get; set; }

    /// <summary>
    /// The number of gateways the customer is expected to start with.
    /// </summary>
    public int InitialGatewaysCount { get; set; }

    /// <summary>
    /// The name of a contact at the customer.
    /// </summary>
    public string? ContactName { get; set; }

    /// <summary>
    /// Whether to apply a hard limit to the customer.
    /// </summary>
    public bool HardLimit { get; set; }

    /// <summary>
    /// Whether to enable admin auto-sync for the customer.
    /// </summary>
    public bool AdminAutoSyncIsEnabled { get; set; }
}
