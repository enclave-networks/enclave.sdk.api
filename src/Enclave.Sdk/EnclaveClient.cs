using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json;
using Enclave.Sdk.Api.Clients;
using Enclave.Sdk.Api.Clients.Interfaces;
using Enclave.Sdk.Api.Data.Account;
using Enclave.Sdk.Api.Handlers;

namespace Enclave.Sdk.Api;

/// <summary>
/// Our main entry point for all API work.
/// </summary>
public class EnclaveClient
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Create an <see cref="EnclaveClient"/> using settings found in the .enclave/credentials.json file in your user directory.
    /// </summary>
    /// <exception cref="ArgumentNullException">Throws if options in file are null.</exception>
    public EnclaveClient()
    {
        var options = GetSettingsFile();

        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        _httpClient = SetupHttpClient(options);
    }

    /// <summary>
    /// Simple <see cref="EnclaveClient"/> Setup using just a PersonalAccessToken.
    /// </summary>
    /// <param name="personalAccessToken">The token created on the Enclave Portal.</param>
    public EnclaveClient(string personalAccessToken)
    {
        var options = new EnclaveClientOptions { PersonalAccessToken = personalAccessToken };

        _httpClient = SetupHttpClient(options);
    }

    /// <summary>
    /// Setup all requirements for making API calls.
    /// </summary>
    /// <param name="options">Options for setting up the <see cref="EnclaveClient"/>.</param>
    /// <exception cref="ArgumentNullException">Throws if options are null.</exception>
    public EnclaveClient(EnclaveClientOptions options)
    {
        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        _httpClient = SetupHttpClient(options);
    }

    /// <summary>
    /// Gets a list of <see cref="AccountOrganisation"/> associated to the authorised user.
    /// </summary>
    /// <returns>List of organisation containing the OrgId and Name and the users role in that organisation.</returns>
    /// <exception cref="InvalidOperationException">throws when the Api returns a null response.</exception>
    public async Task<List<AccountOrganisation>> GetOrganisationsAsync()
    {
        var organisations = await _httpClient.GetFromJsonAsync<AccountOrganisationTopLevel>("/account/orgs", Constants.JsonSerializerOptions);

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

        var location = Path.Combine(userProfile, ".enclave", "credentials.json");

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
    }

    private static HttpClient SetupHttpClient(EnclaveClientOptions options)
    {
        var httpClient = new HttpClient(new ProblemDetailsHttpMessageHandler())
        {
            BaseAddress = new Uri(options.BaseUrl),
        };

        if (!string.IsNullOrWhiteSpace(options.PersonalAccessToken))
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", options.PersonalAccessToken);
        }

        var clientHeader = new ProductInfoHeaderValue("Enclave.Sdk.Api", Assembly.GetExecutingAssembly().GetName().Version?.ToString());
        httpClient.DefaultRequestHeaders.UserAgent.Add(clientHeader);
        return httpClient;
    }
}