namespace Enclave.Sdk.Api.Data.Organisations;

public class OrganisationPricing
{
    /// <summary>
    /// The starter tier pricing info.
    /// </summary>
    public OrganisationPlanPricing Starter { get; init; }

    /// <summary>
    /// Pro tier pricing info.
    /// </summary>
    public OrganisationPlanPricing Pro { get; init; }

    /// <summary>
    /// Business tier pricing info.
    /// </summary>
    public OrganisationPlanPricing Business { get; init; }

    /// <summary>
    /// The last billing event for the organisation (if any).
    /// </summary>
    public OrganisationBillingEvent LastBillingEvent { get; init; }
}
