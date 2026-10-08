using System.Net.Http.Json;
using System.Text.Json;
using Enclave.Api.Modules.OrganisationManagement;
using Enclave.Api.Modules.OrganisationManagement.Organisation.Models;
using Enclave.Configuration.Data.Identifiers;
using Enclave.Sdk.Api.Clients.Interfaces;
using Enclave.Sdk.Api.Data;

namespace Enclave.Sdk.Api.Clients;

/// <inheritdoc cref="IOrganisationScopedClient" />
internal class OrganisationScopedClient : ClientBase, IOrganisationScopedClient
{
    private readonly string _orgRoute;

    /// <summary>
    /// This constructor is called by <see cref="EnclaveClient"/> when setting up a client from an organisation ID,
    /// and by <see cref="OrganisationClient"/>. It also calls the <see cref="ClientBase"/> constructor.
    /// </summary>
    /// <param name="httpClient">an instance of httpClient with a baseURL referencing the API.</param>
    /// <param name="orgId">The ID of the organisation used for routing the API calls.</param>
    public OrganisationScopedClient(HttpClient httpClient, OrganisationGuid orgId)
        : base(httpClient)
    {
        OrgId = orgId;
        _orgRoute = $"org/{orgId}";

        Dns = new DnsClient(httpClient, _orgRoute);
        EnrolmentKeys = new EnrolmentKeysClient(httpClient, _orgRoute);
        Logs = new LogsClient(httpClient, _orgRoute);
        Policies = new PoliciesClient(httpClient, _orgRoute);
        EnrolledSystems = new SystemsClient(httpClient, _orgRoute);
        Tags = new TagsClient(httpClient, _orgRoute);
        UnapprovedSystems = new UnapprovedSystemsClient(httpClient, _orgRoute);
        TrustRequirements = new TrustRequirementsClient(httpClient, _orgRoute);
    }

    /// <inheritdoc/>
    public OrganisationGuid OrgId { get; }

    /// <inheritdoc/>
    public IDnsClient Dns { get; }

    /// <inheritdoc/>
    public IEnrolmentKeysClient EnrolmentKeys { get; }

    /// <inheritdoc/>
    public ILogsClient Logs { get; }

    /// <inheritdoc/>
    public IPoliciesClient Policies { get; }

    /// <inheritdoc/>
    public ISystemsClient EnrolledSystems { get; }

    /// <inheritdoc/>
    public ITagsClient Tags { get; }

    /// <inheritdoc/>
    public IUnapprovedSystemsClient UnapprovedSystems { get; }

    /// <inheritdoc/>
    public ITrustRequirementsClient TrustRequirements { get; }

    /// <inheritdoc/>
    public async Task<OrganisationPropertiesModel?> GetAsync()
    {
        var model = await HttpClient.GetFromJsonAsync<OrganisationPropertiesModel>(_orgRoute, Constants.JsonSerializerOptions);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public IPatchClient<OrganisationPatchModel, OrganisationPropertiesModel> Update()
    {
        return new PatchClient<OrganisationPatchModel, OrganisationPropertiesModel>(HttpClient, _orgRoute);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<OrganisationUser>> GetOrganisationUsersAsync()
    {
        var model = await HttpClient.GetFromJsonAsync<OrganisationUsersModel>($"{_orgRoute}/users", Constants.JsonSerializerOptions);

        EnsureNotNull(model);

        return model.Users ?? Array.Empty<OrganisationUser>();
    }

    /// <inheritdoc/>
    public async Task<OrganisationUser> RemoveUserAsync(string accountId)
    {
        var route = $"{_orgRoute}/users/{PathSegment(accountId)}";

        using var response = await HttpClient.DeleteAsync(route);

        return await ReadModelAsync<OrganisationUser>(response, HttpMethod.Delete, route);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<OrganisationInviteModel>> GetPendingInvitesAsync()
    {
        var model = await HttpClient.GetFromJsonAsync<OrganisationPendingInvitesModel>($"{_orgRoute}/invites", Constants.JsonSerializerOptions);

        EnsureNotNull(model);

        return model.Invites ?? Array.Empty<OrganisationInviteModel>();
    }

    /// <inheritdoc/>
    public async Task<OrganisationInviteModel> InviteUserAsync(string emailAddress)
    {
        var route = $"{_orgRoute}/invites";

        using var encoded = CreateJsonContent(new OrganisationInviteModel
        {
            EmailAddress = emailAddress,
        });

        using var response = await HttpClient.PostAsync(route, encoded);

        return await ReadModelAsync<OrganisationInviteModel>(response, HttpMethod.Post, route);
    }

    /// <inheritdoc/>
    public async Task<OrganisationInviteModel> CancelInviteAync(string emailAddress)
    {
        var route = $"{_orgRoute}/invites";

        using var encoded = CreateJsonContent(new OrganisationInviteModel
        {
            EmailAddress = emailAddress,
        });

        using var request = new HttpRequestMessage
        {
            Content = encoded,
            Method = HttpMethod.Delete,
            RequestUri = new Uri($"{HttpClient.BaseAddress}{route}"),
        };

        using var response = await HttpClient.SendAsync(request);

        return await ReadModelAsync<OrganisationInviteModel>(response, request.Method, route);
    }

    // The API answers RemoveUser, CreateInvite and DeleteInvite with a model (portal OrganisationController.cs), so
    // these calls return it. The status is checked before the body is read, so a failure throws HttpRequestException
    // with its status whatever body it has.
    //
    // A success with no body has no model to return. ASP.NET Core sends a null result as 204 No Content
    // (https://learn.microsoft.com/aspnet/core/web-api/advanced/formatting#special-case-formatters), and something
    // between the client and the API can answer 200 with no body. System.Text.Json throws JsonException for input
    // with no JSON tokens, which names neither the call nor the status, so the body is read as text and an empty one
    // throws InvalidOperationException here, as does a body of JSON null. The bodies are single small models, so
    // reading them as text first costs little.
    private static async Task<TModel> ReadModelAsync<TModel>(HttpResponseMessage response, HttpMethod method, string route)
        where TModel : class
    {
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync();

        var model = string.IsNullOrWhiteSpace(body)
            ? null
            : JsonSerializer.Deserialize<TModel>(body, Constants.JsonSerializerOptions);

        if (model is null)
        {
            throw new InvalidOperationException(
                $"The API answered {method} {route} with {(int)response.StatusCode} ({response.StatusCode}) and no {typeof(TModel).Name} in the body.");
        }

        return model;
    }
}
