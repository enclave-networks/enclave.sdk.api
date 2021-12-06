using Enclave.Sdk.Api.Data;
using Enclave.Sdk.Api.Data.Pagination;
using Enclave.Sdk.Api.Data.PatchModel;
using Enclave.Sdk.Api.Data.Policies;
using Enclave.Sdk.Api.Data.Policies.Enum;

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

    /// <summary>
    /// Delete multiple Policies.
    /// </summary>
    /// <param name="policyIds">The ids of the Policies you want to delete.</param>
    /// <returns>The number of deleted Policies.</returns>
    Task<int> DeletePoliciesAsync(params PolicyId[] policyIds);

    /// <summary>
    /// Get a specific Policy.
    /// </summary>
    /// <param name="policyId">The Id of the Policy to get.</param>
    /// <returns>The <see cref="Policy"/>.</returns>
    Task<Policy> GetAsync(PolicyId policyId);

    /// <summary>
    /// Patch request to update a Policy.
    /// </summary>
    /// <param name="policyId">The Id of the Policy to update.</param>
    /// <param cref="PatchBuilder{TModel}" name="builder">An instance of <see cref="PatchBuilder{TModel}"/> used to setup our patch request.</param>
    /// <returns>The updated <see cref="Policy"/>.</returns>
    Task<Policy> UpdateAsync(PolicyId policyId, PatchBuilder<PolicyPatch> builder);

    /// <summary>
    /// Delete a Policy.
    /// </summary>
    /// <param name="policyId">The Id of the Policy to delete.</param>
    /// <returns>The deleted <see cref="Policy"/>.</returns>
    Task<Policy> DeleteAsync(PolicyId policyId);

    /// <summary>
    /// Enable a Policy.
    /// </summary>
    /// <param name="policyId">The Id of the Policy to enable.</param>
    /// <returns>The enabled <see cref="Policy"/>.</returns>
    Task<Policy> EnableAsync(PolicyId policyId);

    /// <summary>
    /// Disable a Policy.
    /// </summary>
    /// <param name="policyId">The Id of the Policy to disable.</param>
    /// <returns>The disabled <see cref="Policy"/>.</returns>
    Task<Policy> DisableAsync(PolicyId policyId);

    /// <summary>
    /// Enable multiple Policies.
    /// </summary>
    /// <param name="policyIds">The ids of the Policies you want to enable.</param>
    /// <returns>The number of enabled Policies.</returns>
    Task<int> EnablePoliciesAsync(params PolicyId[] policyIds);

    /// <summary>
    /// Disable multiple Policies.
    /// </summary>
    /// <param name="policyIds">The ids of the Policies you want to disable.</param>
    /// <returns>The number of disabled Policies.</returns>
    Task<int> DisablePoliciesAsync(params PolicyId[] policyIds);
}