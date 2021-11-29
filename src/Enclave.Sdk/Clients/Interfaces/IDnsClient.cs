using Enclave.Sdk.Api.Data.Dns;
using Enclave.Sdk.Api.Data.Pagination;

namespace Enclave.Sdk.Api.Clients.Interfaces;

/// <summary>
/// Perform CRUD operations on DNS rules.
/// </summary>
public interface IDnsClient
{
    /// <summary>
    /// Gets a summary of DNS properties.
    /// </summary>
    /// <returns>A summary of DNS properties.</returns>
    Task<DnsSummary> GetPropertiesSummaryAsync();

    /// <summary>
    /// Gets a paginated list of DNS zones.
    /// </summary>
    /// <param name="pageNumber">Which page number do you want to return.</param>
    /// <param name="perPage">How many items per page.</param>
    /// <returns>A paginated response model with links to get the previous, next, first and last pages.</returns>
    Task<PaginatedResponseModel<BasicDnsZone>> GetZonesAsync(int? pageNumber = null, int? perPage = null);
}