using Enclave.Sdk.Api.Data;
using Enclave.Sdk.Api.Data.Pagination;
using Enclave.Sdk.Api.Data.PatchModel;
using Enclave.Sdk.Api.Data.TrustRequirements;
using Enclave.Sdk.Api.Data.TrustRequirements.Enum;

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
    Task<PaginatedResponseModel<TrustRequirementSummary>> GetTrustRequirementsAsync(
        string? searchTerm = null,
        TrustRequirementSortOrder? sortOrder = null,
        int? pageNumber = null,
        int? perPage = null);

    /// <summary>
    /// Creates a Trust Requirement using a <see cref="TrustRequirementCreate"/> Model.
    /// </summary>
    /// <param name="createModel">The model needed to create a Trust Requirement.</param>
    /// <returns>The created <see cref="TrustRequirement"/>.</returns>
    Task<TrustRequirement> CreateAsync(TrustRequirementCreate createModel);

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
    /// <returns>The <see cref="TrustRequirement"/>.</returns>
    Task<TrustRequirement> GetAsync(TrustRequirementId requirementId);

    /// <summary>
    /// Starts an update patch request.
    /// </summary>
    /// <param name="requirementId">The TrustRequirementId to update.</param>
    /// <returns>A PatchClient for fluent updating.</returns>
    IPatchClient<TrustRequirementPatch, TrustRequirement> Update(TrustRequirementId requirementId);

    /// <summary>
    /// Delete a Trust Requirement.
    /// </summary>
    /// <param name="requirementId">The Id of the Trust Requirement to delete.</param>
    /// <returns>The deleted <see cref="TrustRequirement"/>.</returns>
    Task<TrustRequirement> DeleteAsync(TrustRequirementId requirementId);
}
