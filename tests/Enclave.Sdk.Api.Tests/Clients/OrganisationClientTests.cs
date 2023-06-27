using System.Text.Json;
using Enclave.Api.Modules.AccountManagement.PublicAccount.Models;
using Enclave.Api.Modules.OrganisationManagement;
using Enclave.Api.Modules.OrganisationManagement.Organisation.Models;
using Enclave.Configuration.Data.Enums;
using Enclave.Configuration.Data.Identifiers;
using Enclave.Configuration.Data.Modules.Organisation.Models;
using Enclave.Sdk.Api.Clients;
using FluentAssertions;
using NUnit.Framework;
using WireMock.FluentAssertions;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Enclave.Sdk.Api.Tests.Clients;

public class OrganisationClientTests
{
    private OrganisationClient _organisationClient;
    private WireMockServer _server;
    private string _orgRoute;
    private readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };


    private OrganisationPropertiesModel _organisationResponse;
    private OrganisationUsersModel _organisationUsersResponse;

    [SetUp]
    public void Setup()
    {
        _server = WireMockServer.Start();

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri(_server.Urls[0]),
        };

        var currentOrganisation = new AccountOrganisationModel(OrganisationGuid.New(), "TestName", UserOrganisationRole.Owner, false);

        _orgRoute = $"/org/{currentOrganisation.OrgId}";

        _organisationClient = new OrganisationClient(httpClient, currentOrganisation);

        _organisationResponse = new(OrganisationGuid.New(),
            DateTime.Now,
            "Org1",
            OrganisationPlan.External,
            string.Empty,
            string.Empty,
            int.MaxValue,
            int.MaxValue,
            0,
            Array.Empty<OrganisationFeature>(),
            null,
            null,
            null,
            null,
            TrialState.None,
            false,
            null);

        _organisationUsersResponse = new(new List<OrganisationUser>
        {
            new OrganisationUser(AccountGuid.New(), "test1@gmail.com", "test1", DateTime.Now, UserOrganisationRole.Admin),
            new OrganisationUser(AccountGuid.New(), "test2@gmail.com", "test2", DateTime.Now, UserOrganisationRole.Admin),
        });
    }

    [Test]
    public async Task Should_return_a_detailed_organisation_model_when_calling_GetAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath(_orgRoute).UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(_organisationResponse, _serializerOptions)));

        // Act
        var result = await _organisationClient.GetAsync();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(_organisationResponse.Id);
    }

    [Test]
    public async Task Should_return_a_detailed_organisation_model_when_updating_with_UpdateAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath(_orgRoute).UsingPatch())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(_organisationResponse, _serializerOptions)));

        // Act
        var result = await _organisationClient.Update().Set(x => x.Website, "newWebsite").ApplyAsync();

        // Assert
        result.Should().NotBeNull();
        result.Website.Should().Be(_organisationResponse.Website);
    }

    [Test]
    public async Task Should_make_a_call_to_api_when_updating_with_UpdateAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath(_orgRoute).UsingPatch())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(_organisationResponse, _serializerOptions)));

        // Act
        var result = await _organisationClient.Update().Set(x => x.Website, "newWebsite").ApplyAsync();

        // Assert
        _server.Should().HaveReceivedACall().AtUrl($"{_server.Urls[0]}{_orgRoute}");
    }

    [Test]
    public async Task Should_return_a_list_of_organisation_users_when_calling_GetOrganisationUsersAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/users").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(_organisationUsersResponse, _serializerOptions)));

        // Act
        var result = await _organisationClient.GetOrganisationUsersAsync();

        // Assert
        result.Count.Should().Be(2);
    }

    [Test]
    public async Task Should_make_a_call_to_api_when_calling_GetOrganisationUsersAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/users").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(_organisationUsersResponse, _serializerOptions)));

        // Act
        var result = await _organisationClient.GetOrganisationUsersAsync();

        // Assert
        _server.Should().HaveReceivedACall().AtUrl($"{_server.Urls[0]}{_orgRoute}/users");
    }

    [Test]
    public async Task Should_make_a_call_to_api_when_calling_RemoveUserAsync()
    {
        // Arrange
        var accountId = "test";

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/users/{accountId}").UsingDelete())
          .RespondWith(
            Response.Create()
              .WithSuccess());
        // Act
        await _organisationClient.RemoveUserAsync(accountId);

        // Assert
        _server.Should().HaveReceivedACall().AtUrl($"{_server.Urls[0]}{_orgRoute}/users/{accountId}");
    }

    [Test]
    public async Task Should_return_list_of_pending_invites_when_calling_GetPendingInvitesAsync()
    {
        // Arrange
        var invites = new OrganisationPendingInvitesModel(new List<OrganisationInviteModel>
        {
            new OrganisationInviteModel
            {
                EmailAddress = "testEmail1",
            },
            new OrganisationInviteModel
            {
                EmailAddress = "testEmail2",
            },
        });

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/invites").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(invites, _serializerOptions)));

        // Act
        var result = await _organisationClient.GetPendingInvitesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(2);
    }

    [Test]
    public async Task Should_make_a_call_to_api_when_calling_GetPendingInvitesAsync()
    {
        // Arrange
        var invites = new OrganisationPendingInvitesModel(new List<OrganisationInviteModel>
        {
            new OrganisationInviteModel()
        });

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/invites").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(invites, _serializerOptions)));

        // Act
        var result = await _organisationClient.GetPendingInvitesAsync();

        // Assert
        _server.Should().HaveReceivedACall().AtUrl($"{_server.Urls[0]}{_orgRoute}/invites");
    }

    [Test]
    public async Task Should_make_a_call_to_api_when_calling_InviteUserAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/invites").UsingPost())
          .RespondWith(
            Response.Create()
              .WithSuccess());

        // Act
        await _organisationClient.InviteUserAsync("testEmailAddress");

        // Assert
        _server.Should().HaveReceivedACall().AtUrl($"{_server.Urls[0]}{_orgRoute}/invites");
    }

    [Test]
    public async Task Should_make_a_call_to_api_when_calling_CancelInviteAync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/invites").UsingDelete())
          .RespondWith(
            Response.Create()
              .WithSuccess());

        // Act
        await _organisationClient.CancelInviteAync("testEmailAddress");

        // Assert
        _server.Should().HaveReceivedACall().AtUrl($"{_server.Urls[0]}{_orgRoute}/invites");
    }
}
