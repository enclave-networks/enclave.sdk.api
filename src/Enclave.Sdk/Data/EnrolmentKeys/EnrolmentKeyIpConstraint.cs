namespace Enclave.Sdk.Api.Data.EnrolmentKeys;

/// <summary>
/// Model for IP Address constraints.
/// </summary>
public class EnrolmentKeyIpConstraint
{
    /// <summary>
    /// The IP range.
    /// </summary>
    public string Range { get; init; } = default!;

    /// <summary>
    /// A description for the range.
    /// </summary>
    public string? Description { get; init; }
}