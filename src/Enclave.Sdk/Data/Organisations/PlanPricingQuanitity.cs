namespace Enclave.Sdk.Api.Data.Organisations;

/// <summary>
/// The price of a certain quantity of systems.
/// </summary>
public class PlanPricingQuanitity
{
    /// <summary>
    /// The max system capacity of this quantity.
    /// </summary>
    public int Capacity { get; init; }

    /// <summary>
    /// The price of this quantity.
    /// </summary>
    public decimal Price { get; init; }

    /// <summary>
    /// Whether or not this is the default quantity to display in the plan.
    /// </summary>
    public bool IsDefault { get; init; }
}
