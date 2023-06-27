using System.Net.Http.Json;
using System.Web;
using Enclave.Api.Modules.SystemManagement.Dns.Models;
using Enclave.Api.Scaffolding.Pagination.Models;
using Enclave.Configuration.Data.Identifiers;
using Enclave.Sdk.Api.Clients.Interfaces;
using Enclave.Sdk.Api.Data;

namespace Enclave.Sdk.Api.Clients;

/// <inheritdoc cref="IDnsClient" />
internal class DnsClient : ClientBase, IDnsClient
{
    private readonly string _orgRoute;

    /// <summary>
    /// Constructor which will be called by <see cref="OrganisationClient"/> when it's created.
    /// </summary>
    /// <param name="httpClient">an instance of httpClient with a baseURL referencing the API.</param>
    /// <param name="orgRoute">The organisation API route.</param>
    public DnsClient(HttpClient httpClient, string orgRoute)
    : base(httpClient)
    {
        _orgRoute = orgRoute;
    }

    /// <inheritdoc/>
    public async Task<DnsSummaryModel> GetPropertiesSummaryAsync()
    {
        var model = await HttpClient.GetFromJsonAsync<DnsSummaryModel>($"{_orgRoute}/dns", Constants.JsonSerializerOptions);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public async Task<PaginatedResponseModel<DnsZoneSummaryModel>> GetZonesAsync(int? pageNumber = null, int? perPage = null)
    {
        var queryString = BuildQueryString(null, null, pageNumber, perPage);

        var model = await HttpClient.GetFromJsonAsync<PaginatedResponseModel<DnsZoneSummaryModel>>($"{_orgRoute}/dns/zones?{queryString}", Constants.JsonSerializerOptions);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public async Task<DnsZoneModel> CreateZoneAsync(DnsZoneCreateModel createModel)
    {
        if (createModel is null)
        {
            throw new ArgumentNullException(nameof(createModel));
        }

        var result = await HttpClient.PostAsJsonAsync($"{_orgRoute}/dns/zones", createModel, Constants.JsonSerializerOptions);

        var model = await DeserialiseAsync<DnsZoneModel>(result.Content);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public async Task<DnsZoneModel> GetZoneAsync(DnsZoneId dnsZoneId)
    {
        var model = await HttpClient.GetFromJsonAsync<DnsZoneModel>($"{_orgRoute}/dns/zones/{dnsZoneId}", Constants.JsonSerializerOptions);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public IPatchClient<DnsZonePatchModel, DnsZoneModel> UpdateZone(DnsZoneId dnsZoneId)
    {
        return new PatchClient<DnsZonePatchModel, DnsZoneModel>(HttpClient, $"{_orgRoute}/dns/zones/{dnsZoneId}");
    }

    /// <inheritdoc/>
    public async Task<DnsZoneModel> DeleteZoneAsync(DnsZoneId dnsZoneId)
    {
        var result = await HttpClient.DeleteAsync($"{_orgRoute}/dns/zones/{dnsZoneId}");

        result.EnsureSuccessStatusCode();

        var model = await DeserialiseAsync<DnsZoneModel>(result.Content);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public async Task<PaginatedResponseModel<DnsRecordSummaryModel>> GetRecordsAsync(
        DnsZoneId? dnsZoneId = null,
        string? searchTerm = null,
        int? pageNumber = null,
        int? perPage = null)
    {
        var queryString = BuildQueryString(dnsZoneId, searchTerm, pageNumber, perPage);

        var model = await HttpClient.GetFromJsonAsync<PaginatedResponseModel<DnsRecordSummaryModel>>($"{_orgRoute}/dns/records?{queryString}", Constants.JsonSerializerOptions);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public async Task<DnsRecordModel> CreateRecordAsync(DnsRecordCreateModel createModel)
    {
        if (createModel is null)
        {
            throw new ArgumentNullException(nameof(createModel));
        }

        var result = await HttpClient.PostAsJsonAsync($"{_orgRoute}/dns/records", createModel, Constants.JsonSerializerOptions);

        var model = await DeserialiseAsync<DnsRecordModel>(result.Content);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public async Task<int> DeleteRecordsAsync(params DnsRecordId[] records)
    {
        using var content = CreateJsonContent(new
        {
            recordIds = records,
        });

        using var request = new HttpRequestMessage
        {
            Content = content,
            Method = HttpMethod.Delete,
            RequestUri = new Uri($"{HttpClient.BaseAddress}{_orgRoute}/dns/records"),
        };

        var result = await HttpClient.SendAsync(request);

        result.EnsureSuccessStatusCode();

        var model = await DeserialiseAsync<BulkDnsRecordDeleteResult>(result.Content);

        EnsureNotNull(model);

        return model.DnsRecordsDeleted;
    }

    /// <inheritdoc/>
    public async Task<int> DeleteRecordsAsync(IEnumerable<DnsRecordId> records)
    {
        if (records is null)
        {
            throw new ArgumentNullException(nameof(records));
        }

        return await DeleteRecordsAsync(records.ToArray());
    }

    /// <inheritdoc/>
    public async Task<DnsRecordModel> GetRecordAsync(DnsRecordId dnsRecordId)
    {
        var model = await HttpClient.GetFromJsonAsync<DnsRecordModel>($"{_orgRoute}/dns/records/{dnsRecordId}", Constants.JsonSerializerOptions);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public IPatchClient<DnsRecordPatchModel, DnsRecordModel> UpdateRecord(DnsRecordId dnsRecordId)
    {
        return new PatchClient<DnsRecordPatchModel, DnsRecordModel>(HttpClient, $"{_orgRoute}/dns/records/{dnsRecordId}");
    }

    /// <inheritdoc/>
    public async Task<DnsRecordModel> DeleteRecordAsync(DnsRecordId dnsRecordId)
    {
        var result = await HttpClient.DeleteAsync($"{_orgRoute}/dns/records/{dnsRecordId}");

        result.EnsureSuccessStatusCode();

        var model = await DeserialiseAsync<DnsRecordModel>(result.Content);

        EnsureNotNull(model);

        return model;
    }

    private static string? BuildQueryString(DnsZoneId? dnsZoneId, string? searchTerm, int? pageNumber, int? perPage)
    {
        var queryString = HttpUtility.ParseQueryString(string.Empty);
        if (dnsZoneId is not null)
        {
            queryString.Add("zoneId", dnsZoneId.ToString());
        }

        if (searchTerm is not null)
        {
            queryString.Add("search", searchTerm);
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
