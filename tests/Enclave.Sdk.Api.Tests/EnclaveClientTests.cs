using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Enclave.Api.Modules.AccountManagement.PublicAccount.Models;
using Enclave.Api.Modules.ActivityLogs.Logs.Models;
using Enclave.Api.Scaffolding.Pagination.Models;
using Enclave.Configuration.Data.Enums;
using Enclave.Configuration.Data.Identifiers;
using Enclave.Sdk.Api.Exceptions;
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

    // A null organisation model names no organisation, so it is refused where the mistake is made,
    // with the parameter named, rather than failing inside the client.
    [Test]
    public void Should_throw_an_argument_null_exception_when_created_from_a_null_organisation()
    {
        // Act
        var act = () => _client.CreateOrganisationClient((AccountOrganisationModel)null);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("organisation");
    }

    // The default OrganisationGuid is no organisation, so a client made from it would send every call to
    // an organisation that does not exist; it is refused where the mistake is made.
    [Test]
    public void Should_throw_an_argument_exception_when_created_from_an_empty_organisation_id()
    {
        // Act
        var act = () => _client.CreateOrganisationClient(default(OrganisationGuid));

        // Assert
        act.Should().Throw<ArgumentException>().WithParameterName("orgId");
        _server.LogEntries.Should().BeEmpty();
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

    // A caller that wants to see or withhold the requests a client builds (a dry run, or request logging)
    // gives its own handler. The handler answers here, so nothing is sent: the base URL uses the reserved
    // .invalid domain (RFC 2606), which resolvers report as nonexistent (RFC 6761 section 6.4), so a request
    // that bypassed the handler would fail. The handler sees the request as it would be sent, Authorization
    // header included.
    [Test]
    public async Task Should_send_requests_through_the_given_handler_with_the_authorization_header()
    {
        // Arrange
        var orgs = new QueryAccountOrgsResponseModel(new List<AccountOrganisationModel>
        {
            new AccountOrganisationModel(OrganisationGuid.New(), "org1", UserOrganisationRole.Owner, false)
        });
        var body = JsonSerializer.Serialize(orgs, _serializerOptions);

        var handler = new RecordingHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(body, Encoding.UTF8, "application/json"),
        });

        var client = new EnclaveClient(new EnclaveClientOptions
        {
            BaseUrl = "https://api.enclave.invalid",
            PersonalAccessToken = "TOKEN",
            HttpMessageHandler = handler,
        });

        // Act
        var result = await client.GetOrganisationsAsync();

        // Assert
        result.Should().ContainSingle().Which.OrgId.Should().Be(orgs.Orgs[0].OrgId);
        var request = handler.Requests.Should().ContainSingle().Subject;
        request.Method.Should().Be(HttpMethod.Get);
        request.Uri.Should().Be(new Uri("https://api.enclave.invalid/account/orgs"));
        request.Authorization.Should().Be(new AuthenticationHeaderValue("Bearer", "TOKEN"));
    }

    // The given handler takes the place of the network, not of the client's own response handling, so a
    // problem+json response from it still throws EnclaveApiException with the API's problem details.
    [Test]
    public async Task Should_throw_an_enclave_api_exception_for_a_problem_response_from_the_given_handler()
    {
        // Arrange
        var handler = new RecordingHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.Forbidden)
        {
            Content = new StringContent("{\"title\":\"Forbidden\",\"status\":403}", Encoding.UTF8, "application/problem+json"),
        });

        var client = new EnclaveClient(new EnclaveClientOptions
        {
            BaseUrl = "https://api.enclave.invalid",
            PersonalAccessToken = "TOKEN",
            HttpMessageHandler = handler,
        });

        // Act
        var act = () => client.GetOrganisationsAsync();

        // Assert
        var exception = (await act.Should().ThrowAsync<EnclaveApiException>()).Which;
        exception.ProblemDetails.Title.Should().Be("Forbidden");
        handler.Requests.Should().ContainSingle();
    }

    // With no handler given, requests go to the network, with the token in the Authorization header.
    [Test]
    public async Task Should_send_requests_to_the_base_url_with_the_authorization_header_when_no_handler_is_given()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath("/account/orgs").UsingGet())
          .RespondWith(
            Response.Create()
              .WithStatusCode(200)
              .WithBody("{\"orgs\":[]}"));

        var client = new EnclaveClient(new EnclaveClientOptions
        {
            BaseUrl = _server.Urls[0],
            PersonalAccessToken = "TOKEN",
        });

        // Act
        await client.GetOrganisationsAsync();

        // Assert
        var request = _server.LogEntries.Should().ContainSingle().Subject.RequestMessage;
        request.Path.Should().Be("/account/orgs");
        request.Headers["Authorization"].Should().Equal("Bearer TOKEN");
    }
}