using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json;
using Enclave.Api.Modules.AccountManagement.PublicAccount.Models;
using Enclave.Configuration.Data.Identifiers;
using Enclave.Sdk.Api.Clients;
using Enclave.Sdk.Api.Clients.Interfaces;
using Enclave.Sdk.Api.Handlers;

namespace Enclave.Sdk.Api;

/// <summary>
/// Our main entry point for all API work.
/// </summary>
public class EnclaveClient
{
    private static readonly JsonSerializerOptions CredentialsFileJsonOptions = new() { PropertyNameCaseInsensitive = true };

    private readonly HttpClient _httpClient;

    private readonly HttpClient? _partnerHttpClient;

    /// <summary>
    /// Create an <see cref="EnclaveClient"/> using settings found in the .enclave/credentials.json file in your user directory.
    /// </summary>
    /// <exception cref="FileNotFoundException">Throws if there is no credentials file.</exception>
    /// <exception cref="InvalidOperationException">Throws if the credentials file is not valid JSON or holds no credentials.</exception>
    public EnclaveClient()
    {
        var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var location = Path.Combine(userProfile, ".enclave", "credentials.json");
        var options = ReadCredentialsFile(location);

        _httpClient = SetupHttpClient(options.BaseUrl, options);
        _partnerHttpClient = SetupPartnerHttpClient(options);
    }

    /// <summary>
    /// Simple <see cref="EnclaveClient"/> Setup using just a PersonalAccessToken.
    /// </summary>
    /// <param name="personalAccessToken">The token created on the Enclave Portal.</param>
    public EnclaveClient(string personalAccessToken)
    {
        var options = new EnclaveClientOptions { PersonalAccessToken = personalAccessToken };

        _httpClient = SetupHttpClient(options.BaseUrl, options);
        _partnerHttpClient = SetupPartnerHttpClient(options);
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

        _httpClient = SetupHttpClient(options.BaseUrl, options);
        _partnerHttpClient = SetupPartnerHttpClient(options);
    }

    /// <summary>
    /// Gets a list of <see cref="AccountOrganisationModel"/> associated to the authorised user.
    /// </summary>
    /// <returns>List of organisation containing the OrgId and Name and the users role in that organisation.</returns>
    /// <exception cref="InvalidOperationException">throws when the Api returns a null response.</exception>
    public async Task<IReadOnlyList<AccountOrganisationModel>> GetOrganisationsAsync()
    {
        var organisations = await _httpClient.GetFromJsonAsync<QueryAccountOrgsResponseModel>("/account/orgs", Constants.JsonSerializerOptions);

        if (organisations is null)
        {
            throw new InvalidOperationException("Could not deserialize orgs associated to this token");
        }

        return organisations.Orgs;
    }

    /// <summary>
    /// Create an <see cref="OrganisationClient"/> from an <see cref="AccountOrganisationModel"/>.
    /// </summary>
    /// <param name="organisation">the <see cref="AccountOrganisationModel"/> from <see cref="GetOrganisationsAsync"/>.</param>
    /// <returns>OrganisationClient that can be used for all following Api Calls related to an organisation.</returns>
    /// <exception cref="ArgumentNullException">Throws if <paramref name="organisation"/> is null.</exception>
    public IOrganisationClient CreateOrganisationClient(AccountOrganisationModel organisation)
    {
        ArgumentNullException.ThrowIfNull(organisation);

        return new OrganisationClient(_httpClient, organisation);
    }

    /// <summary>
    /// Create a client for one organisation from its ID, without fetching the organisation list first.
    /// </summary>
    /// <param name="orgId">The ID of the organisation, for example one saved from an earlier <see cref="GetOrganisationsAsync"/> call.</param>
    /// <returns>A client that sends every organisation call to that organisation. It has no <see cref="IOrganisationClient.Organisation"/>, since an ID alone does not give the organisation's name or the user's role.</returns>
    /// <exception cref="ArgumentException">Throws if <paramref name="orgId"/> is empty.</exception>
    public IOrganisationScopedClient CreateOrganisationClient(OrganisationGuid orgId)
    {
        if (orgId.Equals(default(OrganisationGuid)))
        {
            throw new ArgumentException("The organisation ID is empty.", nameof(orgId));
        }

        return new OrganisationScopedClient(_httpClient, orgId);
    }

    /// <summary>
    /// Create a client for the Enclave Partner API, for one partner.
    /// </summary>
    /// <param name="partnerId">The ID of the partner. The partner portal shows it.</param>
    /// <returns>A client that sends every call for that partner to <see cref="EnclaveClientOptions.PartnerApiBaseUrl"/>.</returns>
    /// <exception cref="ArgumentException">Throws if <paramref name="partnerId"/> is empty.</exception>
    /// <exception cref="InvalidOperationException">Throws if <see cref="EnclaveClientOptions.PartnerApiBaseUrl"/> was set to null or blank.</exception>
    public IPartnerClient CreatePartnerClient(PartnerId partnerId)
    {
        if (partnerId.Equals(default(PartnerId)))
        {
            throw new ArgumentException("The partner ID is empty.", nameof(partnerId));
        }

        if (_partnerHttpClient is null)
        {
            throw new InvalidOperationException(
                "No partner API base URL is set. Set PartnerApiBaseUrl in the EnclaveClientOptions, or partnerApiBaseUrl in the credentials file, to the URL of the Enclave Partner API.");
        }

        return new PartnerClient(_partnerHttpClient, partnerId);
    }

    /// <summary>
    /// Create an <see cref="AuthorityClient"/>.
    /// </summary>
    /// <returns>An instance of AuthorityClient for use with enrol requests.</returns>
    public IAuthorityClient CreateAuthorityClient()
    {
        return new AuthorityClient(_httpClient);
    }

    /// <summary>
    /// Reads the options in a credentials file, with errors that name the file and say how to supply a token.
    /// </summary>
    /// <param name="location">The path of the credentials file.</param>
    /// <returns>The options the file holds.</returns>
    internal static EnclaveClientOptions ReadCredentialsFile(string location)
    {
        const string expectedContent = "{ \"personalAccessToken\": \"<your token>\" }";

        string json;

        try
        {
            json = File.ReadAllText(location);
        }
        catch (Exception ex) when (ex is FileNotFoundException or DirectoryNotFoundException)
        {
            throw new FileNotFoundException(
                $"No Enclave credentials file at {location}. Create it containing {expectedContent}, or pass a token to the EnclaveClient(string) or EnclaveClient(EnclaveClientOptions) constructor. Personal access tokens are created on the account page of the Enclave portal.",
                location,
                ex);
        }

        EnclaveClientOptions? options;

        try
        {
            options = JsonSerializer.Deserialize<EnclaveClientOptions>(json, CredentialsFileJsonOptions);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"The Enclave credentials file at {location} is not valid JSON. It should contain {expectedContent}.", ex);
        }

        return options ?? throw new InvalidOperationException($"The Enclave credentials file at {location} holds no credentials. It should contain {expectedContent}.");
    }

    private static HttpClient? SetupPartnerHttpClient(EnclaveClientOptions options)
    {
        // The partner API is a separate service on its own host (portal src/Enclave.Partner.Api), and
        // PartnerApiBaseUrl defaults to production's, https://partner-api.enclave.io. A caller that sets it to
        // null or blank has asked for no partner API. Falling back to BaseUrl would send partner calls to the
        // main API, which has no partner routes, so there is then no partner HttpClient, and
        // CreatePartnerClient says what to set.
        //
        // The partner HttpClient is built once, like the main one, from the same options: the same token,
        // User-Agent and handlers. A caller's handler becomes the inner handler of both problem details
        // handlers. That is safe because an HttpClient already sends concurrent requests through one handler,
        // and neither HttpClient is disposed, so neither disposes the caller's handler.
        if (string.IsNullOrWhiteSpace(options.PartnerApiBaseUrl))
        {
            return null;
        }

        return SetupHttpClient(options.PartnerApiBaseUrl, options);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Needed for lifecycle of consumer")]
    private static HttpClient SetupHttpClient(string baseUrl, EnclaveClientOptions options)
    {
        // A caller's handler goes beneath the problem details handler, so it takes the place of the network
        // and problem+json responses it returns still become EnclaveApiException. The HttpClient is never
        // disposed, so the caller's handler is never disposed here either; the caller owns it.
        var problemDetailsHandler = options.HttpMessageHandler is null
            ? new ProblemDetailsHttpMessageHandler()
            : new ProblemDetailsHttpMessageHandler(options.HttpMessageHandler);

        var httpClient = new HttpClient(problemDetailsHandler)
        {
            BaseAddress = new Uri(baseUrl),
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