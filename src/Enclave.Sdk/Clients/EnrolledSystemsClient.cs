using System.Net.Http.Json;
using System.Web;
using Enclave.Sdk.Api.Clients.Interfaces;
using Enclave.Sdk.Api.Data;
using Enclave.Sdk.Api.Data.AutoExpiry;
using Enclave.Sdk.Api.Data.EnrolledSystems;
using Enclave.Sdk.Api.Data.EnrolledSystems.Enum;
using Enclave.Sdk.Api.Data.EnrolmentKeys;
using Enclave.Sdk.Api.Data.Pagination;
using Enclave.Sdk.Api.Data.PatchModel;

namespace Enclave.Sdk.Api.Clients;

/// <inheritdoc cref="IEnrolledSystemsClient" />
internal class EnrolledSystemsClient : ClientBase, IEnrolledSystemsClient
{
    private readonly string _orgRoute;

    /// <summary>
    /// Consutructor which will be called by <see cref="OrganisationClient"/> when it's created.
    /// </summary>
    /// <param name="httpClient">an instance of httpClient with a baseURL referencing the API.</param>
    /// <param name="orgRoute">The organisation API route.</param>
    public EnrolledSystemsClient(HttpClient httpClient, string orgRoute)
    : base(httpClient)
    {
        _orgRoute = orgRoute;
    }

    /// <inheritdoc/>
    public async Task<PaginatedResponseModel<EnrolledSystemSummary>> GetSystemsAsync(
        int? enrolmentKeyId = null,
        string? searchTerm = null,
        bool? includeDisabled = null,
        SystemQuerySortMode? sortOrder = null,
        string? dnsName = null,
        int? pageNumber = null,
        int? perPage = null)
    {
        var queryString = BuildQueryString(enrolmentKeyId, searchTerm, includeDisabled, sortOrder, dnsName, pageNumber, perPage);

        var model = await HttpClient.GetFromJsonAsync<PaginatedResponseModel<EnrolledSystemSummary>>($"{_orgRoute}/systems?{queryString}", Constants.JsonSerializerOptions);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public async Task<int> RevokeSystemsAsync(params SystemId[] systemIds)
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
    public async Task<int> RevokeSystemsAsync(IEnumerable<SystemId> systemIds)
    {
        return await RevokeSystemsAsync(systemIds.ToArray());
    }

    /// <inheritdoc/>
    public async Task<EnrolledSystem> GetAsync(SystemId systemId)
    {
        var model = await HttpClient.GetFromJsonAsync<EnrolledSystem>($"{_orgRoute}/systems/{systemId}", Constants.JsonSerializerOptions);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public IPatchClient<EnrolledSystemPatch, EnrolledSystem> Update(SystemId systemId)
    {
        return new PatchClient<EnrolledSystemPatch, EnrolledSystem>(HttpClient, $"{_orgRoute}/systems/{systemId}");
    }

    /// <inheritdoc/>
    public async Task<EnrolledSystem> RevokeAsync(SystemId systemId)
    {
        var result = await HttpClient.DeleteAsync($"{_orgRoute}/systems/{systemId}");

        result.EnsureSuccessStatusCode();

        var model = await DeserialiseAsync<EnrolledSystem>(result.Content);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public async Task<EnrolledSystem> EnableAsync(SystemId systemId)
    {
        var result = await HttpClient.PutAsync($"{_orgRoute}/systems/{systemId}/enable", null);

        var model = await DeserialiseAsync<EnrolledSystem>(result.Content);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public async Task<EnrolledSystem> DisableAsync(SystemId systemId)
    {
        var result = await HttpClient.PutAsync($"{_orgRoute}/systems/{systemId}/disable", null);

        var model = await DeserialiseAsync<EnrolledSystem>(result.Content);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public async Task<int> BulkEnableAsync(params SystemId[] systemIds)
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
    public async Task<int> BulkEnableAsync(IEnumerable<SystemId> systemIds)
    {
        return await BulkEnableAsync(systemIds.ToArray());
    }

    /// <inheritdoc/>
    public async Task<int> BulkDisableAsync(params SystemId[] systemIds)
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
    public async Task<int> BulkDisableAsync(IEnumerable<SystemId> systemIds)
    {
        return await BulkDisableAsync(systemIds.ToArray());
    }

    /// <inheritdoc/>
    public async Task<EnrolledSystem> EnableUntilAsync(SystemId systemId, DateTimeOffset expiryDateTime, ExpiryAction expiryAction, string? timeZonedId = null)
    {
        var requestModel = new AutoExpire
        {
            ExpiryDateTime = expiryDateTime.ToString("o"),
            ExpiryAction = expiryAction,
            TimeZoneId = timeZonedId,
        };

        var result = await HttpClient.PutAsJsonAsync($"{_orgRoute}/systems/{systemId}/enable-until", requestModel, Constants.JsonSerializerOptions);

        result.EnsureSuccessStatusCode();

        var model = await DeserialiseAsync<EnrolledSystem>(result.Content);

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
