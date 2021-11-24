using System.Net.Http.Headers;
using System.Net.Http.Json;
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
    /// Setup all requirements for making API calls.
    /// </summary>
    /// <param name="settings">optional set of settings should you need to configure the client further such as your own <see cref="HttpClient" />.</param>
    public EnclaveClient(EnclaveClientOptions? settings = default)
    {
        if (settings is null)
        {
            settings = GetSettingsFile();
        }

        _httpClient = settings?.HttpClient ?? new HttpClient();
        _httpClient.BaseAddress = new Uri(settings?.BaseUrl ?? FallbackUrl);

        if (!string.IsNullOrWhiteSpace(settings?.PersonalAccessToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", settings?.PersonalAccessToken);
        }

        var clientHeader = new ProductInfoHeaderValue("Enclave.Sdk.Api", Assembly.GetExecutingAssembly().GetName().Version?.ToString());
        _httpClient.DefaultRequestHeaders.UserAgent.Add(clientHeader);

        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
    }

    /// <summary>
    /// Gets a list of <see cref="AccountOrganisation"/> associated to the authorised user.
    /// </summary>
    /// <returns>List of organisation containing the OrgId and Name and the users role in that organisation.</returns>
    /// <exception cref="InvalidOperationException">throws when the Api returns a null response.</exception>
    public async Task<List<AccountOrganisation>> GetOrganisationsAsync()
    {
        var organisations = await _httpClient.GetFromJsonAsync<AccountOrganisationTopLevel>("/account/orgs", _jsonSerializerOptions);

        if (organisations is null)
        {
            throw new InvalidOperationException("Could not deserialize orgs associated to this token");
        }

        return organisations.Orgs;
    }

    /// <summary>
    /// Create an <see cref="OrganisationClient"/> from an <see cref="AccountOrganisation"/>.
    /// </summary>
    /// <param name="organisation">the <see cref="AccountOrganisation"/> from <see cref="GetOrganisationsAsync"/>.</param>
    /// <returns>OrganisationClient that can be used for all following Api Calls related to an organisation.</returns>
    public IOrganisationClient CreateOrganisationClient(AccountOrganisation organisation)
    {
        return new OrganisationClient(_httpClient, organisation);
    }

    private EnclaveClientOptions? GetSettingsFile()
    {
        var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        var location = Path.Combine(userProfile, ".encalve", "credentials.json");

        try
        {
            var json = File.ReadAllText(location);
            var settings = JsonSerializer.Deserialize<EnclaveClientOptions>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return settings;
        }
        catch (IOException)
        {
            return null;
        }
        catch
        {
            throw new ArgumentException("Can't find user settings file please refer to documentaiton for more information " +
                "or provide a bearer token in the constructor");
        }
    }
}