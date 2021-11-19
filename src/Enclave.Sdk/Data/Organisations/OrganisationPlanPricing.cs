namespace Enclave.Sdk.Api.Data.Organisations;

public class OrganisationPlanPricing
{
    public string CurrencySymbol { get; init; }

    public bool Enabled { get; init; }

    public IReadOnlyList<Quantity> Quantities { get; init; }
}
