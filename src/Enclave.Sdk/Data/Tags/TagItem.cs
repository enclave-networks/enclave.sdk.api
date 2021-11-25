namespace Enclave.Sdk.Api.Data.Tags;

/// <summary>
/// Represents a single tag.
/// </summary>
public class TagItem
{
    /// <summary>
    /// The tag name.
    /// </summary>
    public string Tag { get; init; } = default!;

    /// <summary>
    /// The last time this tag was last referenced.
    /// </summary>
    public DateTime LastReferenced { get; init; }

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
}