using Enclave.Sdk.Api.Clients;
using FluentAssertions;
using NUnit.Framework;
using System.Text.Json;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.FluentAssertions;

namespace Enclave.Sdk.Api.Tests.Clients;

public class TrustRequirementsClientTests
{
    private TrustRequirementsClient _trustRequirementsClient;
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

        var organisationId = OrganisationGuid.New();
        _orgRoute = $"/org/{organisationId}";

        _trustRequirementsClient = new TrustRequirementsClient(httpClient, $"org/{organisationId}");
    }

    [Test]
    public async Task Should_return_a_paginated_response_model_when_calling_GetTrustRequirementsAsync()
    {
        // Arrange
        var response = new PaginatedResponseModel<TrustRequirementSummary>
        {
            Items = new List<TrustRequirementSummary>
            {
                new TrustRequirementSummary { Description = "test"}
            },
            Links = new PaginationLinks(),
            Metadata = new PaginationMetadata(),
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/trust-requirements").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        // Act
        var result = await _trustRequirementsClient.GetTrustRequirementsAsync();

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public async Task Should_make_a_call_to_api_with_search_quertString_when_calling_GetTrustRequirementsAsync()
    {
        // Arrange
        var response = new PaginatedResponseModel<TrustRequirementSummary>
        {
            Items = new List<TrustRequirementSummary>
            {
                new TrustRequirementSummary { Description = "test"}
            },
            Links = new PaginationLinks(),
            Metadata = new PaginationMetadata(),
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/trust-requirements").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        var searchTerm = "term";

        // Act
        await _trustRequirementsClient.GetTrustRequirementsAsync(searchTerm: searchTerm);

        // Assert
        _server.Should().HaveReceivedACall().AtAbsoluteUrl($"{_server.Urls[0]}{_orgRoute}/trust-requirements?search={searchTerm}");
    }


    [Test]
    public async Task Should_make_a_call_to_api_with_sort_quertString_when_calling_GetTrustRequirementsAsync()
    {
        // Arrange
        var response = new PaginatedResponseModel<TrustRequirementSummary>
        {
            Items = new List<TrustRequirementSummary>
            {
                new TrustRequirementSummary { Description = "test"}
            },
            Links = new PaginationLinks(),
            Metadata = new PaginationMetadata(),
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/trust-requirements").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        var sortOrder = TrustRequirementSortOrder.RecentlyCreated;

        // Act
        await _trustRequirementsClient.GetTrustRequirementsAsync(sortOrder: sortOrder);

        // Assert
        _server.Should().HaveReceivedACall().AtAbsoluteUrl($"{_server.Urls[0]}{_orgRoute}/trust-requirements?sort={sortOrder}");
    }

    [Test]
    public async Task Should_make_a_call_to_api_with_page_quertString_when_calling_GetTrustRequirementsAsync()
    {
        // Arrange
        var response = new PaginatedResponseModel<TrustRequirementSummary>
        {
            Items = new List<TrustRequirementSummary>
            {
                new TrustRequirementSummary { Description = "test"}
            },
            Links = new PaginationLinks(),
            Metadata = new PaginationMetadata(),
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/trust-requirements").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        var page = 12;

        // Act
        await _trustRequirementsClient.GetTrustRequirementsAsync(pageNumber: page);

        // Assert
        _server.Should().HaveReceivedACall().AtAbsoluteUrl($"{_server.Urls[0]}{_orgRoute}/trust-requirements?page={page}");
    }

    [Test]
    public async Task Should_make_a_call_to_api_with_per_page_quertString_when_calling_GetTrustRequirementsAsync()
    {
        // Arrange
        var response = new PaginatedResponseModel<TrustRequirementSummary>
        {
            Items = new List<TrustRequirementSummary>
            {
                new TrustRequirementSummary { Description = "test"}
            },
            Links = new PaginationLinks(),
            Metadata = new PaginationMetadata(),
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/trust-requirements").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        var page = 12;

        // Act
        await _trustRequirementsClient.GetTrustRequirementsAsync(perPage: page);

        // Assert
        _server.Should().HaveReceivedACall().AtAbsoluteUrl($"{_server.Urls[0]}{_orgRoute}/trust-requirements?per_page={page}");
    }

    [Test]
    public async Task Should_return_policy_model_when_calling_CreateAsync()
    {
        // Arrange
        var response = new TrustRequirement
        {
            Description = "test",
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/trust-requirements").UsingPost())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        var createModel = new TrustRequirementCreate
        {
            Description = "test",
        };

        // Act
        var result = await _trustRequirementsClient.CreateAsync(createModel);

        // Assert
        result.Should().NotBeNull();
        result.Description.Should().Be(response.Description);
    }

    [Test]
    public async Task Should_return_number_of_deleted_policies_when_calling_DeletePoliciesAsync()
    {
        // Arrange
        var response = new BulkTrustRequirementDeleteResult
        {
            RequirementsDeleted = 3,
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/trust-requirements").UsingDelete())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        // Act
        var result = await _trustRequirementsClient.DeleteTrustRequirementsAsync(TrustRequirementId.FromInt(2), TrustRequirementId.FromInt(3), TrustRequirementId.FromInt(5));

        // Assert
        result.Should().Be(3);
    }

    [Test]
    public async Task Should_return_policy_model_when_calling_GetAsync()
    {
        // Arrange
        var response = new TrustRequirement
        {
            Id = TrustRequirementId.FromInt(12),
            Description = "test",
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/trust-requirements/{response.Id}").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        // Act
        var result = await _trustRequirementsClient.GetAsync(TrustRequirementId.FromInt(12));

        // Assert
        result.Should().NotBeNull();
        result.Description.Should().Be(response.Description);
    }

    [Test]
    public async Task Should_return_policy_model_when_calling_UpdateAsync()
    {
        // Arrange
        var response = new TrustRequirement
        {
            Id = TrustRequirementId.FromInt(12),
            Description = "test",
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/trust-requirements/{response.Id}").UsingPatch())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        // Act
        var result = await _trustRequirementsClient.Update(TrustRequirementId.FromInt(12)).Set(p => p.Description, "New Description").ApplyAsync();

        // Assert
        result.Should().NotBeNull();
        result.Description.Should().Be(response.Description);
    }

    [Test]
    public async Task Should_return_policy_model_when_calling_DeleteAsync()
    {
        // Arrange
        var response = new TrustRequirement
        {
            Id = TrustRequirementId.FromInt(12),
            Description = "test",
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/trust-requirements/{response.Id}").UsingDelete())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        // Act
        var result = await _trustRequirementsClient.DeleteAsync(TrustRequirementId.FromInt(12));

        // Assert
        result.Should().NotBeNull();
        result.Description.Should().Be(response.Description);
    }
}
