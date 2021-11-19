using Enclave.Sdk.Api.Data.Pagination;
using Enclave.Sdk.Api.Data.Tags;
using System.Text;

namespace Enclave.Sdk.Api.Clients;

public class TagsClient : ClientBase
{
    private string _orgRoute;

    public TagsClient(HttpClient httpClient, string orgRoute)
    : base(httpClient)
    {
        _orgRoute = orgRoute;
    }

    public async Task<PagninatedResponseModel<TagItem>> GetAsync(string? searchTerm = null, TagQuerySortOrder? sortOrder = null, int? pageNumber = null, int? perPage = null)
    {
        string? queryString = default;
        if (searchTerm is not null)
        {
            queryString += $"search={searchTerm}";
        }

        if (sortOrder is not null)
        {
            queryString += $"sort={sortOrder}";
        }

        if (pageNumber is not null)
        {
            queryString += $"page={pageNumber}";
        }

        if (perPage is not null)
        {
            queryString += $"per_page={perPage}";
        }

        var result = await HttpClient.GetAsync($"{_orgRoute}/tags?{queryString}");

        await CheckStatusCodes(result);

        var model = await DeserializeAsync<PagninatedResponseModel<TagItem>>(result.Content);

        CheckModel(model);

        return model;
    }
}
