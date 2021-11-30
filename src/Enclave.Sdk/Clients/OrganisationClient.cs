using System.Net.Http.Json;
using Enclave.Sdk.Api.Clients.Interfaces;
using Enclave.Sdk.Api.Data;
using Enclave.Sdk.Api.Data.Account;
using Enclave.Sdk.Api.Data.Organisations;
using Enclave.Sdk.Api.Data.PatchModel;

namespace Enclave.Sdk.Api.Clients;

/// <inheritdoc cref="IOrganisationClient" />
public class OrganisationClient : ClientBase, IOrganisationClient
{
    private string _orgRoute;

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

        EnrolmentKeys = new EnrolmentKeysClient(httpClient, _orgRoute);
        Dns = new DnsClient(httpClient, _orgRoute);
    }

    /// <inheritdoc/>
    public AccountOrganisation Organisation { get; }

    /// <inheritdoc/>
    public IAuthorityClient Authority => throw new NotImplementedException();

    /// <inheritdoc/>
    public IDnsClient Dns { get; }

    /// <inheritdoc/>
    public IEnrolmentKeysClient EnrolmentKeys { get; }

    /// <inheritdoc/>
    public ILogsClient Logs => throw new NotImplementedException();

    /// <inheritdoc/>
    public IPoliciesClient Policies => throw new NotImplementedException();

    /// <inheritdoc/>
    public ISystemsClient Systems => throw new NotImplementedException();

    /// <inheritdoc/>
    public ITagsClient Tags => throw new NotImplementedException();

    /// <inheritdoc/>
    public IUnapprovedSystemsClient UnapprovedSystems => throw new NotImplementedException();

    /// <inheritdoc/>
    public async Task<Organisation?> GetAsync()
    {
        var model = await HttpClient.GetFromJsonAsync<Organisation>(_orgRoute);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public async Task<Organisation> UpdateAsync(PatchBuilder<OrganisationPatch> builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        using var encoded = CreateJsonContent(builder.Send());
        var result = await HttpClient.PatchAsync(_orgRoute, encoded);

        result.EnsureSuccessStatusCode();

        var model = await DeserialiseAsync<Organisation>(result.Content);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<OrganisationUser>> GetOrganisationUsersAsync()
    {
        var model = await HttpClient.GetFromJsonAsync<OrganisationUsersTopLevel>($"{_orgRoute}/users");

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
        var model = await HttpClient.GetFromJsonAsync<OrganisationPendingInvites>($"{_orgRoute}/invites");

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