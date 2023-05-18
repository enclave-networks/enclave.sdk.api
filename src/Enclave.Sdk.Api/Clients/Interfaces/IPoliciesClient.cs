using Enclave.Api.Modules.SystemManagement.Policies.Models;
using Enclave.Api.Scaffolding.Pagination.Models;
using Enclave.Configuration.Data.Enums;
using Enclave.Configuration.Data.Identifiers;
using Enclave.Configuration.Data.Modules.Policies.Enums;
using Enclave.Sdk.Api.Data;

namespace Enclave.Sdk.Api.Clients.Interfaces;

/// <summary>
/// Provides operations to get, create, and manipulate Policies.
/// </summary>
public interface IPoliciesClient
{
    /// <summary>
    /// Gets a paginated list of Policies which can be searched and iterated upon.
    /// </summary>
    /// <param name="searchTerm">A partial matching search term.</param>
    /// <param name="includeDisabled">Include the disabled Policies in the results.</param>
    /// <param name="sortOrder">Sort order for the pagination.</param>
    /// <param name="pageNumber">Which page number do you want to return.</param>
    /// <param name="perPage">How many per page.</param>
    /// <returns>A paginated response model with links to get the previous, next, first and last pages.</returns>
    Task<PaginatedResponseModel<PolicyModel>> GetPoliciesAsync(
        string? searchTerm = null,
        bool? includeDisabled = null,
        PolicySortOrder? sortOrder = null,
        int? pageNumber = null,
        int? perPage = null);

    /// <summary>
    /// Creates a Policy using a <see cref="PolicyCreateModel"/> Model.
    /// </summary>
    /// <param name="createModel">The model needed to create a Policy.</param>
    /// <returns>The created <see cref="PolicyModel"/>.</returns>
    Task<PolicyModel> CreateAsync(PolicyCreateModel createModel);

    /// <summary>
    /// Delete multiple Policies.
    /// </summary>
    /// <param name="policyIds">The ids of the Policies you want to delete.</param>
    /// <returns>The number of deleted Policies.</returns>
    Task<int> DeletePoliciesAsync(params PolicyId[] policyIds);

    /// <summary>
    /// Delete multiple Policies.
    /// </summary>
    /// <param name="policyIds">The ids of the Policies you want to delete.</param>
    /// <returns>The number of deleted Policies.</returns>
    Task<int> DeletePoliciesAsync(IEnumerable<PolicyId> policyIds);

    /// <summary>
    /// Get a specific Policy.
    /// </summary>
    /// <param name="policyId">The Id of the Policy to get.</param>
    /// <returns>The <see cref="PolicyModel"/>.</returns>
    Task<PolicyModel> GetAsync(PolicyId policyId);

    /// <summary>
    /// Starts an update patch request.
    /// </summary>
    /// <param name="policyId">The PolicyId to update.</param>
    /// <returns>A PatchClient for fluent updating.</returns>
    IPatchClient<PolicyPatchModel, PolicyModel> Update(PolicyId policyId);

    /// <summary>
    /// Delete a Policy.
    /// </summary>
    /// <param name="policyId">The Id of the Policy to delete.</param>
    /// <returns>The deleted <see cref="PolicyModel"/>.</returns>
    Task<PolicyModel> DeleteAsync(PolicyId policyId);

    /// <summary>
    /// Enable a Policy.
    /// </summary>
    /// <param name="policyId">The Id of the Policy to enable.</param>
    /// <returns>The enabled <see cref="PolicyModel"/>.</returns>
    Task<PolicyModel> EnableAsync(PolicyId policyId);

    /// <summary>
    /// Disable a Policy.
    /// </summary>
    /// <param name="policyId">The Id of the Policy to disable.</param>
    /// <returns>The disabled <see cref="PolicyModel"/>.</returns>
    Task<PolicyModel> DisableAsync(PolicyId policyId);

    /// <summary>
    /// Enable multiple Policies.
    /// </summary>
    /// <param name="policyIds">The ids of the Policies you want to enable.</param>
    /// <returns>The number of enabled Policies.</returns>
    Task<int> EnablePoliciesAsync(params PolicyId[] policyIds);

    /// <summary>
    /// Enable multiple Policies.
    /// </summary>
    /// <param name="policyIds">The ids of the Policies you want to enable.</param>
    /// <returns>The number of enabled Policies.</returns>
    Task<int> EnablePoliciesAsync(IEnumerable<PolicyId> policyIds);

    /// <summary>
    /// Disable multiple Policies.
    /// </summary>
    /// <param name="policyIds">The ids of the Policies you want to disable.</param>
    /// <returns>The number of disabled Policies.</returns>
    Task<int> DisablePoliciesAsync(params PolicyId[] policyIds);

    /// <summary>
    /// Disable multiple Policies.
    /// </summary>
    /// <param name="policyIds">The ids of the Policies you want to disable.</param>
    /// <returns>The number of disabled Policies.</returns>
    Task<int> DisablePoliciesAsync(IEnumerable<PolicyId> policyIds);

    /// <summary>
    /// Enable this policy for a specific period of time.
    /// </summary>
    /// <param name="policyId">The Id of the policy to enable until.</param>
    /// <param name="expiryDateTime">A <see cref="DateTimeOffset"/> specifying the time when the <see cref="PolicyModel"/> should be enabled until.</param>
    /// <param name="expiryAction">What should happen when the expiry date elapses.</param>
    /// <param name="timeZonedId">An IANA or Windows time zone ID. If this isn't null, expiryDateTime will be updated if the specified time zone's rules change.</param>
    /// <returns>A detailed <see cref="PolicyModel"/>.</returns>
    Task<PolicyModel> EnableUntilAsync(PolicyId policyId, DateTimeOffset expiryDateTime, ExpiryAction expiryAction, string? timeZonedId = null);
}