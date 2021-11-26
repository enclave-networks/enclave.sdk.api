namespace Enclave.Sdk.Api.Data.Pagination;

/// <summary>
/// Defines the metadata attached to a paginated response.
/// </summary>
public class PaginationMetadata
{
    /// <summary>
    /// The total number of items (across all pages).
    /// </summary>
    public int Total { get; init; }

    /// <summary>
    /// The first page number.
    /// </summary>
    public int FirstPage { get; init; }

    /// <summary>
    /// The previous page number (null if this is the first page).
    /// </summary>
    public int? PrevPage { get; init; }

    /// <summary>
    /// The last page number.
    /// </summary>
    public int LastPage { get; init; }

    /// <summary>
    /// The next page number (null if this is the last page).
    /// </summary>
    public int? NextPage { get; init; }
}
