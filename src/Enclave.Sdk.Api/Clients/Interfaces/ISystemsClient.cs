using Enclave.Api.Modules.SystemManagement.Systems.Models;
using Enclave.Api.Scaffolding.Pagination.Models;
using Enclave.Configuration.Data.Enums;
using Enclave.Configuration.Data.Modules.Systems.Enums;
using Enclave.Sdk.Api.Data;

namespace Enclave.Sdk.Api.Clients.Interfaces;

/// <summary>
/// Provides operations to get, create, and manipulate Enrolled Systems.
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
    /// <param name="perPage">How many per page.</param>
    /// <returns>A paginated response model with links to get the previous, next, first and last pages.</returns>
    Task<PaginatedResponseModel<SystemSummaryModel>> GetSystemsAsync(
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
    Task<int> RevokeSystemsAsync(params string[] systemIds);

    /// <summary>
    /// Permanetly revoke multiple systems.
    /// </summary>
    /// <param name="systemIds">The System Ids to revoke.</param>
    /// <returns>The number of systems revoked.</returns>
    Task<int> RevokeSystemsAsync(IEnumerable<string> systemIds);

    /// <summary>
    /// Retrieve a Detailed System model.
    /// </summary>
    /// <param name="systemId">The SystemId to Get.</param>
    /// <returns>A Full System Model.</returns>
    Task<SystemModel> GetAsync(string systemId);

    /// <summary>
    /// Starts an update patch request.
    /// </summary>
    /// <param name="systemId">The SystemId to update.</param>
    /// <returns>A PatchClient for fluent updating.</returns>
    IPatchClient<SystemPatchModel, SystemModel> Update(string systemId);

    /// <summary>
    /// Revoke a system permanetly.
    /// </summary>
    /// <param name="systemId">The id of the Enrolled System to revoke.</param>
    /// <returns>The revoked Enrolled System.</returns>
    Task<SystemModel> RevokeAsync(string systemId);

    /// <summary>
    /// Enable an Enrolled System.
    /// </summary>
    /// <param name="systemId">The Id of the Enrolled System to enable.</param>
    /// <returns>A detailed Enrolled System.</returns>
    Task<SystemModel> EnableAsync(string systemId);

    /// <summary>
    /// Disable an Enrolled System.
    /// </summary>
    /// <param name="systemId">The Id of the Enrolled System to disable.</param>
    /// <returns>A detailed Enrolled System.</returns>
    Task<SystemModel> DisableAsync(string systemId);

    /// <summary>
    /// Bulk enable mutliple Enrolled System.
    /// </summary>
    /// <param name="systemIds">An array of Enrolled System Ids to enable.</param>
    /// <returns>The number of keys modified.</returns>
    Task<int> BulkEnableAsync(params string[] systemIds);

    /// <summary>
    /// Bulk enable mutliple Enrolled System.
    /// </summary>
    /// <param name="systemIds">An array of Enrolled System Ids to enable.</param>
    /// <returns>The number of keys modified.</returns>
    Task<int> BulkEnableAsync(IEnumerable<string> systemIds);

    /// <summary>
    /// Bulk disable mutliple Enrolled System.
    /// </summary>
    /// <param name="systemIds">An array of Enrolled System Ids to disable.</param>
    /// <returns>The number of keys modified.</returns>
    Task<int> BulkDisableAsync(params string[] systemIds);

    /// <summary>
    /// Bulk disable mutliple Enrolled System.
    /// </summary>
    /// <param name="systemIds">An array of Enrolled System Ids to disable.</param>
    /// <returns>The number of keys modified.</returns>
    Task<int> BulkDisableAsync(IEnumerable<string> systemIds);

    /// <summary>
    /// Enable this System for a specific period of time.
    /// </summary>
    /// <param name="systemId">The Id of the system to enable until.</param>
    /// <param name="expiryDateTime">A <see cref="DateTimeOffset"/> specifying the time when the <see cref="SystemModel"/> should be enabled until.</param>
    /// <param name="expiryAction">What should happen when the expiry date elapses.</param>
    /// <param name="timeZonedId">An IANA or Windows time zone ID. If this isn't null, expiryDateTime will be updated if the specified time zone's rules change.</param>
    /// <returns>A detailed Enrolled System.</returns>
    Task<SystemModel> EnableUntilAsync(string systemId, DateTimeOffset expiryDateTime, ExpiryAction expiryAction, string? timeZonedId = null);
}