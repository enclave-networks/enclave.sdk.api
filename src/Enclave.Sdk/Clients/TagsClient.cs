using Enclave.Sdk.Api.Clients.Interfaces;
using Enclave.Sdk.Api.Data.Pagination;
using Enclave.Sdk.Api.Data.Tags;
using System.Net.Http.Json;

namespace Enclave.Sdk.Api.Clients;

/// <inheritdoc cref="ITagsClient"/>
public class TagsClient : ClientBase, ITagsClient
{
    private string _orgRoute;

    /// <summary>
    /// Consutructor which will be called by <see cref="OrganisationClient"/> when it's created.
    /// </summary>
    /// <param name="httpClient">an instance of httpClient with a baseURL referencing the API.</param>
    /// <param name="orgRoute">the orgRoute which specifies the orgId.</param>
    public TagsClient(HttpClient httpClient, string orgRoute)
    : base(httpClient)
    {
        _orgRoute = orgRoute;
    }

    /// <inheritdoc/>
    public async Task<PaginatedResponseModel<TagItem>> GetAsync(string? searchTerm = null, TagQuerySortOrder? sortOrder = null, int? pageNumber = null, int? perPage = null)
    {
        var queryString = BuildQueryString(searchTerm, sortOrder, pageNumber, perPage);

        var model = await HttpClient.GetFromJsonAsync<PaginatedResponseModel<TagItem>>($"{_orgRoute}/tags?{queryString}");

        EnsureNotNull(model);

        return model;
    }

    private string? BuildQueryString(string? searchTerm, TagQuerySortOrder? sortOrder, int? pageNumber, int? perPage)
    {
        // TODO: encode values.
        var queryStringSet = false;
        string? queryString = default;
        if (searchTerm is not null)
        {
            queryString += $"search={searchTerm}";
            queryStringSet = true;
        }

        if (sortOrder is not null)
        {
            var delimiter = queryStringSet ? "&" : string.Empty;
            queryString += $"{delimiter}sort={sortOrder}";
            queryStringSet = true;
        }

        if (pageNumber is not null)
        {
            var delimiter = queryStringSet ? "&" : string.Empty;
            queryString += $"{delimiter}page={pageNumber}";
            queryStringSet = true;
        }

        if (perPage is not null)
        {
            var delimiter = queryStringSet ? "&" : string.Empty;
            queryString += $"{delimiter}per_page={perPage}";
        }

        return queryString;
    }
}
