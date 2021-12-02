using Enclave.Sdk.Api.Data.Pagination;
using Enclave.Sdk.Api.Data.Policies;

namespace Enclave.Sdk.Api.Clients.Interfaces;

/// <summary>
/// Perform CRUD operations on DNS rules.
/// </summary>
public interface IPoliciesClient
{
    /// <summary>
    /// Gets a paginated list of Policies which can be searched and interated upon.
    /// </summary>
    /// <param name="searchTerm">A partial matching search term.</param>
    /// <param name="includeDisabled">Include the disabled Policies in the results.</param>
    /// <param name="sortOrder">Sort order for the pagination.</param>
    /// <param name="pageNumber">Which page number do you want to return.</param>
    /// <param name="perPage">How many tags per page.</param>
    /// <returns>A paginated response model with links to get the previous, next, first and last pages.</returns>
    Task<PaginatedResponseModel<Policy>> GetPoliciesAsync(
        string? searchTerm = null,
        bool? includeDisabled = null,
        PolicySortOrder? sortOrder = null,
        int? pageNumber = null,
        int? perPage = null);

    /// <summary>
    /// Creates a Policy using a <see cref="PolicyCreate"/> Model.
    /// </summary>
    /// <param name="createModel">The model needed to create a Policy.</param>
    /// <returns>The created <see cref="Policy"/>.</returns>
    Task<Policy> CreateAsync(PolicyCreate createModel);
}