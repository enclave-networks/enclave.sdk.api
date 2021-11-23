namespace Enclave.Sdk.Api.Data.Pagination;

/// <summary>
/// Defines the available pagination links.
/// </summary>
public class PaginationLinks
{
    /// <summary>
    /// The first page of data.
    /// </summary>
    public Uri First { get; init; }

    /// <summary>
    /// The previous page of data (or null if this is the first page).
    /// </summary>
    public Uri? Prev { get; init; }

    /// <summary>
    /// The next page of data (or null if this is the last page).
    /// </summary>
    public Uri? Next { get; init; }

    /// <summary>
    /// The last page of data.
    /// </summary>
    public Uri Last { get; init; }
}