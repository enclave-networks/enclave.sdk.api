using Enclave.Sdk.Api.Clients;
using FluentAssertions;
using NUnit.Framework;
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
}
