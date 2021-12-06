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

namespace Enclave.Sdk.Api.Tests.Clients;

public class EnrolledSystemClientTests
{
    private EnrolledSystemsClient _enrolledSystemsClient;
    private WireMockServer _server;
    private string _orgRoute;
    private JsonSerializerOptions _serializerOptions = new()
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

}
