namespace Enclave.Sdk.Api.Data.Tags;

/// <summary>
/// The sort order used for a Tag Query.
/// </summary>
public enum TagQuerySortOrder
{
    /// <summary>
    /// Sort Alphabetically.
    /// </summary>
    Alphabetical,

    /// <summary>
    /// Sort by recently used Tags.
    /// </summary>
    RecentlyUsed,

    /// <summary>
    /// Sort by number of Referenced Systems.
    /// </summary>
    ReferencedSystems,
}