using System.Net.Http.Json;
using System.Web;
using Enclave.Api.Modules.SystemManagement.Tags.Models;
using Enclave.Api.Scaffolding.Pagination.Models;
using Enclave.Configuration.Data.Identifiers;
using Enclave.Configuration.Data.Modules.Tags.Enums;
using Enclave.Sdk.Api.Clients.Interfaces;
using Enclave.Sdk.Api.Data;

namespace Enclave.Sdk.Api.Clients;

/// <inheritdoc cref="ITagsClient"/>
internal class TagsClient : ClientBase, ITagsClient
{
    private readonly string _orgRoute;

    /// <summary>
    /// Constructor which will be called by <see cref="OrganisationClient"/> when it's created.
    /// </summary>
    /// <param name="httpClient">an instance of httpClient with a baseURL referencing the API.</param>
    /// <param name="orgRoute">the orgRoute which specifies the orgId.</param>
    public TagsClient(HttpClient httpClient, string orgRoute)
    : base(httpClient)
    {
        _orgRoute = orgRoute;
    }

    /// <inheritdoc/>
    public async Task<PaginatedResponseModel<TagSummaryModel>> GetAsync(string? searchTerm = null, TagQuerySortOrder? sortOrder = null, int? pageNumber = null, int? perPage = null)
    {
        var queryString = BuildQueryString(searchTerm, sortOrder, pageNumber, perPage);

        var model = await HttpClient.GetFromJsonAsync<PaginatedResponseModel<TagSummaryModel>>($"{_orgRoute}/tags?{queryString}", Constants.JsonSerializerOptions);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public async Task<TagModel> CreateAsync(TagCreateModel createModel)
    {
        if (createModel is null)
        {
            throw new ArgumentNullException(nameof(createModel));
        }

        var result = await HttpClient.PostAsJsonAsync($"{_orgRoute}/tags", createModel, Constants.JsonSerializerOptions);

        var model = await DeserialiseAsync<TagModel>(result.Content);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public async Task<int> DeleteTagsAsync(params string[] tagNamesOrRef)
    {
        using var content = CreateJsonContent(new
        {
            tags = tagNamesOrRef,
        });

        using var request = new HttpRequestMessage
        {
            Content = content,
            Method = HttpMethod.Delete,
            RequestUri = new Uri($"{HttpClient.BaseAddress}{_orgRoute}/tags"),
        };

        var result = await HttpClient.SendAsync(request);

        result.EnsureSuccessStatusCode();

        var model = await DeserialiseAsync<BulkTagDeleteResult>(result.Content);

        EnsureNotNull(model);

        return model.TagsDeleted;
    }

    /// <inheritdoc/>
    public async Task<TagModel> GetAsync(TagRefId refId)
    {
        return await GetAsync(refId.ToString());
    }

    /// <inheritdoc/>
    public async Task<TagModel> GetAsync(string tag)
    {
        var model = await HttpClient.GetFromJsonAsync<TagModel>($"{_orgRoute}/tags/{tag}", Constants.JsonSerializerOptions);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public IPatchClient<TagPatchModel, TagModel> Update(TagRefId refId)
    {
        return Update(refId.ToString());
    }

    /// <inheritdoc/>
    public IPatchClient<TagPatchModel, TagModel> Update(string tag)
    {
        return new PatchClient<TagPatchModel, TagModel>(HttpClient, $"{_orgRoute}/tags/{tag}");
    }

    /// <inheritdoc/>
    public async Task<TagModel> DeleteAsync(TagRefId refId)
    {
        return await DeleteAsync(refId.ToString());
    }

    /// <inheritdoc/>
    public async Task<TagModel> DeleteAsync(string tag)
    {
        var result = await HttpClient.DeleteAsync($"{_orgRoute}/tags/{tag}");

        result.EnsureSuccessStatusCode();

        var model = await DeserialiseAsync<TagModel>(result.Content);

        EnsureNotNull(model);

        return model;
    }

    private static string? BuildQueryString(string? searchTerm, TagQuerySortOrder? sortOrder, int? pageNumber, int? perPage)
    {
        var queryString = HttpUtility.ParseQueryString(string.Empty);
        if (searchTerm is not null)
        {
            queryString.Add("search", searchTerm);
        }

        if (sortOrder is not null)
        {
            queryString.Add("sort", sortOrder.ToString());
        }

        if (pageNumber is not null)
        {
            queryString.Add("page", pageNumber.ToString());
        }

        if (perPage is not null)
        {
            queryString.Add("per_page", perPage.ToString());
        }

        return queryString.ToString();
    }
}
