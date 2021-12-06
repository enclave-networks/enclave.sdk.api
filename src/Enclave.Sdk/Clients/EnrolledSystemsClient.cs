using Enclave.Sdk.Api.Clients.Interfaces;
using Enclave.Sdk.Api.Data;
using Enclave.Sdk.Api.Data.EnrolledSystems;
using Enclave.Sdk.Api.Data.EnrolledSystems.Enum;
using Enclave.Sdk.Api.Data.Pagination;
using Enclave.Sdk.Api.Data.PatchModel;

namespace Enclave.Sdk.Api.Clients;

/// <inheritdoc cref="IEnrolledSystemsClient" />
public class EnrolledSystemsClient : ClientBase, IEnrolledSystemsClient
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

        var model = await HttpClient.GetFromJsonAsync<PaginatedResponseModel<EnrolledSystemSummary>>($"{_orgRoute}/systems?{queryString}");

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public async Task<int> RevokeSystems(params string[] systemIds)
    {
        using var content = CreateJsonContent(new
        {
            systemIds = systemIds,
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
    public async Task<EnrolledSystem> GetAsync(string systemId)
    {
        var model = await HttpClient.GetFromJsonAsync<EnrolledSystem>($"{_orgRoute}/systems/{systemId}", Constants.JsonSerializerOptions);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public async Task<EnrolledSystem> UpdateAsync(string systemId, PatchBuilder<SystemPatch> builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        using var encoded = CreateJsonContent(builder.Send());
        var result = await HttpClient.PatchAsync($"{_orgRoute}/systems/{systemId}", encoded);

        result.EnsureSuccessStatusCode();

        var model = await DeserialiseAsync<EnrolledSystem>(result.Content);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public async Task<EnrolledSystem> RevokeAsync(string systemId)
    {
        var result = await HttpClient.DeleteAsync($"{_orgRoute}/systems/{systemId}");

        result.EnsureSuccessStatusCode();

        var model = await DeserialiseAsync<EnrolledSystem>(result.Content);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public async Task<EnrolledSystem> EnableAsync(int systemId)
    {
        var result = await HttpClient.PutAsync($"{_orgRoute}/systems/{systemId}/enable", null);

        var model = await DeserialiseAsync<EnrolledSystem>(result.Content);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public async Task<EnrolledSystem> DisableAsync(int systemId)
    {
        var result = await HttpClient.PutAsync($"{_orgRoute}/systems/{systemId}/disable", null);

        var model = await DeserialiseAsync<EnrolledSystem>(result.Content);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public async Task<int> BulkEnableAsync(params string[] systemIds)
    {
        var requestModel = new
        {
            systemIds = systemIds,
        };

        var result = await HttpClient.PutAsJsonAsync($"{_orgRoute}/systems/enable", requestModel, Constants.JsonSerializerOptions);

        result.EnsureSuccessStatusCode();

        var model = await DeserialiseAsync<BulkSystemUpdateResult>(result.Content);

        EnsureNotNull(model);

        return model.SystemsUpdated;
    }

    /// <inheritdoc/>
    public async Task<int> BulkDisableAsync(params string[] systemIds)
    {
        var requestModel = new
        {
            systemIds = systemIds,
        };

        var result = await HttpClient.PutAsJsonAsync($"{_orgRoute}/systems/disable", requestModel, Constants.JsonSerializerOptions);

        result.EnsureSuccessStatusCode();

        var model = await DeserialiseAsync<BulkSystemUpdateResult>(result.Content);

        EnsureNotNull(model);

        return model.SystemsUpdated;
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
