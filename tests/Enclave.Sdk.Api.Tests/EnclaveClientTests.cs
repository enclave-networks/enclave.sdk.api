using System.Text.Json;
using Enclave.Api.Modules.AccountManagement.PublicAccount.Models;
using Enclave.Api.Modules.ActivityLogs.Logs.Models;
using Enclave.Api.Scaffolding.Pagination.Models;
using Enclave.Configuration.Data.Enums;
using Enclave.Configuration.Data.Identifiers;
using FluentAssertions;
using NUnit.Framework;
using WireMock.FluentAssertions;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Enclave.Sdk.Api.Tests;

public class EnclaveClientTests
{
    private EnclaveClient _client;
    private WireMockServer _server;

    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    [SetUp]
    public void Setup()
    {
        _server = WireMockServer.Start();

        var enclaveSettings = new EnclaveClientOptions
        {
            BaseUrl = _server.Urls[0],
        };

        _client = new EnclaveClient(enclaveSettings);
    }

    [Test]
    public async Task Should_return_list_of_orgs_when_calling_GetOrganisationsAsync()
    {
        // Arrange
        var accountOrg = new QueryAccountOrgsResponseModel(new List<AccountOrganisationModel>
        {
            new AccountOrganisationModel(OrganisationGuid.New(), "org1", UserOrganisationRole.Owner, false)
        });

        _server
          .Given(Request.Create().WithPath("/account/orgs").UsingGet())
          .RespondWith(
            Response.Create()
              .WithStatusCode(200)
              .WithBody(JsonSerializer.Serialize(accountOrg, _serializerOptions)));

        // Act
        var result = await _client.GetOrganisationsAsync();

        // Assert
        result.FirstOrDefault().OrgId.Should().Be(accountOrg.Orgs.FirstOrDefault().OrgId);
    }

    [Test]
    public async Task Should_throw_invalid_operation_exception_if_response_does_not_contain_an_organisation_model()
    {
        // Arrange
        _server
           .Given(Request.Create().WithPath("/account/orgs").UsingGet())
           .RespondWith(
             Response.Create()
               .WithStatusCode(200)
               .WithBody("null"));

        // Assert
        await _client.Invoking(c => c.GetOrganisationsAsync()).Should().ThrowAsync<InvalidOperationException>();
    }

    // A caller that has saved an organisation ID can build a client without first fetching the
    // organisation list, so the client's own calls go to that organisation with no /account/orgs call.
    [Test]
    public async Task Should_send_organisation_calls_to_the_given_id_when_created_from_an_organisation_id()
    {
        // Arrange
        var orgId = OrganisationGuid.New();

        _server
          .Given(Request.Create().WithPath($"/org/{orgId}/users").UsingGet())
          .RespondWith(
            Response.Create()
              .WithStatusCode(200)
              .WithBody("{\"users\":[]}"));

        // Act
        var organisationClient = _client.CreateOrganisationClient(orgId);
        var result = await organisationClient.GetOrganisationUsersAsync();

        // Assert
        organisationClient.OrgId.Should().Be(orgId);
        result.Should().BeEmpty();
        _server.Should().HaveReceivedACall().AtUrl($"{_server.Urls[0]}/org/{orgId}/users");
        _server.Should().HaveReceived(0).Calls().AtUrl($"{_server.Urls[0]}/account/orgs");
    }

    // The sub-clients take their route from the organisation client, so a client made from an ID
    // sends their calls to the same organisation.
    [Test]
    public async Task Should_send_sub_client_calls_to_the_given_id_when_created_from_an_organisation_id()
    {
        // Arrange
        var orgId = OrganisationGuid.New();
        var page = new PaginatedResponseModel<LogEntryModel>(
            new(0, 0, 0, 0, 0),
            new(new Uri("http://enclave.io"), new Uri("http://enclave.io"), new Uri("http://enclave.io"), new Uri("http://enclave.io")),
            new List<LogEntryModel>().ToAsyncEnumerable());

        _server
          .Given(Request.Create().WithPath($"/org/{orgId}/logs").UsingGet())
          .RespondWith(
            Response.Create()
              .WithStatusCode(200)
              .WithBody(await page.ToJsonAsync(_serializerOptions)));

        // Act
        var result = await _client.CreateOrganisationClient(orgId).Logs.GetLogsAsync();

        // Assert
        result.Should().NotBeNull();
        _server.LogEntries.Select(e => e.RequestMessage.Path).Should().Equal($"/org/{orgId}/logs");
    }
}