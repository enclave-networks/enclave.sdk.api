namespace Enclave.Sdk.Api.Data.EnrolmentKeys;

/// <summary>
/// Input model for IP Address constraints.
/// </summary>
public class EnrolmentKeyIpConstraintInput
{
    /// <summary>
    /// A mandatory constructor for creating the IP Constraint.
    /// </summary>
    /// <param name="range">The IP range.</param>
    public EnrolmentKeyIpConstraintInput(string range)
    {
        Range = range;
    }

    /// <summary>
    /// The IP range.
    /// </summary>
    public string Range { get; } = default!;

    /// <summary>
    /// A description for the range.
    /// </summary>
    public string? Description { get; set; }
}