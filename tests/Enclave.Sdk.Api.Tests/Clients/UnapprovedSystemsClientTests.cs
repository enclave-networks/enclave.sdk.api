using Enclave.Sdk.Api.Clients;
using FluentAssertions;
using NUnit.Framework;
using System.Text.Json;
using WireMock.Server;
using WireMock.FluentAssertions;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

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
    }

    [Test]
    public async Task Should_return_a_paginated_response_model_when_calling_GetSystemsAsync()
    {
        // Arrange
        var response = new PaginatedResponseModel<UnapprovedSystemSummary>
        {
            Items = new List<UnapprovedSystemSummary>
            {
                new UnapprovedSystemSummary { Description = "test"}
            },
            Links = new PaginationLinks(),
            Metadata = new PaginationMetadata(),
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/unapproved-systems").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        // Act
        var result = await _unapprovedSystemsClient.GetSystemsAsync();

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public async Task Should_make_a_call_to_api_with_enrolment_key_quertString_when_calling_GetSystemsAsync()
    {
        // Arrange
        var response = new PaginatedResponseModel<UnapprovedSystemSummary>
        {
            Items = new List<UnapprovedSystemSummary>
            {
                new UnapprovedSystemSummary { Description = "test"}
            },
            Links = new PaginationLinks(),
            Metadata = new PaginationMetadata(),
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/unapproved-systems").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

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
        var response = new PaginatedResponseModel<UnapprovedSystemSummary>
        {
            Items = new List<UnapprovedSystemSummary>
            {
                new UnapprovedSystemSummary { Description = "test"}
            },
            Links = new PaginationLinks(),
            Metadata = new PaginationMetadata(),
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/unapproved-systems").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

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
        var response = new PaginatedResponseModel<UnapprovedSystemSummary>
        {
            Items = new List<UnapprovedSystemSummary>
            {
                new UnapprovedSystemSummary { Description = "test"}
            },
            Links = new PaginationLinks(),
            Metadata = new PaginationMetadata(),
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/unapproved-systems").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

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
        var response = new PaginatedResponseModel<UnapprovedSystemSummary>
        {
            Items = new List<UnapprovedSystemSummary>
            {
                new UnapprovedSystemSummary { Description = "test"}
            },
            Links = new PaginationLinks(),
            Metadata = new PaginationMetadata(),
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/unapproved-systems").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

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
        var response = new PaginatedResponseModel<UnapprovedSystemSummary>
        {
            Items = new List<UnapprovedSystemSummary>
            {
                new UnapprovedSystemSummary { Description = "test"}
            },
            Links = new PaginationLinks(),
            Metadata = new PaginationMetadata(),
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/unapproved-systems").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        var page = 12;

        // Act
        await _unapprovedSystemsClient.GetSystemsAsync(perPage: page);

        // Assert
        _server.Should().HaveReceivedACall().AtAbsoluteUrl($"{_server.Urls[0]}{_orgRoute}/unapproved-systems?per_page={page}");
    }

    [Test]
    public async Task Should_return_number_of_declined_systems_when_calling_DeclineSystems()
    {
        var response = new BulkUnapprovedSystemDeclineResult
        {
            SystemsDeclined = 2,
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/unapproved-systems").UsingDelete())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        // Act
        var result = await _unapprovedSystemsClient.DeclineSystems(SystemId.FromString("system1"), SystemId.FromString("system2"));

        // Assert
        result.Should().Be(2);
    }

    [Test]
    public async Task Should_return_unapproved_system_detail_model_when_calling_GetAsync()
    {
        // Arrange
        var response = new UnapprovedSystem
        {
            SystemId = SystemId.FromString("newId"),
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/unapproved-systems/{response.SystemId}").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        // Act
        var result = await _unapprovedSystemsClient.GetAsync(SystemId.FromString("newId"));

        // Assert
        result.Should().NotBeNull();
        result.SystemId.Should().Be(response.SystemId);
    }

    [Test]
    public async Task Should_return_unapproved_system_detail_model_when_calling_UpdateAsync()
    {
        // Arrange
        var response = new UnapprovedSystem
        {
            SystemId = SystemId.FromString("newId"),
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/unapproved-systems/{response.SystemId}").UsingPatch())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        // Act
        var result = await _unapprovedSystemsClient.Update(SystemId.FromString("newId")).Set(u => u.Description, "New System").ApplyAsync();

        // Assert
        result.Should().NotBeNull();
        result.SystemId.Should().Be(response.SystemId);
    }

    [Test]
    public async Task Should_return_unapproved_system_detail_model_when_calling_DeclineAsync()
    {
        // Arrange
        var response = new UnapprovedSystem
        {
            SystemId = SystemId.FromString("newId"),
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/unapproved-systems/{response.SystemId}").UsingDelete())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        // Act
        var result = await _unapprovedSystemsClient.DeclineAsync(SystemId.FromString("newId"));

        // Assert
        result.Should().NotBeNull();
        result.SystemId.Should().Be(response.SystemId);
    }

    [Test]
    public async Task Should_not_throw_an_error_when_calling_ApproveAsync()
    {
        // Arrange
        var response = new UnapprovedSystem
        {
            SystemId = SystemId.FromString("newId"),
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/unapproved-systems/{response.SystemId}/approve").UsingPut())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        // Act
        await _unapprovedSystemsClient.ApproveAsync(SystemId.FromString("newId"));
    }

    [Test]
    public async Task Should_return_number_of_approved_systems_when_calling_ApproveSystemsAsync()
    {
        var response = new BulkUnapprovedSystemApproveResult
        {
            SystemsApproved = 2,
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/unapproved-systems/approve").UsingPut())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        // Act
        var result = await _unapprovedSystemsClient.ApproveSystemsAsync(SystemId.FromString("system1") , SystemId.FromString("system2"));

        // Assert
        result.Should().Be(2);
    }
}