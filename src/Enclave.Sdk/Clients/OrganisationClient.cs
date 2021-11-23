using Enclave.Sdk.Api.Clients.Interfaces;
using Enclave.Sdk.Api.Data.Account;
using Enclave.Sdk.Api.Data.Organisations;

namespace Enclave.Sdk.Api.Clients;

/// <inheritdoc cref="IOrganisationClient" />
public class OrganisationClient : ClientBase, IOrganisationClient
{
    /// <inheritdoc/>
    public AccountOrganisation CurrentOrganisation { get; private set; }

    /// <inheritdoc/>
    public IAuthorityClient Authority => throw new NotImplementedException();

    /// <inheritdoc/>
    public IDnsClient Dns => throw new NotImplementedException();

    /// <inheritdoc/>
    public IEnrolmentKeysClient EnrolmentKeys => throw new NotImplementedException();

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

    private string _orgRoute;

    /// <summary>
    /// This constructor is called by EnclaveClient when setting up the OrganisationClient.
    /// It also calls the ClientBase constructor.
    /// </summary>
    /// <param name="httpClient">an instance of httpClient with a baseURL referencing the API.</param>
    /// <param name="currentOrganisation">The current organisaiton used for routing the API calls.</param>
    public OrganisationClient(HttpClient httpClient, AccountOrganisation currentOrganisation)
        : base(httpClient)
    {
        CurrentOrganisation = currentOrganisation;
        _orgRoute = $"org/{CurrentOrganisation.OrgId}";
    }

    /// <inheritdoc/>
    public async Task<Organisation?> GetAsync()
    {
        var result = await HttpClient.GetAsync(_orgRoute);
        await CheckStatusCodes(result);

        var model = await DeserialiseAsync<Organisation>(result.Content);

        CheckModel(model);

        return model;
    }

    /// <inheritdoc/>
    public async Task<Organisation> UpdateAsync(Dictionary<string, object> updatedModel)
    {
        using var encoded = Encode(updatedModel);
        var result = await HttpClient.PatchAsync(_orgRoute, encoded);
        await CheckStatusCodes(result);

        var model = await DeserialiseAsync<Organisation>(result.Content);

        CheckModel(model);

        return model;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<OrganisationUser>?> GetOrganisationUsersAsync()
    {
        var result = await HttpClient.GetAsync($"{_orgRoute}/users");
        await CheckStatusCodes(result);

        var model = await DeserialiseAsync<OrganisationUsersTopLevel>(result.Content);

        CheckModel(model);

        return model.Users;
    }

    /// <inheritdoc/>
    public async Task RemoveUserAsync(string accountId)
    {
        var result = await HttpClient.DeleteAsync($"{_orgRoute}/users/{accountId}");
        await CheckStatusCodes(result);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<OrganisationInvite>> GetPendingInvitesAsync()
    {
        var result = await HttpClient.GetAsync($"{_orgRoute}/invites");
        await CheckStatusCodes(result);

        var model = await DeserialiseAsync<OrganisationPendingInvites>(result.Content);

        CheckModel(model);

        return model.Invites;
    }

    /// <inheritdoc/>
    public async Task InviteUserAsync(string emailAddress)
    {
        using var encoded = Encode(new OrganisationInvite
        {
            EmailAddress = emailAddress,
        });

        var result = await HttpClient.PostAsync($"{_orgRoute}/invites", encoded);
        await CheckStatusCodes(result);
    }

    /// <inheritdoc/>
    public async Task CancelInviteAync(string emailAddress)
    {
        using var encoded = Encode(new OrganisationInvite
        {
            EmailAddress = emailAddress,
        });

        using var request = new HttpRequestMessage
        {
            Content = encoded,
            Method = HttpMethod.Delete,
            RequestUri = new Uri($"{HttpClient.BaseAddress}{_orgRoute}/invites"),
        };

        var result = await HttpClient.SendAsync(request);
        await CheckStatusCodes(result);
    }

    /// <inheritdoc/>
    public async Task<OrganisationPricing> GetOrganisationPricing()
    {
        var result = await HttpClient.GetAsync($"{_orgRoute}/pricing");
        await CheckStatusCodes(result);

        var model = await DeserialiseAsync<OrganisationPricing>(result.Content);

        CheckModel(model);

        return model;
    }
}