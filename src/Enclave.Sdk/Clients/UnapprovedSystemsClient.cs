using System.Net.Http.Json;
using System.Web;
using Enclave.Api.Modules.SystemManagement.UnapprovedSystems.Models;
using Enclave.Api.Scaffolding.Pagination.Models;
using Enclave.Sdk.Api.Clients.Interfaces;
using Enclave.Sdk.Api.Data;

namespace Enclave.Sdk.Api.Clients;

/// <inheritdoc cref="IUnapprovedSystemsClient"/>
internal class UnapprovedSystemsClient : ClientBase, IUnapprovedSystemsClient
{
    private readonly string _orgRoute;

    /// <summary>
    /// This constructor is called by <see cref="EnclaveClient"/> when setting up the <see cref="UnapprovedSystemsClient"/>.
    /// It also calls the <see cref="ClientBase"/> constructor.
    /// </summary>
    /// <param name="httpClient">an instance of httpClient with a baseURL referencing the API.</param>
    /// <param name="orgRoute">The organisation API route.</param>
    public UnapprovedSystemsClient(HttpClient httpClient, string orgRoute)
    : base(httpClient)
    {
        _orgRoute = orgRoute;
    }

    /// <inheritdoc/>
    public async Task<PaginatedResponseModel<UnapprovedSystemSummaryModel>> GetSystemsAsync(
        int? enrolmentKeyId = null,
        string? searchTerm = null,
        UnapprovedSystemQuerySortMode? sortOrder = null,
        int? pageNumber = null,
        int? perPage = null)
    {
        var queryString = BuildQueryString(enrolmentKeyId, searchTerm, sortOrder, pageNumber, perPage);

        var model = await HttpClient.GetFromJsonAsync<PaginatedResponseModel<UnapprovedSystemSummaryModel>>($"{_orgRoute}/unapproved-systems?{queryString}", Constants.JsonSerializerOptions);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public async Task<int> DeclineSystems(params string[] systemIds)
    {
        using var content = CreateJsonContent(new
        {
            systemIds,
        });

        using var request = new HttpRequestMessage
        {
            Content = content,
            Method = HttpMethod.Delete,
            RequestUri = new Uri($"{HttpClient.BaseAddress}{_orgRoute}/unapproved-systems"),
        };

        var result = await HttpClient.SendAsync(request);

        result.EnsureSuccessStatusCode();

        var model = await DeserialiseAsync<BulkUnapprovedSystemDeclineResult>(result.Content);

        EnsureNotNull(model);

        return model.SystemsDeclined;
    }

    /// <inheritdoc/>
    public async Task<int> DeclineSystems(IEnumerable<string> systemIds)
    {
        return await DeclineSystems(systemIds.ToArray());
    }

    /// <inheritdoc/>
    public async Task<UnapprovedSystemModel> GetAsync(string systemId)
    {
        var model = await HttpClient.GetFromJsonAsync<UnapprovedSystemModel>($"{_orgRoute}/unapproved-systems/{systemId}", Constants.JsonSerializerOptions);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public IPatchClient<UnapprovedSystemPatchModel, UnapprovedSystemModel> Update(string systemId)
    {
        return new PatchClient<UnapprovedSystemPatchModel, UnapprovedSystemModel>(HttpClient, $"{_orgRoute}/unapproved-systems/{systemId}");
    }

    /// <inheritdoc/>
    public async Task<UnapprovedSystemModel> DeclineAsync(string systemId)
    {
        var result = await HttpClient.DeleteAsync($"{_orgRoute}/unapproved-systems/{systemId}");

        result.EnsureSuccessStatusCode();

        var model = await DeserialiseAsync<UnapprovedSystemModel>(result.Content);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public async Task ApproveAsync(string systemId)
    {
        var result = await HttpClient.PutAsync($"{_orgRoute}/unapproved-systems/{systemId}/approve", null);

        result.EnsureSuccessStatusCode();
    }

    /// <inheritdoc/>
    public async Task<int> ApproveSystemsAsync(params string[] systemIds)
    {
        var requestModel = new
        {
            systemIds,
        };

        var result = await HttpClient.PutAsJsonAsync($"{_orgRoute}/unapproved-systems/approve", requestModel, Constants.JsonSerializerOptions);

        result.EnsureSuccessStatusCode();

        var model = await DeserialiseAsync<BulkUnapprovedSystemApproveResult>(result.Content);

        EnsureNotNull(model);

        return model.SystemsApproved;
    }

    /// <inheritdoc/>
    public async Task<int> ApproveSystemsAsync(IEnumerable<string> systemIds)
    {
        return await ApproveSystemsAsync(systemIds.ToArray());
    }

    private static string? BuildQueryString(
        int? enrolmentKeyId,
        string? searchTerm,
        UnapprovedSystemQuerySortMode? sortOrder,
        int? pageNumber,
        int? perPage)
    {
        var queryString = HttpUtility.ParseQueryString(string.Empty);
        if (enrolmentKeyId is not null)
        {
            queryString.Add("enrolment_key", enrolmentKeyId.ToString());
        }

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