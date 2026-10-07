using Enclave.Sdk.Api.Clients;
using Enclave.Sdk.Api.Clients.Interfaces;
using Enclave.Sdk.Api.Data;
using Enclave.Configuration.Data.Enums;
using FluentAssertions;
using NUnit.Framework;
using System.Net;
using System.Text.Json;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.FluentAssertions;
using Enclave.Configuration.Data.Identifiers;
using Enclave.Api.Modules.SystemManagement.Systems.Models;
using Enclave.Api.Scaffolding.Pagination.Models;
using Enclave.Configuration.Data.Modules.Systems.Enums;
using Enclave.Api.Modules.SystemManagement.Dns.Models;
using Enclave.Api.Modules.SystemManagement.Tags.Models;

namespace Enclave.Sdk.Api.Tests.Clients;

public class SystemClientTests
{
    private SystemsClient _enrolledSystemsClient;
    private WireMockServer _server;
    private string _orgRoute;
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private PaginatedResponseModel<SystemSummaryModel> _paginatedResponse;
    private SystemModel _systemResponse;

    [SetUp]
    public void Setup()
    {
        _server = WireMockServer.Start();

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri(_server.Urls[0]),
        };

        var organisationId = OrganisationGuid.New();
        _orgRoute = $"/org/{organisationId}";

        _enrolledSystemsClient = new SystemsClient(httpClient, $"org/{organisationId}");

        _paginatedResponse = new(
                new(0, 0, 0, 0, 0),
                new(new Uri("http://enclave.io"), new Uri("http://enclave.io"), new Uri("http://enclave.io"), new Uri("http://enclave.io")),
                new List<SystemSummaryModel>
                {
                    new("1",
                        DateTimeOffset.Now,
                        null,
                        SystemType.GeneralPurpose,
                        SystemState.Connected,
                        DateTimeOffset.Now,
                        DateTimeOffset.Now,
                        EnrolmentKeyId.FromInt(1),
                        string.Empty,
                        true,
                        null,
                        Array.Empty<SystemGatewayRouteModel>(),
                        Array.Empty<IUsedTagModel>(),
                        null)
                }.ToAsyncEnumerable());

        _systemResponse = new SystemModel("1",
                        DateTimeOffset.Now,
                        "new description",
                        SystemType.GeneralPurpose,
                        SystemState.Connected,
                        DateTimeOffset.Now,
                        DateTimeOffset.Now,
                        EnrolmentKeyId.FromInt(1),
                        string.Empty,
                        false,
                        true,
                        null,
                        Array.Empty<SystemGatewayRouteModel>(),
                        Array.Empty<IUsedTagModel>(),
                        Array.Empty<SystemDnsEntry>(),
                        Array.Empty<string>(),
                        null,
                        null);
    }

    [Test]
    public async Task Should_return_a_paginated_response_model_when_calling_GetSystemsAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/systems").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _paginatedResponse.ToJsonAsync(_serializerOptions)));

        // Act
        var result = await _enrolledSystemsClient.GetSystemsAsync();

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public async Task Should_make_a_call_to_api_with_enrolment_key_quertString_when_calling_GetSystemsAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/systems").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _paginatedResponse.ToJsonAsync(_serializerOptions)));

        var enrolmentKeyId = 123;

        // Act
        await _enrolledSystemsClient.GetSystemsAsync(enrolmentKeyId: enrolmentKeyId);

        // Assert
        _server.Should().HaveReceivedACall().AtAbsoluteUrl($"{_server.Urls[0]}{_orgRoute}/systems?enrolment_key={enrolmentKeyId}");
    }

    [Test]
    public async Task Should_make_a_call_to_api_with_search_quertString_when_calling_GetSystemsAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/systems").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _paginatedResponse.ToJsonAsync(_serializerOptions)));

        var searchTerm = "term";

        // Act
        await _enrolledSystemsClient.GetSystemsAsync(searchTerm: searchTerm);

        // Assert
        _server.Should().HaveReceivedACall().AtAbsoluteUrl($"{_server.Urls[0]}{_orgRoute}/systems?search={searchTerm}");
    }

    [Test]
    public async Task Should_make_a_call_to_api_with_include_disabled_quertString_when_calling_GetSystemsAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/systems").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _paginatedResponse.ToJsonAsync(_serializerOptions)));

        var includeDisabled = true;

        // Act
        await _enrolledSystemsClient.GetSystemsAsync(includeDisabled: includeDisabled);

        // Assert
        _server.Should().HaveReceivedACall().AtAbsoluteUrl($"{_server.Urls[0]}{_orgRoute}/systems?include_disabled={includeDisabled}");
    }

    [Test]
    public async Task Should_make_a_call_to_api_with_sort_quertString_when_calling_GetSystemsAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/systems").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _paginatedResponse.ToJsonAsync(_serializerOptions)));

        var sortOrder = SystemQuerySortMode.RecentlyConnected;

        // Act
        await _enrolledSystemsClient.GetSystemsAsync(sortOrder: sortOrder);

        // Assert
        _server.Should().HaveReceivedACall().AtAbsoluteUrl($"{_server.Urls[0]}{_orgRoute}/systems?sort={sortOrder}");
    }

    [Test]
    public async Task Should_make_a_call_to_api_with_dns_quertString_when_calling_GetSystemsAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/systems").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _paginatedResponse.ToJsonAsync(_serializerOptions)));

        var dnsName = "test.dns";

        // Act
        await _enrolledSystemsClient.GetSystemsAsync(dnsName: dnsName);

        // Assert
        _server.Should().HaveReceivedACall().AtAbsoluteUrl($"{_server.Urls[0]}{_orgRoute}/systems?dns={dnsName}");
    }

    [Test]
    public async Task Should_make_a_call_to_api_with_page_quertString_when_calling_GetSystemsAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/systems").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _paginatedResponse.ToJsonAsync(_serializerOptions)));

        var page = 12;

        // Act
        await _enrolledSystemsClient.GetSystemsAsync(pageNumber: page);

        // Assert
        _server.Should().HaveReceivedACall().AtAbsoluteUrl($"{_server.Urls[0]}{_orgRoute}/systems?page={page}");
    }

    [Test]
    public async Task Should_make_a_call_to_api_with_per_page_quertString_when_calling_GetSystemsAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/systems").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _paginatedResponse.ToJsonAsync(_serializerOptions)));

        var page = 12;

        // Act
        await _enrolledSystemsClient.GetSystemsAsync(perPage: page);

        // Assert
        _server.Should().HaveReceivedACall().AtAbsoluteUrl($"{_server.Urls[0]}{_orgRoute}/systems?per_page={page}");
    }

    [Test]
    public async Task Should_return_number_of_revoked_systems_when_calling_RevokeSystemsAsync()
    {
        // Arrange
        var response = new BulkSystemRevokedResult(2);

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/systems").UsingDelete())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await response.ToJsonAsync(_serializerOptions)));

        // Act
        var result = await _enrolledSystemsClient.RevokeSystemsAsync("asdf", "asdf3");

        // Assert
        result.Should().Be(2);
    }

    [Test]
    public async Task Should_return_the_updated_system_when_calling_UpdateAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/systems/{_systemResponse.SystemId}").UsingPatch())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _systemResponse.ToJsonAsync(_serializerOptions)));

        // Act
        var result = await _enrolledSystemsClient.Update(_systemResponse.SystemId).Set(e => e.Description, "new description").ApplyAsync();

        // Assert
        result.Should().NotBeNull();
        result.Description.Should().Be(_systemResponse.Description);
    }

    [Test]
    public async Task Should_return_the_revoked_system_when_calling_RevokeAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/systems/{_systemResponse.SystemId}").UsingDelete())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _systemResponse.ToJsonAsync(_serializerOptions)));

        // Act
        var result = await _enrolledSystemsClient.RevokeAsync(_systemResponse.SystemId);

        // Assert
        result.Should().NotBeNull();
        result.SystemId.Should().Be(_systemResponse.SystemId);
    }

    [Test]
    public async Task Should_return_the_enabled_system_when_calling_EnableAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/systems/{_systemResponse.SystemId}/enable").UsingPut())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _systemResponse.ToJsonAsync(_serializerOptions)));

        // Act
        var result = await _enrolledSystemsClient.EnableAsync(_systemResponse.SystemId);

        // Assert
        result.Should().NotBeNull();
        result.SystemId.Should().Be(_systemResponse.SystemId);
    }

    [Test]
    public async Task Should_return_the_disabled_system_when_calling_DisableAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/systems/{_systemResponse.SystemId}/disable").UsingPut())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _systemResponse.ToJsonAsync(_serializerOptions)));

        // Act
        var result = await _enrolledSystemsClient.DisableAsync(_systemResponse.SystemId);

        // Assert
        result.Should().NotBeNull();
        result.SystemId.Should().Be(_systemResponse.SystemId);
    }

    [Test]
    public async Task Should_return_number_of_enabled_systems_when_calling_BulkEnableAsync()
    {
        // Arrange
        var response = new BulkSystemUpdateResult(2);

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/systems/enable").UsingPut())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await response.ToJsonAsync(_serializerOptions)));

        // Act
        var result = await _enrolledSystemsClient.BulkEnableAsync("asdf", "asdf3");

        // Assert
        result.Should().Be(2);
    }

    [Test]
    public async Task Should_return_number_of_disabled_systems_when_calling_BulkDisableAsync()
    {
        // Arrange
        var response = new BulkSystemUpdateResult(2);

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/systems/disable").UsingPut())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await response.ToJsonAsync(_serializerOptions)));

        // Act
        var result = await _enrolledSystemsClient.BulkDisableAsync("asdf", "asdf3");

        // Assert
        result.Should().Be(2);
    }

    // GET systems/meta/search-keys answers with the keys the search term of GetSystemsAsync accepts
    // (portal SystemsController.GetSearchKeyMetadata). The keys are two of the systems' search keys
    // (portal SystemSearchKeyService.cs) as the API writes them, with enums as names.
    [Test]
    public async Task Should_return_the_search_keys_when_calling_GetSearchKeysAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/systems/meta/search-keys").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody("""
                [
                  {
                    "name": "key",
                    "modifiers": null,
                    "dataType": "EnrolmentKey",
                    "description": "Filter your search by one or more enrolment keys.",
                    "hintText": "Filter by the name of the key used to enrol the system",
                    "hintValues": null,
                    "canHaveMultiple": false,
                    "isDefault": false,
                    "useExactMatch": false
                  },
                  {
                    "name": "tags",
                    "modifiers": [ "Or" ],
                    "dataType": "Tags",
                    "description": "Filter your search by one or more tags.",
                    "hintText": "Filter to systems that have a set of tags assigned",
                    "hintValues": null,
                    "canHaveMultiple": true,
                    "isDefault": false,
                    "useExactMatch": true
                  }
                ]
                """));

        // Act
        var result = await _enrolledSystemsClient.GetSearchKeysAsync();

        // Assert
        result.Should().BeEquivalentTo(
            new[]
            {
                new SearchKey
                {
                    Name = "key",
                    DataType = SearchKeyDataType.EnrolmentKey,
                    Description = "Filter your search by one or more enrolment keys.",
                    HintText = "Filter by the name of the key used to enrol the system",
                },
                new SearchKey
                {
                    Name = "tags",
                    Modifiers = new() { SearchModifier.Or },
                    DataType = SearchKeyDataType.Tags,
                    Description = "Filter your search by one or more tags.",
                    HintText = "Filter to systems that have a set of tags assigned",
                    CanHaveMultiple = true,
                    UseExactMatch = true,
                },
            },
            options => options.WithStrictOrdering());
    }

    private static readonly (string Name, string Method, string Suffix, Func<ISystemsClient, string, Task> Call)[] SystemIdCalls =
    {
        ("GetAsync", "GET", "", (client, id) => client.GetAsync(id)),
        ("Update", "PATCH", "", (client, id) => client.Update(id).Set(s => s.Description, "description").ApplyAsync()),
        ("RevokeAsync", "DELETE", "", (client, id) => client.RevokeAsync(id)),
        ("EnableAsync", "PUT", "/enable", (client, id) => client.EnableAsync(id)),
        ("DisableAsync", "PUT", "/disable", (client, id) => client.DisableAsync(id)),
        ("EnableUntilAsync", "PUT", "/enable-until", (client, id) => client.EnableUntilAsync(id, DateTimeOffset.UtcNow.AddHours(1), ExpiryAction.Disable)),
    };

    private static IEnumerable<TestCaseData> SystemIdEscapingCases() =>
        SystemIdCalls.Select(c => new TestCaseData(c.Method, c.Suffix, c.Call).SetArgDisplayNames(c.Name));

    private static IEnumerable<TestCaseData> RejectedSystemIdCases() =>
        from c in SystemIdCalls
        from id in new[] { null, "", ".", ".." }
        select new TestCaseData(c.Call, id).SetArgDisplayNames(c.Name, id is null ? "null" : $"\"{id}\"");

    // A system ID is one segment of the URL path. .NET removes ".." segments when it combines a relative
    // path with the base address (RFC 3986 section 5.2.4), so "../policies/1" left unescaped would send
    // DisableAsync's PUT to org/<id>/policies/1/disable, which disables policy 1. Escaped, the whole value
    // stays one segment under systems, where it names no system. The request is checked as it left the
    // client, where the escaping is visible, and the server is checked for any request that reached the
    // policies route.
    [TestCaseSource(nameof(SystemIdEscapingCases))]
    public async Task Should_send_a_system_id_as_one_escaped_path_segment(string method, string suffix, Func<ISystemsClient, string, Task> call)
    {
        // Arrange
        using var server = WireMockServer.Start();
        server
          .Given(Request.Create().WithPath("/*").UsingAnyMethod())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _systemResponse.ToJsonAsync(_serializerOptions)));

        var organisationId = OrganisationGuid.New();
        var recorder = new RecordingHttpMessageHandler(new HttpClientHandler());
        var client = new SystemsClient(new HttpClient(recorder) { BaseAddress = new Uri(server.Urls[0]) }, $"org/{organisationId}");

        // Act
        await call(client, "../policies/1");

        // Assert
        var request = recorder.Requests.Should().ContainSingle().Subject;
        request.Method.Method.Should().Be(method);
        request.Uri.AbsolutePath.Should().Be($"/org/{organisationId}/systems/..%2Fpolicies%2F1{suffix}");
        server.LogEntries.Should().ContainSingle()
            .Which.RequestMessage.Path.Should().NotStartWith($"/org/{organisationId}/policies");
    }

    // Escaping leaves "." and ".." as they are, and .NET resolves them as dot-segments, so RevokeAsync("..")
    // would DELETE org/<id>. An empty ID drops the segment and addresses the systems list, where DELETE is
    // the bulk revoke. None of these names a system, so each is refused before a request is sent.
    [TestCaseSource(nameof(RejectedSystemIdCases))]
    public async Task Should_refuse_a_system_id_that_is_not_a_path_segment_without_sending_a_request(Func<ISystemsClient, string, Task> call, string systemId)
    {
        // Arrange
        var recorder = new RecordingHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));
        var client = new SystemsClient(new HttpClient(recorder) { BaseAddress = new Uri("http://localhost/") }, $"org/{OrganisationGuid.New()}");

        // Act
        var act = () => call(client, systemId);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>().WithParameterName("systemId");
        recorder.Requests.Should().BeEmpty();
    }
}
