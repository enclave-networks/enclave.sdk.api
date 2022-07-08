using Enclave.Sdk.Api.Data;
using Enclave.Sdk.Api.Data.Pagination;
using Enclave.Sdk.Api.Data.PatchModel;
using Enclave.Sdk.Api.Data.Tags;

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
    Task<PaginatedResponseModel<BasicTag>> GetAsync(string? searchTerm = null, TagQuerySortOrder? sortOrder = null, int? pageNumber = null, int? perPage = null);

    /// <summary>
    /// Creates a Policy using a <see cref="TagCreate"/> Model.
    /// </summary>
    /// <param name="createModel">The model needed to create a Tag.</param>
    /// <returns>The created <see cref="DetailedTag"/>.</returns>
    Task<DetailedTag> CreateAsync(TagCreate createModel);

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
    /// <returns>The <see cref="DetailedTag"/>.</returns>
    Task<DetailedTag> GetAsync(TagRefId refId);

    /// <summary>
    /// Get a specific tag.
    /// </summary>
    /// <param name="tag">The tag name.</param>
    /// <returns>The <see cref="DetailedTag"/>.</returns>
    Task<DetailedTag> GetAsync(string tag);

    /// <summary>
    /// Starts an update patch request.
    /// </summary>
    /// <param name="refId">The TagRefId.</param>
    /// <returns>A PatchClient for fluent updating.</returns>
    IPatchClient<TagPatch, DetailedTag> Update(TagRefId refId);

    /// <summary>
    /// Starts an update patch request.
    /// </summary>
    /// <param name="tag">The tag name.</param>
    /// <returns>A PatchClient for fluent updating.</returns>
    IPatchClient<TagPatch, DetailedTag> Update(string tag);

    /// <summary>
    /// Delete a Tag.
    /// </summary>
    /// <param name="refId">The TagRefId of the Tag to delete.</param>
    /// <returns>The deleted <see cref="DetailedTag"/>.</returns>
    Task<DetailedTag> DeleteAsync(TagRefId refId);

    /// <summary>
    /// Delete a Tag.
    /// </summary>
    /// <param name="tag">The name of the Tag to delete.</param>
    /// <returns>The deleted <see cref="DetailedTag"/>.</returns>
    Task<DetailedTag> DeleteAsync(string tag);
}