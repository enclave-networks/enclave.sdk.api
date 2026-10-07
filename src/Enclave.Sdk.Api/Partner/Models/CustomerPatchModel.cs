using Enclave.Api.Scaffolding.Models;
using Enclave.Sdk.Api.Clients.Interfaces;

namespace Enclave.Sdk.Api.Partner.Models;

/// <summary>
/// The fields of a customer that <see cref="ICustomersClient.Update"/> can change. Only the fields set on the patch
/// client are sent. It has the property names, types and nullability of the
/// partner API's model, and the limits given are the API's
/// (portal src/Enclave.Partner.Api/Modules/CustomerManagement/Customers/Models/CustomerPatchModel.cs and
/// Validators/CustomerPatchModelValidator.cs).
/// </summary>
public class CustomerPatchModel : PatchModel
{
    /// <summary>
    /// The customer's name.
    /// </summary>
    public string? Name
    {
        get => Get<string>(nameof(Name));
        set => Set(nameof(Name), value);
    }

    /// <summary>
    /// Whether to request an industry discount.
    /// </summary>
    public bool IndustryDiscount
    {
        get => Get<bool>(nameof(IndustryDiscount));
        set => Set(nameof(IndustryDiscount), value);
    }

    /// <summary>
    /// The number of systems the customer is billed for. The API requires at least 2.
    /// </summary>
    public int LicensedAgentsCount
    {
        get => Get<int>(nameof(LicensedAgentsCount));
        set => Set(nameof(LicensedAgentsCount), value);
    }

    /// <summary>
    /// The number of gateways the customer is billed for.
    /// </summary>
    public int LicensedGatewaysCount
    {
        get => Get<int>(nameof(LicensedGatewaysCount));
        set => Set(nameof(LicensedGatewaysCount), value);
    }

    /// <summary>
    /// The name of a contact at the customer.
    /// </summary>
    public string? ContactName
    {
        get => Get<string>(nameof(ContactName));
        set => Set(nameof(ContactName), value);
    }

    /// <summary>
    /// Whether to enable a hard limit for the customer.
    /// </summary>
    public bool EnableHardLimit
    {
        get => Get<bool>(nameof(EnableHardLimit));
        set => Set(nameof(EnableHardLimit), value);
    }

    /// <summary>
    /// Whether to enable admin auto-sync for the customer. <see cref="ICustomersClient.EnableAutoSyncAsync"/> and
    /// <see cref="ICustomersClient.DisableAutoSyncAsync"/> change it through routes of their own.
    /// </summary>
    public bool AdminAutoSyncIsEnabled
    {
        get => Get<bool>(nameof(AdminAutoSyncIsEnabled));
        set => Set(nameof(AdminAutoSyncIsEnabled), value);
    }
}
