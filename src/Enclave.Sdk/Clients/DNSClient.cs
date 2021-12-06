using System.Net.Http.Json;
using System.Web;
using Enclave.Sdk.Api.Clients.Interfaces;
using Enclave.Sdk.Api.Data;
using Enclave.Sdk.Api.Data.Dns;
using Enclave.Sdk.Api.Data.Pagination;
using Enclave.Sdk.Api.Data.PatchModel;

namespace Enclave.Sdk.Api.Clients;

/// <inheritdoc cref="IDnsClient" />
internal class DnsClient : ClientBase, IDnsClient
{
    private readonly string _orgRoute;

    /// <summary>
    /// Consutructor which will be called by <see cref="OrganisationClient"/> when it's created.
    /// </summary>
    /// <param name="httpClient">an instance of httpClient with a baseURL referencing the API.</param>
    /// <param name="orgRoute">The organisation API route.</param>
    public DnsClient(HttpClient httpClient, string orgRoute)
    : base(httpClient)
    {
        _orgRoute = orgRoute;
    }

    /// <inheritdoc/>
    public async Task<DnsSummary> GetPropertiesSummaryAsync()
    {
        var model = await HttpClient.GetFromJsonAsync<DnsSummary>($"{_orgRoute}/dns", Constants.JsonSerializerOptions);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public async Task<PaginatedResponseModel<DnsZoneSummary>> GetZonesAsync(int? pageNumber = null, int? perPage = null)
    {
        var queryString = BuildQueryString(null, null, pageNumber, perPage);

        var model = await HttpClient.GetFromJsonAsync<PaginatedResponseModel<DnsZoneSummary>>($"{_orgRoute}/dns/zones?{queryString}");

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public async Task<DnsZone> CreateZoneAsync(DnsZoneCreate createModel)
    {
        if (createModel is null)
        {
            throw new ArgumentNullException(nameof(createModel));
        }

        var result = await HttpClient.PostAsJsonAsync($"{_orgRoute}/dns/zones", createModel, Constants.JsonSerializerOptions);

        var model = await DeserialiseAsync<DnsZone>(result.Content);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public async Task<DnsZone> GetZoneAsync(DnsZoneId dnsZoneId)
    {
        var model = await HttpClient.GetFromJsonAsync<DnsZone>($"{_orgRoute}/dns/zones/{dnsZoneId}", Constants.JsonSerializerOptions);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public async Task<DnsZone> UpdateZoneAsync(DnsZoneId dnsZoneId, PatchBuilder<DnsZonePatch> builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        using var encoded = CreateJsonContent(builder.Send());
        var result = await HttpClient.PatchAsync($"{_orgRoute}/dns/zones/{dnsZoneId}", encoded);

        result.EnsureSuccessStatusCode();

        var model = await DeserialiseAsync<DnsZone>(result.Content);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public async Task<DnsZone> DeleteZoneAsync(DnsZoneId dnsZoneId)
    {
        var result = await HttpClient.DeleteAsync($"{_orgRoute}/dns/zones/{dnsZoneId}");

        result.EnsureSuccessStatusCode();

        var model = await DeserialiseAsync<DnsZone>(result.Content);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public async Task<PaginatedResponseModel<DnsRecordSummary>> GetRecordsAsync(
        DnsZoneId? dnsZoneId = null,
        string? hostname = null,
        int? pageNumber = null,
        int? perPage = null)
    {
        var queryString = BuildQueryString(dnsZoneId, hostname, pageNumber, perPage);

        var model = await HttpClient.GetFromJsonAsync<PaginatedResponseModel<DnsRecordSummary>>($"{_orgRoute}/dns/records?{queryString}");

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public async Task<DnsRecord> CreateRecordAsync(DnsRecordCreate createModel)
    {
        if (createModel is null)
        {
            throw new ArgumentNullException(nameof(createModel));
        }

        var result = await HttpClient.PostAsJsonAsync($"{_orgRoute}/dns/records", createModel, Constants.JsonSerializerOptions);

        var model = await DeserialiseAsync<DnsRecord>(result.Content);

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
    public async Task<DnsRecord> GetRecordAsync(DnsRecordId dnsRecordId)
    {
        var model = await HttpClient.GetFromJsonAsync<DnsRecord>($"{_orgRoute}/dns/records/{dnsRecordId}", Constants.JsonSerializerOptions);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public async Task<DnsRecord> UpdateRecordAsync(DnsRecordId dnsRecordId, PatchBuilder<DnsRecordPatch> builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        using var encoded = CreateJsonContent(builder.Send());
        var result = await HttpClient.PatchAsync($"{_orgRoute}/dns/records/{dnsRecordId}", encoded);

        result.EnsureSuccessStatusCode();

        var model = await DeserialiseAsync<DnsRecord>(result.Content);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public async Task<DnsRecord> DeleteRecordAsync(DnsRecordId dnsRecordId)
    {
        var result = await HttpClient.DeleteAsync($"{_orgRoute}/dns/records/{dnsRecordId}");

        result.EnsureSuccessStatusCode();

        var model = await DeserialiseAsync<DnsRecord>(result.Content);

        EnsureNotNull(model);

        return model;
    }

    private static string? BuildQueryString(DnsZoneId? dnsZoneId, string? hostname, int? pageNumber, int? perPage)
    {
        var queryString = HttpUtility.ParseQueryString(string.Empty);
        if (dnsZoneId is not null)
        {
            queryString.Add("zoneId", dnsZoneId.ToString());
        }

        if (hostname is not null)
        {
            queryString.Add("hostname", hostname);
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
