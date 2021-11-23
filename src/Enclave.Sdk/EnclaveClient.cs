using System.Net.Http.Headers;
using System.Reflection;
using System.Text.Json;
using Enclave.Sdk.Api.Clients;
using Enclave.Sdk.Api.Clients.Interfaces;
using Enclave.Sdk.Api.Data;
using Enclave.Sdk.Api.Data.Account;

namespace Enclave.Sdk.Api;

/// <summary>
/// Our main entry point for all API work.
/// </summary>
public class EnclaveClient
{
    private const string FallbackUrl = "https://api.enclave.io/";

    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    /// <summary>
    /// Setup all requirments for making api calls.
    /// </summary>
    /// <param name="settings">optional set of settings should you need to configure the client further such as your own httpClient.</param>
    public EnclaveClient(EnclaveSettings? settings = default)
    {
        if (settings is null)
        {
            settings = GetSettingsFile();
        }

        _httpClient = settings?.HttpClient ?? new HttpClient();
        _httpClient.BaseAddress = new Uri(settings?.BaseUrl ?? FallbackUrl);

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", settings?.BearerToken);
        var clientHeader = new ProductInfoHeaderValue("SDK", Assembly.GetExecutingAssembly().GetName().Version?.ToString());
        _httpClient.DefaultRequestHeaders.UserAgent.Add(clientHeader);

        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
    }

    /// <summary>
    /// Gets a list of organisation associated to the authorised user.
    /// </summary>
    /// <returns>List of organisation containing the OrgId and Name and the users role in that organisation.</returns>
    /// <exception cref="InvalidOperationException">throws when the Api returns a null response.</exception>
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

    /// <summary>
    /// Create an organisationClient from an AccountOrganisation.
    /// </summary>
    /// <param name="organisation">the AccountOrganisation from GetOrganisationsAsync.</param>
    /// <returns>OrganisationClient that can be used for all following Api Calls related to an organisation.</returns>
    public IOrganisationClient CreateOrganisationClient(AccountOrganisation organisation)
    {
        return new OrganisationClient(_httpClient, organisation);
    }

    private EnclaveSettings? GetSettingsFile()
    {
        var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var location = $"{userProfile}\\.enclave";

        try
        {
            using var streamReader = new StreamReader($"{location}\\credentials.json");
            var json = streamReader.ReadToEnd();
            var settings = JsonSerializer.Deserialize<EnclaveSettings>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return settings;
        }
        catch
        {
            throw new ArgumentException("Can't find user settings file please refer to documentaiton for more information " +
                "or provide a bearer token in the constructor");
        }
    }
}