using System.Net.Http.Json;
using System.Web;
using Enclave.Api.Modules.SystemManagement.Common.Models;
using Enclave.Api.Modules.SystemManagement.Systems.Models;
using Enclave.Api.Scaffolding.Pagination.Models;
using Enclave.Configuration.Data.Enums;
using Enclave.Configuration.Data.Modules.Systems.Enums;
using Enclave.Sdk.Api.Clients.Interfaces;
using Enclave.Sdk.Api.Data;

namespace Enclave.Sdk.Api.Clients;

/// <inheritdoc cref="ISystemsClient" />
internal class SystemsClient : ClientBase, ISystemsClient
{
    private readonly string _orgRoute;

    /// <summary>
    /// Consutructor which will be called by <see cref="OrganisationClient"/> when it's created.
    /// </summary>
    /// <param name="httpClient">an instance of httpClient with a baseURL referencing the API.</param>
    /// <param name="orgRoute">The organisation API route.</param>
    public SystemsClient(HttpClient httpClient, string orgRoute)
    : base(httpClient)
    {
        _orgRoute = orgRoute;
    }

    /// <inheritdoc/>
    public async Task<PaginatedResponseModel<SystemSummaryModel>> GetSystemsAsync(
        int? enrolmentKeyId = null,
        string? searchTerm = null,
        bool? includeDisabled = null,
        SystemQuerySortMode? sortOrder = null,
        string? dnsName = null,
        int? pageNumber = null,
        int? perPage = null)
    {
        var queryString = BuildQueryString(enrolmentKeyId, searchTerm, includeDisabled, sortOrder, dnsName, pageNumber, perPage);

        var model = await HttpClient.GetFromJsonAsync<PaginatedResponseModel<SystemSummaryModel>>($"{_orgRoute}/systems?{queryString}", Constants.JsonSerializerOptions);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public async Task<int> RevokeSystemsAsync(params string[] systemIds)
    {
        using var content = CreateJsonContent(new
        {
            systemIds,
        });

        using var request = new HttpRequestMessage
        {
            Content = content,
            Method = HttpMethod.Delete,
            RequestUri = new Uri($"{HttpClient.BaseAddress}{_orgRoute}/systems"),
        };

        var result = await HttpClient.SendAsync(request);

        result.EnsureSuccessStatusCode();

        var model = await DeserialiseAsync<BulkSystemRevokedResult>(result.Content);

        EnsureNotNull(model);

        return model.SystemsRevoked;
    }

    /// <inheritdoc/>
    public async Task<int> RevokeSystemsAsync(IEnumerable<string> systemIds)
    {
        return await RevokeSystemsAsync(systemIds.ToArray());
    }

    /// <inheritdoc/>
    public async Task<SystemModel> GetAsync(string systemId)
    {
        var model = await HttpClient.GetFromJsonAsync<SystemModel>($"{_orgRoute}/systems/{systemId}", Constants.JsonSerializerOptions);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public IPatchClient<SystemPatchModel, SystemModel> Update(string systemId)
    {
        return new PatchClient<SystemPatchModel, SystemModel>(HttpClient, $"{_orgRoute}/systems/{systemId}");
    }

    /// <inheritdoc/>
    public async Task<SystemModel> RevokeAsync(string systemId)
    {
        var result = await HttpClient.DeleteAsync($"{_orgRoute}/systems/{systemId}");

        result.EnsureSuccessStatusCode();

        var model = await DeserialiseAsync<SystemModel>(result.Content);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public async Task<SystemModel> EnableAsync(string systemId)
    {
        var result = await HttpClient.PutAsync($"{_orgRoute}/systems/{systemId}/enable", null);

        var model = await DeserialiseAsync<SystemModel>(result.Content);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public async Task<SystemModel> DisableAsync(string systemId)
    {
        var result = await HttpClient.PutAsync($"{_orgRoute}/systems/{systemId}/disable", null);

        var model = await DeserialiseAsync<SystemModel>(result.Content);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public async Task<int> BulkEnableAsync(params string[] systemIds)
    {
        var requestModel = new
        {
            systemIds,
        };

        var result = await HttpClient.PutAsJsonAsync($"{_orgRoute}/systems/enable", requestModel, Constants.JsonSerializerOptions);

        result.EnsureSuccessStatusCode();

        var model = await DeserialiseAsync<BulkSystemUpdateResult>(result.Content);

        EnsureNotNull(model);

        return model.SystemsUpdated;
    }

    /// <inheritdoc/>
    public async Task<int> BulkEnableAsync(IEnumerable<string> systemIds)
    {
        return await BulkEnableAsync(systemIds.ToArray());
    }

    /// <inheritdoc/>
    public async Task<int> BulkDisableAsync(params string[] systemIds)
    {
        var requestModel = new
        {
            systemIds,
        };

        var result = await HttpClient.PutAsJsonAsync($"{_orgRoute}/systems/disable", requestModel, Constants.JsonSerializerOptions);

        result.EnsureSuccessStatusCode();

        var model = await DeserialiseAsync<BulkSystemUpdateResult>(result.Content);

        EnsureNotNull(model);

        return model.SystemsUpdated;
    }

    /// <inheritdoc/>
    public async Task<int> BulkDisableAsync(IEnumerable<string> systemIds)
    {
        return await BulkDisableAsync(systemIds.ToArray());
    }

    /// <inheritdoc/>
    public async Task<SystemModel> EnableUntilAsync(string systemId, DateTimeOffset expiryDateTime, ExpiryAction expiryAction, string? timeZonedId = null)
    {
        var requestModel = new AutoExpireModel(timeZonedId, expiryDateTime.ToString("o"), expiryAction);

        var result = await HttpClient.PutAsJsonAsync($"{_orgRoute}/systems/{systemId}/enable-until", requestModel, Constants.JsonSerializerOptions);

        result.EnsureSuccessStatusCode();

        var model = await DeserialiseAsync<SystemModel>(result.Content);

        EnsureNotNull(model);

        return model;
    }

    private static string? BuildQueryString(
        int? enrolmentKeyId = null,
        string? searchTerm = null,
        bool? includeDisabled = null,
        SystemQuerySortMode? sortOrder = null,
        string? dnsName = null,
        int? pageNumber = null,
        int? perPage = null)
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

        if (includeDisabled is not null)
        {
            queryString.Add("include_disabled", includeDisabled.ToString());
        }

        if (sortOrder is not null)
        {
            queryString.Add("sort", sortOrder.ToString());
        }

        if (dnsName is not null)
        {
            queryString.Add("dns", dnsName);
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
