using Enclave.Sdk.Api.Data.Pagination;
using Enclave.Sdk.Api.Data.Tags;

namespace Enclave.Sdk.Api.Clients.Interfaces;

/// <summary>
/// Access and search for tags.
/// </summary>
public interface ITagsClient
{
    /// <summary>
    /// Gets a paginated list of tags which can be searched and interated upon.
    /// </summary>
    /// <param name="searchTerm">A partial matching search term.</param>
    /// <param name="sortOrder">Sort order for the pagination.</param>
    /// <param name="pageNumber">Which page number do you want to return.</param>
    /// <param name="perPage">How many per page.</param>
    /// <returns>A paginated response model with links to get the previous, next, first and last pages.</returns>
    Task<PaginatedResponseModel<TagItem>> GetAsync(string? searchTerm = null, TagQuerySortOrder? sortOrder = null, int? pageNumber = null, int? perPage = null);
}