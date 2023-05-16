using Enclave.Api.Modules.SystemManagement.EnrolmentKeys.Models;
using Enclave.Api.Scaffolding.Pagination.Models;
using Enclave.Configuration.Data.Enums;
using Enclave.Configuration.Data.Identifiers;
using Enclave.Configuration.Data.Modules.EnrolmentKeys.Enums;
using Enclave.Sdk.Api.Data;

namespace Enclave.Sdk.Api.Clients.Interfaces;

/// <summary>
/// Provides operations to get, create, and manipulate Enrolment Keys.
/// </summary>
public interface IEnrolmentKeysClient
{
    /// <summary>
    /// Gets a paginated list of Enrolment Keys which can be searched and interated upon.
    /// </summary>
    /// <param name="searchTerm">A partial matching search term.</param>
    /// <param name="includeDisabled">Include the disabled Enrolment Keys in the results.</param>
    /// <param name="sortOrder">Sort order for the pagination.</param>
    /// <param name="pageNumber">Which page number do you want to return.</param>
    /// <param name="perPage">How many Enrolment Keys per page.</param>
    /// <returns>A paginated response model with links to get the previous, next, first and last pages.</returns>
    Task<PaginatedResponseModel<EnrolmentKeySummaryModel>> GetEnrolmentKeysAsync(string? searchTerm = null, bool? includeDisabled = null, EnrolmentKeySortOrder? sortOrder = null, int? pageNumber = null, int? perPage = null);

    /// <summary>
    /// Creates an Enrolment Key using a <see cref="EnrolmentKeyCreateModel"/> Model.
    /// </summary>
    /// <param name="createModel">The model needed to create an Enrolment Key.</param>
    /// <returns>The created Enrolment Key as a <see cref="EnrolmentKeyModel"/>.</returns>
    Task<EnrolmentKeyModel> CreateAsync(EnrolmentKeyCreateModel createModel);

    /// <summary>
    /// Gets a detailed Enrolment Key model.
    /// </summary>
    /// <param name="enrolmentKeyId">The Id of the Enrolment Key to get.</param>
    /// <returns>A detailed Enrolment Key.</returns>
    Task<EnrolmentKeyModel> GetAsync(EnrolmentKeyId enrolmentKeyId);

    /// <summary>
    /// Starts an update patch request.
    /// </summary>
    /// <param name="enrolmentKeyId">The EnrolmentKeyId to update.</param>
    /// <returns>A PatchClient for fluent updating.</returns>
    IPatchClient<EnrolmentKeyPatchModel, EnrolmentKeyModel> Update(EnrolmentKeyId enrolmentKeyId);

    /// <summary>
    /// Enable an Enrolment Key.
    /// </summary>
    /// <param name="enrolmentKeyId">The Id of the Enrolment Key to enable.</param>
    /// <returns>A detailed Enrolment Key.</returns>
    Task<EnrolmentKeyModel> EnableAsync(EnrolmentKeyId enrolmentKeyId);

    /// <summary>
    /// Disable an Enrolment Key.
    /// </summary>
    /// <param name="enrolmentKeyId">The Id of the Enrolment Key to disable.</param>
    /// <returns>A detailed Enrolment Key.</returns>
    Task<EnrolmentKeyModel> DisableAsync(EnrolmentKeyId enrolmentKeyId);

    /// <summary>
    /// Bulk enable mutliple Enrolment Keys.
    /// </summary>
    /// <param name="enrolmentKeys">An array of Enrolment Key Ids to enable.</param>
    /// <returns>The number of keys modified.</returns>
    Task<int> BulkEnableAsync(params EnrolmentKeyId[] enrolmentKeys);

    /// <summary>
    /// Bulk enable mutliple Enrolment Keys.
    /// </summary>
    /// <param name="enrolmentKeys">An IEnumerable of Enrolment Key Ids to enable.</param>
    /// <returns>The number of keys modified.</returns>
    Task<int> BulkEnableAsync(IEnumerable<EnrolmentKeyId> enrolmentKeys);

    /// <summary>
    /// Bulk disable mutliple Enrolment Keys.
    /// </summary>
    /// <param name="enrolmentKeys">An array of Enrolment Key Ids to disable.</param>
    /// <returns>The number of keys modified.</returns>
    Task<int> BulkDisableAsync(params EnrolmentKeyId[] enrolmentKeys);

    /// <summary>
    /// Bulk disable mutliple Enrolment Keys.
    /// </summary>
    /// <param name="enrolmentKeys">An IEnumerable of Enrolment Key Ids to disable.</param>
    /// <returns>The number of keys modified.</returns>
    Task<int> BulkDisableAsync(IEnumerable<EnrolmentKeyId> enrolmentKeys);

    /// <summary>
    /// Enable this Enrolment Key for a specific period of time.
    /// </summary>
    /// <param name="enrolmentKeyId">The Id of the Enrolment Key to enable until.</param>
    /// <param name="expiryDateTime">A <see cref="DateTimeOffset"/> specifying the time when the <see cref="EnrolmentKeyModel"/> should be enabled until.</param>
    /// <param name="expiryAction">What should happen when the expiry date elapses.</param>
    /// <param name="timeZonedId">An IANA or Windows time zone ID. If this isn't null, expiryDateTime will be updated if the specified time zone's rules change.</param>
    /// <returns>A detailed Enrolment Key.</returns>
    Task<EnrolmentKeyModel> EnableUntilAsync(EnrolmentKeyId enrolmentKeyId, DateTimeOffset expiryDateTime, ExpiryAction expiryAction, string? timeZonedId = null);
}