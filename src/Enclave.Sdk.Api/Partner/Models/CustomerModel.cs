using Enclave.Configuration.Data.Identifiers;

namespace Enclave.Sdk.Api.Partner.Models;

/// <summary>
/// A customer of a partner. A customer is an Enclave organisation, and <see cref="Id"/> is its organisation ID.
/// It has the property names, types and nullability of the partner API's model
/// (portal src/Enclave.Partner.Api/Modules/CustomerManagement/Customers/Models/CustomerModel.cs).
/// </summary>
public class CustomerModel
{
    /// <summary>
    /// The ID of the customer's organisation.
    /// </summary>
    public OrganisationGuid Id { get; init; }

    /// <summary>
    /// The name of the customer's organisation.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// The number of enrolled systems.
    /// </summary>
    public int EnrolledSystems { get; init; }

    /// <summary>
    /// The number of configured gateways.
    /// </summary>
    public int ConfiguredGateways { get; init; }

    /// <summary>
    /// The number of systems licensed to the customer. Usage can go past it, up to <see cref="HardLimit"/> when a hard limit applies.
    /// </summary>
    public int LicensedSystems { get; init; }

    /// <summary>
    /// The number of gateways licensed to the customer. Usage can go past it.
    /// </summary>
    public int LicensedGateways { get; init; }

    /// <summary>
    /// The hard limit on the customer's usage, or -1 when no hard limit applies.
    /// </summary>
    public int HardLimit { get; init; }

    /// <summary>
    /// Whether an industry discount applies.
    /// </summary>
    public bool IndustryDiscount { get; init; }

    /// <summary>
    /// The name of the customer's primary contact.
    /// </summary>
    public string? ContactName { get; init; }

    /// <summary>
    /// The billing period in months, when the customer is paid for.
    /// </summary>
    public int? BillingPeriodMonths { get; init; }

    /// <summary>
    /// The customer's status.
    /// </summary>
    public CustomerStatusEnum Status { get; init; }

    /// <summary>
    /// The end of the customer's trial, when the customer is in one.
    /// </summary>
    public DateTimeOffset? TrialEndDate { get; init; }

    /// <summary>
    /// Whether Enclave has forced the hard limit on, so it cannot be removed.
    /// </summary>
    public bool HardLimitForcedOn { get; init; }

    /// <summary>
    /// Whether the customer is suspended, by the end of a free trial or by an Enclave administrator.
    /// </summary>
    public bool IsSuspended { get; init; }

    /// <summary>
    /// Whether the calling user has access to the customer's organisation.
    /// </summary>
    public bool UserHasAccess { get; init; }

    /// <summary>
    /// The date of the next invoice.
    /// </summary>
    public DateTimeOffset? NextInvoiceDate { get; init; }

    /// <summary>
    /// Whether this customer is the partner's free NFR licence.
    /// </summary>
    public bool IsNfr { get; init; }

    /// <summary>
    /// Whether admin auto-sync is enabled.
    /// </summary>
    public bool AdminAutoSyncIsEnabled { get; init; }

    /// <summary>
    /// The oldest Enclave agent version among the customer's enrolled systems, or null when it has none.
    /// Only the customer list sets it, and only for the customers' current state; every other call leaves it null.
    /// </summary>
    public string? OldestEnclaveVersion { get; init; }
}
