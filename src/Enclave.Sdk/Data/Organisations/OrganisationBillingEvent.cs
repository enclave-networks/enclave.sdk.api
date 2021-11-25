using Enclave.Sdk.Api.Data.Organisations.Enum;

namespace Enclave.Sdk.Api.Data.Organisations;

/// <summary>
/// Defines the properties available for a billing event on an organisation.
/// </summary>
public class OrganisationBillingEvent
{
    /// <summary>
    /// The code indicating the billing event.
    /// </summary>
    public string Code { get; init; } = default!;

    /// <summary>
    /// A human-readable message describing the event.
    /// </summary>
    public string Message { get; init; } = default!;

    /// <summary>
    /// The event 'level'.
    /// </summary>
    public BillingEventLevel Level { get; init; }
}