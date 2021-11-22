using Enclave.Sdk.Api.Clients;
using Enclave.Sdk.Api.Clients.Interfaces;
using Enclave.Sdk.Api.Data.Account;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.Json;

namespace Enclave.Sdk.Api;

public class EnclaveClient
{
    public OrganisationClient? Organisation { get; private set; }

    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    // TODO Make HttpClient and BaseUrl optional and for token pull it from .enclave folder if it's not supplied
    public EnclaveClient(HttpClient httpClient, string baseUrl, string token)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(baseUrl);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var clientHeader = new ProductInfoHeaderValue("SDK", Assembly.GetExecutingAssembly().GetName().Version?.ToString());
        _httpClient.DefaultRequestHeaders.UserAgent.Add(clientHeader);

        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
    }

    public async Task<List<AccountOrganisation>> GetOrganisationsAsync()
    {
        var result = await _httpClient.GetAsync("/account/orgs");

        if (result is null)
        {
            throw new InvalidOperationException("Did not get any response");
        }

        result.EnsureSuccessStatusCode();

        var contentStream = await result.Content.ReadAsStreamAsync();
        var organisations = await JsonSerializer.DeserializeAsync<AccountOrganisationTopLevel>(contentStream, _jsonSerializerOptions);

        if (organisations is null)
        {
            throw new InvalidOperationException("Could not deserialize orgs associated to this token");
        }

        return organisations.Orgs;
    }

    public IOrganisationClient CreateOrganisationClient(AccountOrganisation organisation)
    {
        return new OrganisationClient(_httpClient, organisation);
    }
}