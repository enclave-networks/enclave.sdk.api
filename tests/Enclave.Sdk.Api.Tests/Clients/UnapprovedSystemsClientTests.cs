using Enclave.Sdk.Api.Clients;
using Enclave.Sdk.Api.Clients.Interfaces;
using Enclave.Sdk.Api.Data;
using FluentAssertions;
using NUnit.Framework;
using System.Net;
using System.Text.Json;
using WireMock.Server;
using WireMock.FluentAssertions;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using Enclave.Configuration.Data.Identifiers;
using Enclave.Api.Scaffolding.Pagination.Models;
using Enclave.Api.Modules.SystemManagement.Systems.Models;
using Enclave.Api.Modules.SystemManagement.Tags.Models;
using Enclave.Configuration.Data.Modules.Systems.Enums;
using Enclave.Api.Modules.SystemManagement.UnapprovedSystems.Models;

namespace Enclave.Sdk.Api.Tests.Clients;

public class UnapprovedSystemsClientTests
{
    private readonly UnapprovedSystemsClient _unapprovedSystemsClient;
    private readonly WireMockServer _server;
    private readonly string _orgRoute;
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };


    private PaginatedResponseModel<UnapprovedSystemSummaryModel> _paginatedResponse;
    private UnapprovedSystemModel _unapprovedSystemResponse;

    public UnapprovedSystemsClientTests()
    {
        _server = WireMockServer.Start();

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri(_server.Urls[0]),
        };

        var organisationId = OrganisationGuid.New();
        _orgRoute = $"/org/{organisationId}";

        _unapprovedSystemsClient = new UnapprovedSystemsClient(httpClient, $"org/{organisationId}");

        _paginatedResponse = new(
                new(0, 0, 0, 0, 0),
                new(new Uri("http://enclave.io"), new Uri("http://enclave.io"), new Uri("http://enclave.io"), new Uri("http://enclave.io")),
                new List<UnapprovedSystemSummaryModel>
                {
                    new("newId",
                        SystemType.GeneralPurpose,
                        "system",
                        string.Empty,
                        DateTime.Now,
                        EnrolmentKeyId.FromInt(1),
                        string.Empty,
                        Array.Empty<IUsedTagModel>())
                }.ToAsyncEnumerable());

        _unapprovedSystemResponse = new UnapprovedSystemModel("newId",
                        SystemType.GeneralPurpose,
                        "system",
                        string.Empty,
                        DateTime.Now,
                        EnrolmentKeyId.FromInt(1),
                        string.Empty,
                        false,
                        Array.Empty<IUsedTagModel>(),
                        null);
    }

    [Test]
    public async Task Should_return_a_paginated_response_model_when_calling_GetSystemsAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/unapproved-systems").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _paginatedResponse.ToJsonAsync(_serializerOptions)));

        // Act
        var result = await _unapprovedSystemsClient.GetSystemsAsync();

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public async Task Should_make_a_call_to_api_with_enrolment_key_quertString_when_calling_GetSystemsAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/unapproved-systems").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _paginatedResponse.ToJsonAsync(_serializerOptions)));

        var enrolmentKeyId = 12;

        // Act
        await _unapprovedSystemsClient.GetSystemsAsync(enrolmentKeyId: enrolmentKeyId);

        // Assert
        _server.Should().HaveReceivedACall().AtAbsoluteUrl($"{_server.Urls[0]}{_orgRoute}/unapproved-systems?enrolment_key={enrolmentKeyId}");
    }

    [Test]
    public async Task Should_make_a_call_to_api_with_search_quertString_when_calling_GetSystemsAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/unapproved-systems").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _paginatedResponse.ToJsonAsync(_serializerOptions)));

        var searchTerm = "term";

        // Act
        await _unapprovedSystemsClient.GetSystemsAsync(searchTerm: searchTerm);

        // Assert
        _server.Should().HaveReceivedACall().AtAbsoluteUrl($"{_server.Urls[0]}{_orgRoute}/unapproved-systems?search={searchTerm}");
    }


    [Test]
    public async Task Should_make_a_call_to_api_with_sort_quertString_when_calling_GetSystemsAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/unapproved-systems").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _paginatedResponse.ToJsonAsync(_serializerOptions)));

        var sortOrder = UnapprovedSystemQuerySortMode.RecentlyEnrolled;

        // Act
        await _unapprovedSystemsClient.GetSystemsAsync(sortOrder: sortOrder);

        // Assert
        _server.Should().HaveReceivedACall().AtAbsoluteUrl($"{_server.Urls[0]}{_orgRoute}/unapproved-systems?sort={sortOrder}");
    }

    [Test]
    public async Task Should_make_a_call_to_api_with_page_quertString_when_calling_GetSystemsAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/unapproved-systems").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _paginatedResponse.ToJsonAsync(_serializerOptions)));

        var page = 12;

        // Act
        await _unapprovedSystemsClient.GetSystemsAsync(pageNumber: page);

        // Assert
        _server.Should().HaveReceivedACall().AtAbsoluteUrl($"{_server.Urls[0]}{_orgRoute}/unapproved-systems?page={page}");
    }

    [Test]
    public async Task Should_make_a_call_to_api_with_per_page_quertString_when_calling_GetSystemsAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/unapproved-systems").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _paginatedResponse.ToJsonAsync(_serializerOptions)));

        var page = 12;

        // Act
        await _unapprovedSystemsClient.GetSystemsAsync(perPage: page);

        // Assert
        _server.Should().HaveReceivedACall().AtAbsoluteUrl($"{_server.Urls[0]}{_orgRoute}/unapproved-systems?per_page={page}");
    }

    [Test]
    public async Task Should_return_number_of_declined_systems_when_calling_DeclineSystems()
    {
        var response = new BulkUnapprovedSystemDeclineResult(2);

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/unapproved-systems").UsingDelete())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await response.ToJsonAsync(_serializerOptions)));

        // Act
        var result = await _unapprovedSystemsClient.DeclineSystems("system1", "system2");

        // Assert
        result.Should().Be(2);
    }

    [Test]
    public async Task Should_return_unapproved_system_detail_model_when_calling_GetAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/unapproved-systems/{_unapprovedSystemResponse.SystemId}").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _unapprovedSystemResponse.ToJsonAsync(_serializerOptions)));

        // Act
        var result = await _unapprovedSystemsClient.GetAsync("newId");

        // Assert
        result.Should().NotBeNull();
        result.SystemId.Should().Be(_unapprovedSystemResponse.SystemId);
    }

    [Test]
    public async Task Should_return_unapproved_system_detail_model_when_calling_UpdateAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/unapproved-systems/{_unapprovedSystemResponse.SystemId}").UsingPatch())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _unapprovedSystemResponse.ToJsonAsync(_serializerOptions)));

        // Act
        var result = await _unapprovedSystemsClient.Update("newId").Set(u => u.Description, "New System").ApplyAsync();

        // Assert
        result.Should().NotBeNull();
        result.SystemId.Should().Be(_unapprovedSystemResponse.SystemId);
    }

    [Test]
    public async Task Should_return_unapproved_system_detail_model_when_calling_DeclineAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/unapproved-systems/{_unapprovedSystemResponse.SystemId}").UsingDelete())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _unapprovedSystemResponse.ToJsonAsync(_serializerOptions)));

        // Act
        var result = await _unapprovedSystemsClient.DeclineAsync("newId");

        // Assert
        result.Should().NotBeNull();
        result.SystemId.Should().Be(_unapprovedSystemResponse.SystemId);
    }

    [Test]
    public async Task Should_not_throw_an_error_when_calling_ApproveAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/unapproved-systems/{_unapprovedSystemResponse.SystemId}/approve").UsingPut())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _unapprovedSystemResponse.ToJsonAsync(_serializerOptions)));

        // Act
        await _unapprovedSystemsClient.ApproveAsync("newId");
    }

    [Test]
    public async Task Should_return_number_of_approved_systems_when_calling_ApproveSystemsAsync()
    {
        var response = new BulkUnapprovedSystemApproveResult(2);

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/unapproved-systems/approve").UsingPut())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await response.ToJsonAsync(_serializerOptions)));

        // Act
        var result = await _unapprovedSystemsClient.ApproveSystemsAsync("system1" , "system2");

        // Assert
        result.Should().Be(2);
    }

    // GET unapproved-systems/meta/search-keys answers with the keys the search term of GetSystemsAsync
    // accepts (portal UnapprovedSystemsController.GetSearchKeyMetadata). Unapproved systems are searched
    // with the systems' search keys (portal SystemSearchKeyService.cs), and these are two of them as the
    // API writes them, with enums as names.
    [Test]
    public async Task Should_return_the_search_keys_when_calling_GetSearchKeysAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/unapproved-systems/meta/search-keys").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody("""
                [
                  {
                    "name": "description",
                    "modifiers": null,
                    "dataType": "None",
                    "description": "Filter your search by description.",
                    "hintText": "Search by description or hostname",
                    "hintValues": null,
                    "canHaveMultiple": false,
                    "isDefault": true,
                    "useExactMatch": false
                  },
                  {
                    "name": "os",
                    "modifiers": null,
                    "dataType": "None",
                    "description": "Filter your search by one or more operating systems.",
                    "hintText": "Filter by system platform (e.g. windows, linux, macos)",
                    "hintValues": [ "Windows", "Linux", "Mac" ],
                    "canHaveMultiple": false,
                    "isDefault": false,
                    "useExactMatch": true
                  }
                ]
                """));

        // Act
        var result = await _unapprovedSystemsClient.GetSearchKeysAsync();

        // Assert
        result.Should().BeEquivalentTo(
            new[]
            {
                new SearchKey
                {
                    Name = "description",
                    DataType = SearchKeyDataType.None,
                    Description = "Filter your search by description.",
                    HintText = "Search by description or hostname",
                    IsDefault = true,
                },
                new SearchKey
                {
                    Name = "os",
                    DataType = SearchKeyDataType.None,
                    Description = "Filter your search by one or more operating systems.",
                    HintText = "Filter by system platform (e.g. windows, linux, macos)",
                    HintValues = new() { "Windows", "Linux", "Mac" },
                    UseExactMatch = true,
                },
            },
            options => options.WithStrictOrdering());
    }

    private static readonly (string Name, string Method, string Suffix, Func<IUnapprovedSystemsClient, string, Task> Call)[] SystemIdCalls =
    {
        ("GetAsync", "GET", "", (client, id) => client.GetAsync(id)),
        ("Update", "PATCH", "", (client, id) => client.Update(id).Set(u => u.Description, "description").ApplyAsync()),
        ("DeclineAsync", "DELETE", "", (client, id) => client.DeclineAsync(id)),
        ("ApproveAsync", "PUT", "/approve", (client, id) => client.ApproveAsync(id)),
    };

    private static IEnumerable<TestCaseData> SystemIdEscapingCases() =>
        SystemIdCalls.Select(c => new TestCaseData(c.Method, c.Suffix, c.Call).SetArgDisplayNames(c.Name));

    private static IEnumerable<TestCaseData> RejectedSystemIdCases() =>
        from c in SystemIdCalls
        from id in new[] { null, "", ".", ".." }
        select new TestCaseData(c.Call, id).SetArgDisplayNames(c.Name, id is null ? "null" : $"\"{id}\"");

    // A system ID is one segment of the URL path. .NET removes ".." segments when it combines a relative
    // path with the base address (RFC 3986 section 5.2.4), so "../systems/ABCDE" left unescaped would send
    // DeclineAsync's DELETE to org/<id>/systems/ABCDE, which revokes enrolled system ABCDE. Escaped, the
    // whole value stays one segment under unapproved-systems, where it names no system. The request is
    // checked as it left the client, where the escaping is visible, and the server is checked for any
    // request that reached the enrolled systems route.
    [TestCaseSource(nameof(SystemIdEscapingCases))]
    public async Task Should_send_a_system_id_as_one_escaped_path_segment(string method, string suffix, Func<IUnapprovedSystemsClient, string, Task> call)
    {
        // Arrange
        using var server = WireMockServer.Start();
        server
          .Given(Request.Create().WithPath("/*").UsingAnyMethod())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _unapprovedSystemResponse.ToJsonAsync(_serializerOptions)));

        var organisationId = OrganisationGuid.New();
        var recorder = new RecordingHttpMessageHandler(new HttpClientHandler());
        var client = new UnapprovedSystemsClient(new HttpClient(recorder) { BaseAddress = new Uri(server.Urls[0]) }, $"org/{organisationId}");

        // Act
        await call(client, "../systems/ABCDE");

        // Assert
        var request = recorder.Requests.Should().ContainSingle().Subject;
        request.Method.Method.Should().Be(method);
        request.Uri.AbsolutePath.Should().Be($"/org/{organisationId}/unapproved-systems/..%2Fsystems%2FABCDE{suffix}");
        server.LogEntries.Should().ContainSingle()
            .Which.RequestMessage.Path.Should().NotStartWith($"/org/{organisationId}/systems");
    }

    // Escaping leaves "." and ".." as they are, and .NET resolves them as dot-segments, so ApproveAsync("..")
    // would PUT org/<id>/approve. An empty ID drops the segment and addresses the unapproved systems list.
    // None of these names a system, so each is refused before a request is sent.
    [TestCaseSource(nameof(RejectedSystemIdCases))]
    public async Task Should_refuse_a_system_id_that_is_not_a_path_segment_without_sending_a_request(Func<IUnapprovedSystemsClient, string, Task> call, string systemId)
    {
        // Arrange
        var recorder = new RecordingHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));
        var client = new UnapprovedSystemsClient(new HttpClient(recorder) { BaseAddress = new Uri("http://localhost/") }, $"org/{OrganisationGuid.New()}");

        // Act
        var act = () => call(client, systemId);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>().WithParameterName("systemId");
        recorder.Requests.Should().BeEmpty();
    }
}