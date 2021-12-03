using Enclave.Sdk.Api.Data;
using Enclave.Sdk.Api.Data.Pagination;
using Enclave.Sdk.Api.Data.PatchModel;
using Enclave.Sdk.Api.Data.UnaprrovedSystems;
using Enclave.Sdk.Api.Data.UnaprrovedSystems.Enum;

namespace Enclave.Sdk.Api.Clients.Interfaces;

/// <summary>
/// Perform CRUD operations on Unapproved Systems.
/// </summary>
public interface IUnapprovedSystemsClient
{
    /// <summary>
    /// Gets a paginated list of Unapproved Systems which can be searched and interated upon.
    /// </summary>
    /// <param name="enrolmentKeyId">Filter by systems using the specified Enrolment Key.</param>
    /// <param name="searchTerm">A partial matching search term.</param>
    /// <param name="sortOrder">Sort order for the pagination.</param>
    /// <param name="pageNumber">Which page number do you want to return.</param>
    /// <param name="perPage">How many tags per page.</param>
    /// <returns>A paginated response model with links to get the previous, next, first and last pages.</returns>
    Task<PaginatedResponseModel<UnapprovedSystem>> GetSystemsAsync(
        int? enrolmentKeyId = null,
        string? searchTerm = null,
        UnapprovedSystemQuerySortMode? sortOrder = null,
        int? pageNumber = null,
        int? perPage = null);

    /// <summary>
    /// Permanetly decline multiple Unapproved Systems.
    /// </summary>
    /// <param name="systemIds">System Ids to decline.</param>
    /// <returns>The number of systesm declined.</returns>
    Task<int> DeclineSystems(params string[] systemIds);

    /// <summary>
    /// Get the details of an Unapproved System.
    /// </summary>
    /// <param name="systemId">The system Id you want to get.</param>
    /// <returns>A Detailed Unapproved System Model.</returns>
    Task<UnapprovedSystemDetail> GetAsync(string systemId);

    /// <summary>
    /// Patch request to update an Unapproved System.
    /// </summary>
    /// <param name="systemId">The system Id you want to update.</param>
    /// <param cref="PatchBuilder{TModel}" name="builder">An instance of <see cref="PatchBuilder{TModel}"/> used to setup our patch request.</param>
    /// <returns>An updated Detailed Unapproved System model.</returns>
    Task<UnapprovedSystemDetail> UpdateAsync(string systemId, PatchBuilder<UnapprovedSystemPatch> builder);

    /// <summary>
    /// Decline an Unapproved System.
    /// </summary>
    /// <param name="systemId">The system Id you want to decline.</param>
    /// <returns>The declined System.</returns>
    Task<UnapprovedSystemDetail> DeclineAsync(string systemId);

    /// <summary>
    /// Approve a System.
    /// </summary>
    /// <param name="systemId">The system Id you want to approve.</param>
    Task ApproveAsync(string systemId);

    /// <summary>
    /// Approve multiple Unapproved Systems.
    /// </summary>
    /// <param name="systemIds">System Ids to approve.</param>
    /// <returns>The number of systesm approved.</returns>
    Task<int> ApproveSystemsAsync(params string[] systemIds);
}