using Enclave.Api.Modules.SystemManagement.Tags.Models;
using Enclave.Api.Scaffolding.Pagination.Models;
using Enclave.Configuration.Data.Identifiers;
using Enclave.Configuration.Data.Modules.Tags.Enums;
using Enclave.Sdk.Api.Data;

namespace Enclave.Sdk.Api.Clients.Interfaces;

/// <summary>
/// Access and search for tags.
/// </summary>
public interface ITagsClient
{
    /// <summary>
    /// Gets a paginated list of tags which can be searched and interated upon.
    /// </summary>
    /// <param name="searchTerm">A partial matching search term.</param>
    /// <param name="sortOrder">Sort order for the pagination.</param>
    /// <param name="pageNumber">Which page number do you want to return.</param>
    /// <param name="perPage">How many per page.</param>
    /// <returns>A paginated response model with links to get the previous, next, first and last pages.</returns>
    Task<PaginatedResponseModel<TagSummaryModel>> GetAsync(string? searchTerm = null, TagQuerySortOrder? sortOrder = null, int? pageNumber = null, int? perPage = null);

    /// <summary>
    /// Creates a Policy using a <see cref="TagCreateModel"/> Model.
    /// </summary>
    /// <param name="createModel">The model needed to create a Tag.</param>
    /// <returns>The created <see cref="TagModel"/>.</returns>
    Task<TagModel> CreateAsync(TagCreateModel createModel);

    /// <summary>
    /// Delete multiple Tags. By ref or tag name.
    /// </summary>
    /// <param name="tagNamesOrRef">A set of tag names or refIds.</param>
    /// <returns>The number of deleted tags.</returns>
    Task<int> DeleteTagsAsync(params string[] tagNamesOrRef);

    /// <summary>
    /// Get a specific tag.
    /// </summary>
    /// <param name="refId">The TagRefId.</param>
    /// <returns>The <see cref="TagModel"/>.</returns>
    Task<TagModel> GetAsync(TagRefId refId);

    /// <summary>
    /// Get a specific tag.
    /// </summary>
    /// <param name="tag">The tag name.</param>
    /// <returns>The <see cref="TagModel"/>.</returns>
    Task<TagModel> GetAsync(string tag);

    /// <summary>
    /// Starts an update patch request.
    /// </summary>
    /// <param name="refId">The TagRefId.</param>
    /// <returns>A PatchClient for fluent updating.</returns>
    IPatchClient<TagPatchModel, TagModel> Update(TagRefId refId);

    /// <summary>
    /// Starts an update patch request.
    /// </summary>
    /// <param name="tag">The tag name.</param>
    /// <returns>A PatchClient for fluent updating.</returns>
    IPatchClient<TagPatchModel, TagModel> Update(string tag);

    /// <summary>
    /// Delete a Tag.
    /// </summary>
    /// <param name="refId">The TagRefId of the Tag to delete.</param>
    /// <returns>The deleted <see cref="TagModel"/>.</returns>
    Task<TagModel> DeleteAsync(TagRefId refId);

    /// <summary>
    /// Delete a Tag.
    /// </summary>
    /// <param name="tag">The name of the Tag to delete.</param>
    /// <returns>The deleted <see cref="TagModel"/>.</returns>
    Task<TagModel> DeleteAsync(string tag);
}