namespace Enclave.Sdk.Api.Data.EnrolmentKeys;

/// <summary>
/// Input model for IP Address constraints.
/// </summary>
public class EnrolmentKeyIpConstraintInput
{
    /// <summary>
    /// The IP range.
    /// </summary>
    public string Range { get; set; } = default!;

    /// <summary>
    /// A description for the range.
    /// </summary>
    public string? Description { get; set; }
}