using System.Net.Http.Json;
using System.Web;
using Enclave.Sdk.Api.Clients.Interfaces;
using Enclave.Sdk.Api.Data.Dns;
using Enclave.Sdk.Api.Data.Pagination;

namespace Enclave.Sdk.Api.Clients;

/// <inheritdoc cref="IDnsClient" />
public class DnsClient : ClientBase, IDnsClient
{
    private string _orgRoute;

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
    public async Task<PaginatedResponseModel<BasicDnsZone>> GetZonesAsync(int? pageNumber = null, int? perPage = null)
    {
        var queryString = BuildQueryString(pageNumber, perPage);

        var model = await HttpClient.GetFromJsonAsync<PaginatedResponseModel<BasicDnsZone>>($"{_orgRoute}/dns/zones?{queryString}");

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public async Task<FullDnsZone> CreateAsync(DnsZoneCreate createModel)
    {
        if (createModel is null)
        {
            throw new ArgumentNullException(nameof(createModel));
        }

        var result = await HttpClient.PostAsJsonAsync($"{_orgRoute}/dns/zones", createModel, Constants.JsonSerializerOptions);

        var model = await DeserialiseAsync<FullDnsZone>(result.Content);

        EnsureNotNull(model);

        return model;
    }

    private static string? BuildQueryString(int? pageNumber, int? perPage)
    {
        var queryString = HttpUtility.ParseQueryString(string.Empty);
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
