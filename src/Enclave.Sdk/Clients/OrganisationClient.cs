using Enclave.Sdk.Api.Clients.Interfaces;
using Enclave.Sdk.Api.Data.Account;
using Enclave.Sdk.Api.Data.Organisations;

namespace Enclave.Sdk.Api.Clients;

public class OrganisationClient : ClientBase, IOrganisationClient
{
    public AccountOrganisation CurrentOrganisation { get; private set; }

    public DnsClient DNSClient { get; private set; }

    private string _orgRoute;

    public OrganisationClient(HttpClient httpClient, AccountOrganisation currentOrganisation)
        : base(httpClient)
    {
        CurrentOrganisation = currentOrganisation;
        _orgRoute = $"org/{CurrentOrganisation.OrgId}";
    }

    public async Task<Organisation?> GetAsync()
    {
        var result = await HttpClient.GetAsync(_orgRoute);
        await CheckStatusCodes(result);

        var model = await DeserializeAsync<Organisation>(result.Content);

        CheckModel(model);

        return model;
    }

    public async Task<Organisation> UpdateAsync(Dictionary<string, object> updatedModel)
    {
        var encoded = Encode(updatedModel);
        var result = await HttpClient.PatchAsync(_orgRoute, encoded);
        await CheckStatusCodes(result);

        var model = await DeserializeAsync<Organisation>(result.Content);

        CheckModel(model);

        return model;
    }

    public async Task<IReadOnlyList<OrganisationUser>?> GetOrganisationUsersAsync()
    {
        var result = await HttpClient.GetAsync($"{_orgRoute}/users");
        await CheckStatusCodes(result);

        var model = await DeserializeAsync<OrganisationUsersTopLevel>(result.Content);

        CheckModel(model);

        return model.Users;
    }

    public async Task RemoveUserAsync(string accountId)
    {
        var result = await HttpClient.DeleteAsync($"{_orgRoute}/users/{accountId}");
        await CheckStatusCodes(result);
    }

    public async Task<OrganisationPendingInvites> GetPendingInvitesAsync()
    {
        var result = await HttpClient.GetAsync($"{_orgRoute}/invites");
        await CheckStatusCodes(result);

        var model = await DeserializeAsync<OrganisationPendingInvites>(result.Content);

        CheckModel(model);

        return model;
    }

    public async Task InviteUserAsync(string emailAddress)
    {
        var encoded = Encode(new OrganisationInvite
        {
            EmailAddress = emailAddress,
        });

        var result = await HttpClient.PostAsync($"{_orgRoute}/invites", encoded);
        await CheckStatusCodes(result);
    }

    public async Task CancelInviteAync(string emailAddress)
    {
        var encoded = Encode(new OrganisationInvite
        {
            EmailAddress = emailAddress,
        });

        var request = new HttpRequestMessage
        {
            Content = encoded,
            Method = HttpMethod.Delete,
            RequestUri = new Uri($"{HttpClient.BaseAddress}{_orgRoute}/invites"),
        };

        var result = await HttpClient.SendAsync(request);
        await CheckStatusCodes(result);
    }

    public async Task<OrganisationPricing> GetOrganisationPricing()
    {
        var result = await HttpClient.GetAsync($"{_orgRoute}/pricing");
        await CheckStatusCodes(result);

        var model = await DeserializeAsync<OrganisationPricing>(result.Content);

        CheckModel(model);

        return model;
    }

    protected override string PrepareUrl(string url)
    {
        var newUrl = $"{_orgRoute}{url}";
        return base.PrepareUrl(newUrl);
    }
}