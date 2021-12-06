using Enclave.Sdk.Api.Data.Policies.Enum;
using Enclave.Sdk.Api.Data.Tags;

namespace Enclave.Sdk.Api.Data.Policies;

/// <summary>
/// Represents a single policy.
/// </summary>
public class Policy
{
    /// <summary>
    /// The ID of the policy.
    /// </summary>
    public PolicyId Id { get; init; }

    /// <summary>
    /// The UTC timestamp when the policy was created.
    /// </summary>
    public DateTime Created { get; init; }

    /// <summary>
    /// The provided description of the policy.
    /// </summary>
    public string Description { get; init; }

    /// <summary>
    /// Whether or not this policy is enabled.
    /// </summary>
    public bool IsEnabled { get; init; }

    /// <summary>
    /// Indicates the state of the policy.
    /// </summary>
    public PolicyState State { get; init; }

    /// <summary>
    /// The sender-side tags.
    /// </summary>
    public IReadOnlyList<TagReference> SenderTags { get; init; } = Array.Empty<TagReference>();

    /// <summary>
    /// The receiver-side tags.
    /// </summary>
    public IReadOnlyList<TagReference> ReceiverTags { get; init; } = Array.Empty<TagReference>();

    /// <summary>
    /// Access control lists.
    /// </summary>
    public IReadOnlyList<PolicyAcl> Acls { get; init; }

    /// <summary>
    /// Optional notes for the policy.
    /// </summary>
    public string? Notes { get; init; }
}
