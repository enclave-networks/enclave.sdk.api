using Enclave.Sdk.Api.Data.Pagination;
using Enclave.Sdk.Api.Data.SystemManagement;
using Enclave.Sdk.Api.Data.SystemManagement.Enum;

namespace Enclave.Sdk.Api.Clients.Interfaces;

/// <summary>
/// Perform CRUD operations on Systems.
/// </summary>
public interface ISystemsClient
{
    /// <summary>
    /// Gets a paginated list of Systems which can be searched and iterated upon.
    /// </summary>
    /// <param name="enrolmentKeyId">The Enrolment Key Id which the systems are associated to.</param>
    /// <param name="searchTerm">Search for systems with a partial match on description and system ID.</param>
    /// <param name="includeDisabled">Should include disabled Systems.</param>
    /// <param name="sortOrder">Sort order for the pagination.</param>
    /// <param name="dnsName">Searches for systems that will answer to the specified DNS name.</param>
    /// <param name="pageNumber">Which page number do you want to return.</param>
    /// <param name="perPage">How many tags per page.</param>
    /// <returns>A paginated response model with links to get the previous, next, first and last pages.</returns>
    Task<PaginatedResponseModel<SystemSummary>> GetSystemsAsync(
        int? enrolmentKeyId = null,
        string? searchTerm = null,
        bool? includeDisabled = null,
        SystemQuerySortMode? sortOrder = null,
        string? dnsName = null,
        int? pageNumber = null,
        int? perPage = null);

    /// <summary>
    /// Permanetly revoke multiple systems.
    /// </summary>
    /// <param name="systemIds">The System Ids to revoke.</param>
    /// <returns>The number of systems revoked.</returns>
    Task<int> RevokeSystems(params string[] systemIds);

    /// <summary>
    /// Retrieve a Detailed System model.
    /// </summary>
    /// <param name="systemId">The SystemId to Get.</param>
    /// <returns>A Full System Model.</returns>
    Task<EnclaveSystem> GetAsync(string systemId);
}