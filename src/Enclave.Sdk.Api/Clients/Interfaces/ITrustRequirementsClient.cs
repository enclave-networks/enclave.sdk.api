using Enclave.Api.Modules.SystemManagement.TrustRequirements.Models;
using Enclave.Api.Scaffolding.Pagination.Models;
using Enclave.Configuration.Data.Identifiers;
using Enclave.Configuration.Data.Modules.TrustRequirements.Enums;
using Enclave.Sdk.Api.Data;

namespace Enclave.Sdk.Api.Clients.Interfaces;

/// <summary>
/// Provides operations to get, create, and manipulate Trust Requirement.
/// </summary>
public interface ITrustRequirementsClient
{
    /// <summary>
    /// Gets a paginated list of Trust Requirements which can be searched and iterated upon.
    /// </summary>
    /// <param name="searchTerm">A partial matching search term.</param>
    /// <param name="sortOrder">Sort order for the pagination.</param>
    /// <param name="pageNumber">Which page number do you want to return.</param>
    /// <param name="perPage">How many per page.</param>
    /// <returns>A paginated response model with links to get the previous, next, first and last pages.</returns>
    Task<PaginatedResponseModel<TrustRequirementSummaryModel>> GetTrustRequirementsAsync(
        string? searchTerm = null,
        TrustRequirementSortOrder? sortOrder = null,
        int? pageNumber = null,
        int? perPage = null);

    /// <summary>
    /// Creates a Trust Requirement using a <see cref="TrustRequirementCreateModel"/> Model.
    /// </summary>
    /// <param name="createModel">The model needed to create a Trust Requirement.</param>
    /// <returns>The created <see cref="TrustRequirementModel"/>.</returns>
    Task<TrustRequirementModel> CreateAsync(TrustRequirementCreateModel createModel);

    /// <summary>
    /// Delete multiple Trust Requirements.
    /// </summary>
    /// <param name="requirementIds">The ids of the Trust Requirement you want to delete.</param>
    /// <returns>The number of deleted Trust Requirements.</returns>
    Task<int> DeleteTrustRequirementsAsync(params TrustRequirementId[] requirementIds);

    /// <summary>
    /// Get a specific Trust Requirement.
    /// </summary>
    /// <param name="requirementId">The Id of the Trust Requirement to get.</param>
    /// <returns>The <see cref="TrustRequirementModel"/>.</returns>
    Task<TrustRequirementModel> GetAsync(TrustRequirementId requirementId);

    /// <summary>
    /// Starts an update patch request.
    /// </summary>
    /// <param name="requirementId">The TrustRequirementId to update.</param>
    /// <returns>A PatchClient for fluent updating.</returns>
    IPatchClient<TrustRequirementPatchModel, TrustRequirementModel> Update(TrustRequirementId requirementId);

    /// <summary>
    /// Delete a Trust Requirement.
    /// </summary>
    /// <param name="requirementId">The Id of the Trust Requirement to delete.</param>
    /// <returns>The deleted <see cref="TrustRequirementModel"/>.</returns>
    Task<TrustRequirementModel> DeleteAsync(TrustRequirementId requirementId);
}
