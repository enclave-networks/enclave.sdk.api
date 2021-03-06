using System.Net.Http.Json;
using Enclave.Sdk.Api.Clients.Interfaces;
using Enclave.Sdk.Api.Data;
using Enclave.Sdk.Api.Data.Account;
using Enclave.Sdk.Api.Data.Organisations;
using Enclave.Sdk.Api.Data.PatchModel;

namespace Enclave.Sdk.Api.Clients;

/// <inheritdoc cref="IOrganisationClient" />
internal class OrganisationClient : ClientBase, IOrganisationClient
{
    private readonly string _orgRoute;

    /// <summary>
    /// This constructor is called by <see cref="EnclaveClient"/> when setting up the <see cref="OrganisationClient"/>.
    /// It also calls the <see cref="ClientBase"/> constructor.
    /// </summary>
    /// <param name="httpClient">an instance of httpClient with a baseURL referencing the API.</param>
    /// <param name="currentOrganisation">The current organisaiton used for routing the API calls.</param>
    public OrganisationClient(HttpClient httpClient, AccountOrganisation currentOrganisation)
        : base(httpClient)
    {
        Organisation = currentOrganisation;
        _orgRoute = $"org/{Organisation.OrgId}";

        Dns = new DnsClient(httpClient, _orgRoute);
        EnrolmentKeys = new EnrolmentKeysClient(httpClient, _orgRoute);
        Logs = new LogsClient(httpClient, _orgRoute);
        Policies = new PoliciesClient(httpClient, _orgRoute);
        EnrolledSystems = new EnrolledSystemsClient(httpClient, _orgRoute);
        Tags = new TagsClient(httpClient, _orgRoute);
        UnapprovedSystems = new UnapprovedSystemsClient(httpClient, _orgRoute);
    }

    /// <inheritdoc/>
    public AccountOrganisation Organisation { get; }

    /// <inheritdoc/>
    public IDnsClient Dns { get; }

    /// <inheritdoc/>
    public IEnrolmentKeysClient EnrolmentKeys { get; }

    /// <inheritdoc/>
    public ILogsClient Logs { get; }

    /// <inheritdoc/>
    public IPoliciesClient Policies { get; }

    /// <inheritdoc/>
    public IEnrolledSystemsClient EnrolledSystems { get; }

    /// <inheritdoc/>
    public ITagsClient Tags { get; }

    /// <inheritdoc/>
    public IUnapprovedSystemsClient UnapprovedSystems { get; }

    /// <inheritdoc/>
    public async Task<Organisation?> GetAsync()
    {
        var model = await HttpClient.GetFromJsonAsync<Organisation>(_orgRoute, Constants.JsonSerializerOptions);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public IPatchClient<OrganisationPatch, Organisation> Update()
    {
        return new PatchClient<OrganisationPatch, Organisation>(HttpClient, _orgRoute);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<OrganisationUser>> GetOrganisationUsersAsync()
    {
        var model = await HttpClient.GetFromJsonAsync<OrganisationUsersTopLevel>($"{_orgRoute}/users", Constants.JsonSerializerOptions);

        EnsureNotNull(model);

        return model.Users ?? Array.Empty<OrganisationUser>();
    }

    /// <inheritdoc/>
    public async Task RemoveUserAsync(string accountId)
    {
        await HttpClient.DeleteAsync($"{_orgRoute}/users/{accountId}");
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<OrganisationInvite>> GetPendingInvitesAsync()
    {
        var model = await HttpClient.GetFromJsonAsync<OrganisationPendingInvites>($"{_orgRoute}/invites", Constants.JsonSerializerOptions);

        EnsureNotNull(model);

        return model.Invites ?? Array.Empty<OrganisationInvite>();
    }

    /// <inheritdoc/>
    public async Task InviteUserAsync(string emailAddress)
    {
        using var encoded = CreateJsonContent(new OrganisationInvite
        {
            EmailAddress = emailAddress,
        });

        var result = await HttpClient.PostAsync($"{_orgRoute}/invites", encoded);
    }

    /// <inheritdoc/>
    public async Task CancelInviteAync(string emailAddress)
    {
        using var encoded = CreateJsonContent(new OrganisationInvite
        {
            EmailAddress = emailAddress,
        });

        using var request = new HttpRequestMessage
        {
            Content = encoded,
            Method = HttpMethod.Delete,
            RequestUri = new Uri($"{HttpClient.BaseAddress}{_orgRoute}/invites"),
        };

        await HttpClient.SendAsync(request);
    }
}