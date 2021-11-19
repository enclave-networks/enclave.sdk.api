namespace Enclave.Sdk.Api.Data.Organisations;


/// <summary>
/// A model defining the pricing for a given plan.
/// </summary>
public class OrganisationPlanPricing
{
    /// <summary>
    /// The appropriate currency symbol.
    /// </summary>
    public string CurrencySymbol { get; init; }

    /// <summary>
    /// Whether or not this plan is enabled.
    /// </summary>
    public bool Enabled { get; init; }

    /// <summary>
    /// The quantities of systems available in this plan.
    /// </summary>
    public IReadOnlyList<PlanPricingQuanitity> Quantities { get; init; }
}
