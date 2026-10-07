using Enclave.Api.Modules.SystemManagement.UnapprovedSystems.Models;
using Enclave.Api.Scaffolding.Pagination.Models;
using Enclave.Sdk.Api.Data;

namespace Enclave.Sdk.Api.Clients.Interfaces;

/// <summary>
/// Provides operations to get, create, and manipulate Unapproved Systems.
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
    /// <param name="perPage">How many per page.</param>
    /// <returns>A paginated response model with links to get the previous, next, first and last pages.</returns>
    Task<PaginatedResponseModel<UnapprovedSystemSummaryModel>> GetSystemsAsync(
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
    /// Permanetly decline multiple Unapproved Systems.
    /// </summary>
    /// <param name="systemIds">System Ids to decline.</param>
    /// <returns>The number of systesm declined.</returns>
    Task<int> DeclineSystems(IEnumerable<string> systemIds);

    /// <summary>
    /// Get the details of an Unapproved System.
    /// </summary>
    /// <param name="systemId">The system Id you want to get.</param>
    /// <returns>A Detailed Unapproved System Model.</returns>
    /// <exception cref="ArgumentException">Throws if <paramref name="systemId"/> is null, empty, "." or "..".</exception>
    Task<UnapprovedSystemModel> GetAsync(string systemId);

    /// <summary>
    /// Starts an update patch request.
    /// </summary>
    /// <param name="systemId">The SystemId to update.</param>
    /// <returns>A PatchClient for fluent updating.</returns>
    /// <exception cref="ArgumentException">Throws if <paramref name="systemId"/> is null, empty, "." or "..".</exception>
    IPatchClient<UnapprovedSystemPatchModel, UnapprovedSystemModel> Update(string systemId);

    /// <summary>
    /// Decline an Unapproved System.
    /// </summary>
    /// <param name="systemId">The system Id you want to decline.</param>
    /// <returns>The declined System.</returns>
    /// <exception cref="ArgumentException">Throws if <paramref name="systemId"/> is null, empty, "." or "..".</exception>
    Task<UnapprovedSystemModel> DeclineAsync(string systemId);

    /// <summary>
    /// Approve a System.
    /// </summary>
    /// <param name="systemId">The system Id you want to approve.</param>
    /// <exception cref="ArgumentException">Throws if <paramref name="systemId"/> is null, empty, "." or "..".</exception>
    Task ApproveAsync(string systemId);

    /// <summary>
    /// Approve multiple Unapproved Systems.
    /// </summary>
    /// <param name="systemIds">System Ids to approve.</param>
    /// <returns>The number of systesm approved.</returns>
    Task<int> ApproveSystemsAsync(params string[] systemIds);

    /// <summary>
    /// Approve multiple Unapproved Systems.
    /// </summary>
    /// <param name="systemIds">System Ids to approve.</param>
    /// <returns>The number of systesm approved.</returns>
    Task<int> ApproveSystemsAsync(IEnumerable<string> systemIds);

    /// <summary>
    /// Gets the search keys the search term of <see cref="GetSystemsAsync"/> accepts.
    /// </summary>
    /// <returns>The search keys, in the order the API lists them.</returns>
    Task<IReadOnlyList<SearchKey>> GetSearchKeysAsync();
}