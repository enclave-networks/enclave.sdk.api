using Enclave.Sdk.Api.Data;
using Enclave.Sdk.Api.Data.EnrolledSystems;
using Enclave.Sdk.Api.Data.EnrolledSystems.Enum;
using Enclave.Sdk.Api.Data.Pagination;
using Enclave.Sdk.Api.Data.PatchModel;

namespace Enclave.Sdk.Api.Clients.Interfaces;

/// <summary>
/// Provides operations to get, create, and manipulate Enrolled Systems.
/// </summary>
public interface IEnrolledSystemsClient
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
    /// <param name="perPage">How many per page.</param>
    /// <returns>A paginated response model with links to get the previous, next, first and last pages.</returns>
    Task<PaginatedResponseModel<EnrolledSystemSummary>> GetSystemsAsync(
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
    Task<int> RevokeSystemsAsync(params SystemId[] systemIds);

    /// <summary>
    /// Permanetly revoke multiple systems.
    /// </summary>
    /// <param name="systemIds">The System Ids to revoke.</param>
    /// <returns>The number of systems revoked.</returns>
    Task<int> RevokeSystemsAsync(IEnumerable<SystemId> systemIds);

    /// <summary>
    /// Retrieve a Detailed System model.
    /// </summary>
    /// <param name="systemId">The SystemId to Get.</param>
    /// <returns>A Full System Model.</returns>
    Task<EnrolledSystem> GetAsync(SystemId systemId);

    /// <summary>
    /// Starts an update patch request.
    /// </summary>
    /// <param name="systemId">The SystemId to update.</param>
    /// <returns>A PatchClient for fluent updating.</returns>
    IPatchClient<EnrolledSystemPatch, EnrolledSystem> Update(SystemId systemId);

    /// <summary>
    /// Revoke a system permanetly.
    /// </summary>
    /// <param name="systemId">The id of the Enrolled System to revoke.</param>
    /// <returns>The revoked Enrolled System.</returns>
    Task<EnrolledSystem> RevokeAsync(SystemId systemId);

    /// <summary>
    /// Enable an Enrolled System.
    /// </summary>
    /// <param name="systemId">The Id of the Enrolled System to enable.</param>
    /// <returns>A detailed Enrolment Key.</returns>
    Task<EnrolledSystem> EnableAsync(SystemId systemId);

    /// <summary>
    /// Disable an Enrolled System.
    /// </summary>
    /// <param name="systemId">The Id of the Enrolled System to disable.</param>
    /// <returns>A detailed Enrolment Key.</returns>
    Task<EnrolledSystem> DisableAsync(SystemId systemId);

    /// <summary>
    /// Bulk enable mutliple Enrolled System.
    /// </summary>
    /// <param name="systemIds">An array of Enrolled System Ids to enable.</param>
    /// <returns>The number of keys modified.</returns>
    Task<int> BulkEnableAsync(params SystemId[] systemIds);

    /// <summary>
    /// Bulk enable mutliple Enrolled System.
    /// </summary>
    /// <param name="systemIds">An array of Enrolled System Ids to enable.</param>
    /// <returns>The number of keys modified.</returns>
    Task<int> BulkEnableAsync(IEnumerable<SystemId> systemIds);

    /// <summary>
    /// Bulk disable mutliple Enrolled System.
    /// </summary>
    /// <param name="systemIds">An array of Enrolled System Ids to disable.</param>
    /// <returns>The number of keys modified.</returns>
    Task<int> BulkDisableAsync(params SystemId[] systemIds);

    /// <summary>
    /// Bulk disable mutliple Enrolled System.
    /// </summary>
    /// <param name="systemIds">An array of Enrolled System Ids to disable.</param>
    /// <returns>The number of keys modified.</returns>
    Task<int> BulkDisableAsync(IEnumerable<SystemId> systemIds);
}