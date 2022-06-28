using Enclave.Sdk.Api.Data.TrustRequirements;

namespace Enclave.Sdk.Api.Data.Tags;

/// <summary>
/// Represents a single tag.
/// </summary>
public class DetailedTag
{
    /// <summary>
    /// The tag name.
    /// </summary>
    public string Tag { get; init; } = default!;

    /// <summary>
    /// The tag reference and unique ID, that persists between renames of the tag.
    /// </summary>
    public TagRefId Ref { get; init; }

    /// <summary>
    /// The configured custom colour for the tag.
    /// </summary>
    public string? Colour { get; init; }

    /// <summary>
    /// The time this tag was created.
    /// </summary>
    public DateTime Created { get; init; }

    /// <summary>
    /// The last time this tag was modified.
    /// </summary>
    public DateTime LastModified { get; init; }

    /// <summary>
    /// The last time this tag was referenced.
    /// </summary>
    public DateTime? LastReferenced { get; init; }

    /// <summary>
    /// The number of systems that reference this tag.
    /// </summary>
    public int Systems { get; init; }

    /// <summary>
    /// The number of enrolment keys that reference this tag.
    /// </summary>
    public int Keys { get; init; }

    /// <summary>
    /// The number of policies that reference this tag.
    /// </summary>
    public int Policies { get; init; }

    /// <summary>
    /// The number of DNS records that reference this tag.
    /// </summary>
    public int DnsRecords { get; init; }

    /// <summary>
    /// Any notes attached to the tag.
    /// </summary>
    public string? Notes { get; init; }

    /// <summary>
    /// Any trust requirements attached to the tag.
    /// </summary>
    public IReadOnlyList<UsedTrustRequirement> TrustRequirements { get; init; } = default!;
}
