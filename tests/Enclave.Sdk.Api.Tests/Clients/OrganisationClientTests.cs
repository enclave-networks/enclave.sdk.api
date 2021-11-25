using System.Text.Json;
using Enclave.Sdk.Api.Clients;
using Enclave.Sdk.Api.Data;
using Enclave.Sdk.Api.Data.Account;
using Enclave.Sdk.Api.Data.Organisations;
using Enclave.Sdk.Api.Data.PatchModel;
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
    private JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    [SetUp]
    public void Setup()
    {
        _server = WireMockServer.Start();

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri(_server.Urls[0]),
        };

        var currentOrganisation = new AccountOrganisation
        {
            OrgId = OrganisationId.New(),
            OrgName = "TestName",
            Role = UserOrganisationRole.Admin,
        };

        _orgRoute = $"/org/{currentOrganisation.OrgId}";

        _organisationClient = new OrganisationClient(httpClient, currentOrganisation);
    }

    [Test]
    public async Task Should_return_a_detailed_organisation_model_when_calling_GetAsync()
    {
        // Arrange
        var org = new Organisation
        {
            Id = OrganisationId.New(),
        };

        _server
          .Given(Request.Create().WithPath(_orgRoute).UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(org, _serializerOptions)));

        // Act
        var result = await _organisationClient.GetAsync();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(org.Id);
    }

    [Test]
    public async Task Should_return_a_detailed_organisation_model_when_updating_with_UpdateAsync()
    {
        // Arrange
        var org = new Organisation
        {
            Id = OrganisationId.New(),
            Website = "newWebsite",
        };

        _server
          .Given(Request.Create().WithPath(_orgRoute).UsingPatch())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(org, _serializerOptions)));

        var patchModel = new PatchBuilder<OrganisationPatch>().Set(x => x.Website, "newWebsite");

        // Act
        var result = await _organisationClient.UpdateAsync(patchModel);

        // Assert
        result.Should().NotBeNull();
        result.Website.Should().Be(org.Website);
    }

    [Test]
    public async Task Should_make_a_call_to_api_when_updating_with_UpdateAsync()
    {
        // Arrange
        var org = new Organisation
        {
            Id = OrganisationId.New(),
            Website = "newWebsite",
        };

        _server
          .Given(Request.Create().WithPath(_orgRoute).UsingPatch())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(org, _serializerOptions)));

        var patchModel = new PatchBuilder<OrganisationPatch>().Set(x => x.Website, "newWebsite");

        // Act
        var result = await _organisationClient.UpdateAsync(patchModel);

        // Assert
        _server.Should().HaveReceivedACall().AtUrl($"{_server.Urls[0]}{_orgRoute}");
    }

    [Test]
    public async Task Should_return_a_list_of_organisation_users_when_calling_GetOrganisationUsersAsync()
    {
        // Arrange
        var orgUsers = new OrganisationUsersTopLevel
        {
            Users = new List<OrganisationUser>()
            {
                new OrganisationUser
                {
                    Id = AccountId.New(),
                    FirstName = "testUser1",
                    LastName = "lastName1",
                },
                new OrganisationUser
                {
                    Id = AccountId.New(),
                    FirstName = "testUser2",
                    LastName = "lastName2",
                },
            },
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/users").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(orgUsers, _serializerOptions)));

        // Act
        var result = await _organisationClient.GetOrganisationUsersAsync();

        // Assert
        result.Count.Should().Be(2);
    }

    [Test]
    public async Task Should_make_a_call_to_api_when_calling_GetOrganisationUsersAsync()
    {
        // Arrange
        var orgUsers = new OrganisationUsersTopLevel
        {
            Users = new List<OrganisationUser>(),
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/users").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(orgUsers, _serializerOptions)));

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
        var invites = new OrganisationPendingInvites()
        {
            Invites = new List<OrganisationInvite>
            {
                new OrganisationInvite { EmailAddress = "testEmail1" },
                new OrganisationInvite { EmailAddress = "testEmail2" },
            },
        };

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
        var invites = new OrganisationPendingInvites()
        {
            Invites = new List<OrganisationInvite>(),
        };

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

    [Test]
    public async Task Should_return_organisation_pricing_when_calling_GetOrganisationPricing()
    {
        // Arrange
        var organisationPricing = new OrganisationPricing
        {
            LastBillingEvent = new OrganisationBillingEvent(),
            Starter = new OrganisationPlanPricing
            {
                CurrencySymbol = "£",
                Enabled = true,
                Quantities = new List<PlanPricingQuanitity>(),
            },
            Pro = new OrganisationPlanPricing
            {
                CurrencySymbol = "£",
                Enabled = true,
                Quantities = new List<PlanPricingQuanitity>(),
            },
            Business = new OrganisationPlanPricing
            {
                CurrencySymbol = "£",
                Enabled = true,
                Quantities = new List<PlanPricingQuanitity>(),
            },
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/pricing").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(organisationPricing, _serializerOptions)));

        // Act
        var result = await _organisationClient.GetOrganisationPricing();

        // Assert
        result.Should().NotBeNull();
        result.Starter.CurrencySymbol.Should().Be("£");
    }

    [Test]
    public async Task Should_make_a_call_to_api_when_calling_GetOrganisationPricing()
    {
        // Arrange
        var organisationPricing = new OrganisationPricing
        {
            LastBillingEvent = new OrganisationBillingEvent(),
            Starter = new OrganisationPlanPricing(),
            Pro = new OrganisationPlanPricing(),
            Business = new OrganisationPlanPricing(),
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/pricing").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(organisationPricing, _serializerOptions)));

        // Act
        await _organisationClient.GetOrganisationPricing();

        // Assert
        _server.Should().HaveReceivedACall().AtUrl($"{_server.Urls[0]}{_orgRoute}/pricing");
    }
}
