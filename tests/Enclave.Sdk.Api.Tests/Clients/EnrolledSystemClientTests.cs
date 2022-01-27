using Enclave.Sdk.Api.Clients;
using Enclave.Sdk.Api.Data.Account;
using Enclave.Sdk.Api.Data.EnrolledSystems;
using Enclave.Sdk.Api.Data.Pagination;
using FluentAssertions;
using NUnit.Framework;
using System.Text.Json;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.FluentAssertions;
using Enclave.Sdk.Api.Data.EnrolledSystems.Enum;
using Enclave.Sdk.Api.Data.Organisations;
using Enclave.Sdk.Api.Data;
using Enclave.Sdk.Api.Data.PatchModel;

namespace Enclave.Sdk.Api.Tests.Clients;

public class EnrolledSystemClientTests
{
    private EnrolledSystemsClient _enrolledSystemsClient;
    private WireMockServer _server;
    private string _orgRoute;
    private readonly JsonSerializerOptions _serializerOptions = new()
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

        var organisationId = OrganisationId.New();
        _orgRoute = $"/org/{organisationId}";

        _enrolledSystemsClient = new EnrolledSystemsClient(httpClient, $"org/{organisationId}");
    }

    [Test]
    public async Task Should_return_a_paginated_response_model_when_calling_GetSystemsAsync()
    {
        // Arrange
        var response = new PaginatedResponseModel<EnrolledSystemSummary>
        {
            Items = new List<EnrolledSystemSummary>
            {
                new EnrolledSystemSummary { Description = "test"}
            },
            Links = new PaginationLinks(),
            Metadata = new PaginationMetadata(),
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/systems").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        // Act
        var result = await _enrolledSystemsClient.GetSystemsAsync();

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public async Task Should_make_a_call_to_api_with_enrolment_key_quertString_when_calling_GetSystemsAsync()
    {
        // Arrange
        var response = new PaginatedResponseModel<EnrolledSystemSummary>
        {
            Items = new List<EnrolledSystemSummary>
            {
                new EnrolledSystemSummary { Description = "test"}
            },
            Links = new PaginationLinks(),
            Metadata = new PaginationMetadata(),
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/systems").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

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
        var response = new PaginatedResponseModel<EnrolledSystemSummary>
        {
            Items = new List<EnrolledSystemSummary>
            {
                new EnrolledSystemSummary { Description = "test"}
            },
            Links = new PaginationLinks(),
            Metadata = new PaginationMetadata(),
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/systems").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

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
        var response = new PaginatedResponseModel<EnrolledSystemSummary>
        {
            Items = new List<EnrolledSystemSummary>
            {
                new EnrolledSystemSummary { Description = "test"}
            },
            Links = new PaginationLinks(),
            Metadata = new PaginationMetadata(),
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/systems").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

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
        var response = new PaginatedResponseModel<EnrolledSystemSummary>
        {
            Items = new List<EnrolledSystemSummary>
            {
                new EnrolledSystemSummary { Description = "test"}
            },
            Links = new PaginationLinks(),
            Metadata = new PaginationMetadata(),
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/systems").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

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
        var response = new PaginatedResponseModel<EnrolledSystemSummary>
        {
            Items = new List<EnrolledSystemSummary>
            {
                new EnrolledSystemSummary { Description = "test"}
            },
            Links = new PaginationLinks(),
            Metadata = new PaginationMetadata(),
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/systems").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

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
        var response = new PaginatedResponseModel<EnrolledSystemSummary>
        {
            Items = new List<EnrolledSystemSummary>
            {
                new EnrolledSystemSummary { Description = "test"}
            },
            Links = new PaginationLinks(),
            Metadata = new PaginationMetadata(),
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/systems").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

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
        var response = new PaginatedResponseModel<EnrolledSystemSummary>
        {
            Items = new List<EnrolledSystemSummary>
            {
                new EnrolledSystemSummary { Description = "test"}
            },
            Links = new PaginationLinks(),
            Metadata = new PaginationMetadata(),
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/systems").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

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
        var response = new BulkSystemRevokedResult
        {
            SystemsRevoked = 2,
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/systems").UsingDelete())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        // Act
        var result = await _enrolledSystemsClient.RevokeSystemsAsync(SystemId.FromString("asdf"), SystemId.FromString("asdf3"));

        // Assert
        result.Should().Be(2);
    }

    [Test]
    public async Task Should_return_the_updated_system_when_calling_UpdateAsync()
    {
        // Arrange
        var response = new EnrolledSystem
        {
            SystemId = SystemId.FromString("system1"),
            Description = "new description",
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/systems/{response.SystemId}").UsingPatch())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        // Act
        var result = await _enrolledSystemsClient.Update(response.SystemId).Set(e => e.Description, "new description").ApplyAsync();

        // Assert
        result.Should().NotBeNull();
        result.Description.Should().Be(response.Description);
    }

    [Test]
    public async Task Should_return_the_revoked_system_when_calling_RevokeAsync()
    {
        // Arrange
        var response = new EnrolledSystem
        {
            SystemId = SystemId.FromString("system1"),
            Description = "description",
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/systems/{response.SystemId}").UsingDelete())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        // Act
        var result = await _enrolledSystemsClient.RevokeAsync(response.SystemId);

        // Assert
        result.Should().NotBeNull();
        result.SystemId.Should().Be(response.SystemId);
    }

    [Test]
    public async Task Should_return_the_enabled_system_when_calling_EnableAsync()
    {
        // Arrange
        var response = new EnrolledSystem
        {
            SystemId = SystemId.FromString("system1"),
            Description = "description",
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/systems/{response.SystemId}/enable").UsingPut())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        // Act
        var result = await _enrolledSystemsClient.EnableAsync(response.SystemId);

        // Assert
        result.Should().NotBeNull();
        result.SystemId.Should().Be(response.SystemId);
    }

    [Test]
    public async Task Should_return_the_disabled_system_when_calling_DisableAsync()
    {
        // Arrange
        var response = new EnrolledSystem
        {
            SystemId = SystemId.FromString("system1"),
            Description = "description",
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/systems/{response.SystemId}/disable").UsingPut())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        // Act
        var result = await _enrolledSystemsClient.DisableAsync(response.SystemId);

        // Assert
        result.Should().NotBeNull();
        result.SystemId.Should().Be(response.SystemId);
    }

    [Test]
    public async Task Should_return_number_of_enabled_systems_when_calling_BulkEnableAsync()
    {
        // Arrange
        var response = new BulkSystemUpdateResult
        {
            SystemsUpdated = 2,
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/systems/enable").UsingPut())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        // Act
        var result = await _enrolledSystemsClient.BulkEnableAsync(SystemId.FromString("asdf"), SystemId.FromString("asdf3"));

        // Assert
        result.Should().Be(2);
    }

    [Test]
    public async Task Should_return_number_of_disabled_systems_when_calling_BulkDisableAsync()
    {
        // Arrange
        var response = new BulkSystemUpdateResult
        {
            SystemsUpdated = 2,
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/systems/disable").UsingPut())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        // Act
        var result = await _enrolledSystemsClient.BulkDisableAsync(SystemId.FromString("asdf"), SystemId.FromString("asdf3"));

        // Assert
        result.Should().Be(2);
    }


}
