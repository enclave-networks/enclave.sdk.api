using System.Net;
using System.Text.Json;
using Enclave.Api.Modules.AccountManagement.PublicAccount.Models;
using Enclave.Api.Modules.OrganisationManagement;
using Enclave.Api.Modules.OrganisationManagement.Organisation.Models;
using Enclave.Configuration.Data.Enums;
using Enclave.Configuration.Data.Identifiers;
using Enclave.Configuration.Data.Modules.Organisation.Models;
using Enclave.Sdk.Api.Clients;
using Enclave.Sdk.Api.Clients.Interfaces;
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

    // A client made from a full organisation model also answers OrgId, so code written against the
    // ID-only client works with either.
    [Test]
    public void Should_expose_the_id_of_the_organisation_it_was_created_from()
    {
        // Assert
        _organisationClient.OrgId.Should().Be(_organisationClient.Organisation.OrgId);
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
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(RemovedUserJson(AccountGuid.New())));
        // Act
        await _organisationClient.RemoveUserAsync(accountId);

        // Assert
        _server.Should().HaveReceivedACall().AtUrl($"{_server.Urls[0]}{_orgRoute}/users/{accountId}");
    }

    // The API answers a removal with the membership it removed (portal OrganisationController.RemoveUser and
    // RemoveOrganisationUserHandler.cs), so a caller can show who was removed. The expected user is built
    // separately from the response text, and every field is compared.
    [Test]
    public async Task Should_return_the_removed_user_the_api_sends_when_calling_RemoveUserAsync()
    {
        // Arrange
        var accountId = AccountGuid.New();

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/users/{accountId}").UsingDelete())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(RemovedUserJson(accountId)));

        // Act
        var result = await _organisationClient.RemoveUserAsync(accountId.ToString());

        // Assert
        result.Should().BeEquivalentTo(new OrganisationUser(
            accountId,
            "alex@example.com",
            "Alex Smith",
            new DateTime(2025, 3, 14, 9, 30, 0, 123, DateTimeKind.Utc),
            UserOrganisationRole.Admin));
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
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody("""{ "emailAddress": "testEmailAddress" }"""));

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
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody("""{ "emailAddress": "testEmailAddress" }"""));

        // Act
        await _organisationClient.CancelInviteAync("testEmailAddress");

        // Assert
        _server.Should().HaveReceivedACall().AtUrl($"{_server.Urls[0]}{_orgRoute}/invites");
    }

    // The API stores an invite with the address as first entered, finds it by the address in lower case, and
    // answers with the stored invite's address (portal OrganisationRepository.CreateUserInviteAsync and
    // GetEmailOrganisationInviteAsync; CreateOrganisationInviteHandler.cs). Inviting an address again in
    // another case returns the address as first entered, so the response here differs in case from the
    // address sent, and a result built from the argument rather than the body fails the check.
    [Test]
    public async Task Should_return_the_invite_the_api_sends_when_calling_InviteUserAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/invites").UsingPost())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody("""{ "emailAddress": "Sam@Example.com" }"""));

        // Act
        var result = await _organisationClient.InviteUserAsync("sam@example.com");

        // Assert
        result.Should().BeEquivalentTo(new OrganisationInviteModel { EmailAddress = "Sam@Example.com" });
    }

    // The API finds the invite to cancel by the address in lower case and answers with the cancelled invite's
    // stored address (portal OrganisationInviteDeletionHandler.cs and
    // OrganisationRepository.GetEmailOrganisationInviteAsync), so the response here differs in case from the
    // address sent, and a result built from the argument rather than the body fails the check.
    [Test]
    public async Task Should_return_the_cancelled_invite_the_api_sends_when_calling_CancelInviteAync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/invites").UsingDelete())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody("""{ "emailAddress": "Sam@Example.com" }"""));

        // Act
        var result = await _organisationClient.CancelInviteAync("sam@example.com");

        // Assert
        result.Should().BeEquivalentTo(new OrganisationInviteModel { EmailAddress = "Sam@Example.com" });
    }

    // An account ID is one segment of the URL path. .NET removes ".." segments when it combines a relative
    // path with the base address (RFC 3986 section 5.2.4), so "../invites" left unescaped would send the
    // DELETE to org/<id>/invites, the route that cancels invites. Escaped, the whole value stays one segment
    // under users, where it names no account. The request is checked as it left the client, where the
    // escaping is visible, and the server is checked for any request that reached the invites route.
    [Test]
    public async Task Should_send_an_account_id_as_one_escaped_path_segment_when_calling_RemoveUserAsync()
    {
        // Arrange
        using var server = WireMockServer.Start();
        server
          .Given(Request.Create().WithPath("/*").UsingAnyMethod())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(RemovedUserJson(AccountGuid.New())));

        var organisationId = OrganisationGuid.New();
        var recorder = new RecordingHttpMessageHandler(new HttpClientHandler());
        var client = new OrganisationScopedClient(new HttpClient(recorder) { BaseAddress = new Uri(server.Urls[0]) }, organisationId);

        // Act
        await client.RemoveUserAsync("../invites");

        // Assert
        var request = recorder.Requests.Should().ContainSingle().Subject;
        request.Method.Should().Be(HttpMethod.Delete);
        request.Uri.AbsolutePath.Should().Be($"/org/{organisationId}/users/..%2Finvites");
        server.LogEntries.Should().ContainSingle()
            .Which.RequestMessage.Path.Should().NotBe($"/org/{organisationId}/invites");
    }

    // Escaping leaves "." and ".." as they are, and .NET resolves them as dot-segments, so ".." would send
    // the DELETE to org/<id>. An empty ID drops the segment. None of these names an account, so each is
    // refused before a request is sent.
    [TestCase(null)]
    [TestCase("")]
    [TestCase(".")]
    [TestCase("..")]
    public async Task Should_refuse_an_account_id_that_is_not_a_path_segment_without_sending_a_request(string accountId)
    {
        // Arrange
        var recorder = new RecordingHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));
        var client = new OrganisationScopedClient(new HttpClient(recorder) { BaseAddress = new Uri("http://localhost/") }, OrganisationGuid.New());

        // Act
        var act = () => client.RemoveUserAsync(accountId);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>().WithParameterName("accountId");
        recorder.Requests.Should().BeEmpty();
    }

    private static IEnumerable<TestCaseData> SuccessesWithNoModel()
    {
        var calls = new (string Name, string Route, Func<IOrganisationScopedClient, Task> Call)[]
        {
            ("RemoveUserAsync", "users/account", client => client.RemoveUserAsync("account")),
            ("InviteUserAsync", "invites", client => client.InviteUserAsync("sam@example.com")),
            ("CancelInviteAync", "invites", client => client.CancelInviteAync("sam@example.com")),
        };

        var responses = new (string Name, HttpStatusCode Status, string Body)[]
        {
            ("200 with no body", HttpStatusCode.OK, null),
            ("204 No Content", HttpStatusCode.NoContent, null),
            ("200 with JSON null", HttpStatusCode.OK, "null"),
        };

        return from call in calls
               from response in responses
               select new TestCaseData(call.Call, call.Route, response.Status, response.Body).SetArgDisplayNames(call.Name, response.Name);
    }

    // These routes answer success with a model (portal OrganisationController.cs: RemoveUser, CreateInvite and
    // DeleteInvite), and a caller that shows the result needs it. A success with no body, such as a 204 or an
    // empty 200 from something between the client and the API, has no model to return, and nor does a body of
    // JSON null. Each throws InvalidOperationException naming the route and the status, so a caller learns
    // which call got no model; it does not return null, and it is not the JsonException that System.Text.Json
    // gives for input with no JSON tokens, which names neither.
    [TestCaseSource(nameof(SuccessesWithNoModel))]
    public async Task Should_throw_an_invalid_operation_exception_naming_the_route_for_a_success_with_no_model(
        Func<IOrganisationScopedClient, Task> call, string route, HttpStatusCode status, string body)
    {
        // Arrange
        var response = Response.Create().WithStatusCode(status);

        if (body is not null)
        {
            response = response
                .WithHeader("Content-Type", "application/json")
                .WithBody(body);
        }

        _server.Given(Request.Create().WithPath("/*").UsingAnyMethod()).RespondWith(response);

        // Act
        var act = () => call(_organisationClient);

        // Assert
        (await act.Should().ThrowAsync<InvalidOperationException>())
            .WithMessage($"*org/{_organisationClient.OrgId}/{route}*")
            .WithMessage($"*{(int)status}*");
        _server.LogEntries.Should().ContainSingle();
    }

    // The membership the API returns for a removed user, written out as the API writes it: camelCase names,
    // enums by name, typed IDs as 32 hex digits, and dates in ISO 8601 to the millisecond (portal
    // Enclave.Api.Scaffolding/CommonWebStartup.cs:91-99, AddJsonOptions, and
    // FractionalMillisecondTrimmingDateTimeConverter.cs). It is not serialised from the Enclave.Sdk.Api.Data
    // model, so a model whose names or types differ from the API's fails to read. Every field has a value
    // other than its default, so a field the model does not read fails the check. The role is Admin because
    // the API refuses to remove an owner (portal RemoveOrganisationUserHandler.cs).
    private static string RemovedUserJson(AccountGuid id) => $$"""
        {
          "id": "{{id}}",
          "emailAddress": "alex@example.com",
          "fullName": "Alex Smith",
          "joinDate": "2025-03-14T09:30:00.123Z",
          "role": "Admin"
        }
        """;
}
