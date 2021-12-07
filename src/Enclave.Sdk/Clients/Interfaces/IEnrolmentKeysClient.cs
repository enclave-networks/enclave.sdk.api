using Enclave.Sdk.Api.Data;
using Enclave.Sdk.Api.Data.EnrolmentKeys;
using Enclave.Sdk.Api.Data.EnrolmentKeys.Enum;
using Enclave.Sdk.Api.Data.Pagination;
using Enclave.Sdk.Api.Data.PatchModel;

namespace Enclave.Sdk.Api.Clients.Interfaces;

/// <summary>
/// Perform CRUD operations on Enrolment Keys.
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
    Task<PaginatedResponseModel<SimpleEnrolmentKey>> GetEnrolmentKeysAsync(string? searchTerm = null, bool? includeDisabled = null, EnrolmentKeySortOrder? sortOrder = null, int? pageNumber = null, int? perPage = null);

    /// <summary>
    /// Creates an Enrolment Key using a <see cref="EnrolmentKeyCreate"/> Model.
    /// </summary>
    /// <param name="createModel">The model needed to create an Enrolment Key.</param>
    /// <returns>The created Enrolment Key as a <see cref="FullEnrolmentKey"/>.</returns>
    Task<FullEnrolmentKey> CreateAsync(EnrolmentKeyCreate createModel);

    /// <summary>
    /// Gets a detailed Enrolment Key model.
    /// </summary>
    /// <param name="enrolmentKeyId">The Id of the Enrolment Key to get.</param>
    /// <returns>A detailed Enrolment Key.</returns>
    Task<FullEnrolmentKey> GetAsync(int enrolmentKeyId);

    /// <summary>
    /// Patch request to update the EnrolmentKey.
    /// Use <see cref="PatchBuilder{TModel}"/>.
    /// </summary>
    /// <param name="enrolmentKeyId">The Id of the Enrolment Key to update.</param>
    /// <param cref="PatchBuilder{TModel}" name="builder">An instance of <see cref="PatchBuilder{TModel}"/> used to setup our patch request.</param>
    /// <returns>A detailed Enrolment Key.</returns>
    Task<FullEnrolmentKey> UpdateAsync(int enrolmentKeyId, PatchBuilder<EnrolmentKeyPatchModel> builder);

    /// <summary>
    /// Enable an Enrolment Key.
    /// </summary>
    /// <param name="enrolmentKeyId">The Id of the Enrolment Key to enable.</param>
    /// <returns>A detailed Enrolment Key.</returns>
    Task<FullEnrolmentKey> EnableAsync(int enrolmentKeyId);

    /// <summary>
    /// Disable an Enrolment Key.
    /// </summary>
    /// <param name="enrolmentKeyId">The Id of the Enrolment Key to disable.</param>
    /// <returns>A detailed Enrolment Key.</returns>
    Task<FullEnrolmentKey> DisableAsync(int enrolmentKeyId);

    /// <summary>
    /// Bulk enable mutliple Enrolment Keys.
    /// </summary>
    /// <param name="enrolmentKeys">An array of Enrolment Key Ids to enable.</param>
    /// <returns>The number of keys modified.</returns>
    Task<int> BulkEnableAsync(params int[] enrolmentKeys);

    /// <summary>
    /// Bulk disable mutliple Enrolment Keys.
    /// </summary>
    /// <param name="enrolmentKeys">An array of Enrolment Key Ids to disable.</param>
    /// <returns>The number of keys modified.</returns>
    Task<int> BulkDisableAsync(params int[] enrolmentKeys);
}