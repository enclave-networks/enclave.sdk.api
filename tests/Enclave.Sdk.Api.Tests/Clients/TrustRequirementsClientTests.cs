using Enclave.Sdk.Api.Clients;
using FluentAssertions;
using NUnit.Framework;
using System.Text.Json;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.FluentAssertions;
using Enclave.Configuration.Data.Identifiers;
using Enclave.Api.Scaffolding.Pagination.Models;
using Enclave.Api.Modules.SystemManagement.TrustRequirements.Models;
using Enclave.Api.Modules.SystemManagement.Tags.Models;
using Enclave.Sdk.Network.Abstractions.NetworkPolicy;
using Enclave.Configuration.Data.Modules.TrustRequirements.Enums;

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

    private PaginatedResponseModel<TrustRequirementSummaryModel> _paginatedResponse;
    private TrustRequirementModel _trustRequirementResponse;

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

        _paginatedResponse = new(
            new(0, 0, 0, 0, 0),
            new(new Uri("http://enclave.io"), new Uri("http://enclave.io"), new Uri("http://enclave.io"), new Uri("http://enclave.io")),
            new List<TrustRequirementSummaryModel>
            {
                new(TrustRequirementId.FromInt(12),
                    "trust",
                    DateTime.Now,
                    DateTime.Now,
                    TrustRequirementType.UserAuthentication,
                    0,
                    0,
                    "summary"),
            }.ToAsyncEnumerable());

        _trustRequirementResponse = new(TrustRequirementId.FromInt(12),
                    "trust",
                    DateTime.Now,
                    DateTime.Now,
                    TrustRequirementType.UserAuthentication,
                    0,
                    0,
                    "summary",
                    new TrustRequirementSettingsModel(new Dictionary<string, string>(), new List<IReadOnlyDictionary<string,string>>()));
    }

    [Test]
    public async Task Should_return_a_paginated_response_model_when_calling_GetTrustRequirementsAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/trust-requirements").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _paginatedResponse.ToJsonAsync(_serializerOptions)));

        // Act
        var result = await _trustRequirementsClient.GetTrustRequirementsAsync();

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public async Task Should_make_a_call_to_api_with_search_quertString_when_calling_GetTrustRequirementsAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/trust-requirements").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _paginatedResponse.ToJsonAsync(_serializerOptions)));

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
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/trust-requirements").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _paginatedResponse.ToJsonAsync(_serializerOptions)));

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
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/trust-requirements").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _paginatedResponse.ToJsonAsync(_serializerOptions)));

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
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/trust-requirements").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _paginatedResponse.ToJsonAsync(_serializerOptions)));

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
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/trust-requirements").UsingPost())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _trustRequirementResponse.ToJsonAsync(_serializerOptions)));

        var createModel = new TrustRequirementCreateModel(
            "test",
            TrustRequirementType.PublicIp,
            null,
            new TrustRequirementSettingsModel(new Dictionary<string, string>(), new List<IReadOnlyDictionary<string, string>>()));

        // Act
        var result = await _trustRequirementsClient.CreateAsync(createModel);

        // Assert
        result.Should().NotBeNull();
        result.Description.Should().Be(_trustRequirementResponse.Description);
    }

    [Test]
    public async Task Should_return_number_of_deleted_policies_when_calling_DeletePoliciesAsync()
    {
        // Arrange
        var response = new BulkTrustRequirementDeleteResult(3);

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/trust-requirements").UsingDelete())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await response.ToJsonAsync(_serializerOptions)));

        // Act
        var result = await _trustRequirementsClient.DeleteTrustRequirementsAsync(TrustRequirementId.FromInt(2), TrustRequirementId.FromInt(3), TrustRequirementId.FromInt(5));

        // Assert
        result.Should().Be(3);
    }

    [Test]
    public async Task Should_return_policy_model_when_calling_GetAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/trust-requirements/{_trustRequirementResponse.Id}").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _trustRequirementResponse.ToJsonAsync(_serializerOptions)));

        // Act
        var result = await _trustRequirementsClient.GetAsync(TrustRequirementId.FromInt(12));

        // Assert
        result.Should().NotBeNull();
        result.Description.Should().Be(_trustRequirementResponse.Description);
    }

    [Test]
    public async Task Should_return_policy_model_when_calling_UpdateAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/trust-requirements/{_trustRequirementResponse.Id}").UsingPatch())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _trustRequirementResponse.ToJsonAsync(_serializerOptions)));

        // Act
        var result = await _trustRequirementsClient.Update(TrustRequirementId.FromInt(12)).Set(p => p.Description, "New Description").ApplyAsync();

        // Assert
        result.Should().NotBeNull();
        result.Description.Should().Be(_trustRequirementResponse.Description);
    }

    [Test]
    public async Task Should_return_policy_model_when_calling_DeleteAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/trust-requirements/{_trustRequirementResponse.Id}").UsingDelete())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _trustRequirementResponse.ToJsonAsync(_serializerOptions)));

        // Act
        var result = await _trustRequirementsClient.DeleteAsync(TrustRequirementId.FromInt(12));

        // Assert
        result.Should().NotBeNull();
        result.Description.Should().Be(_trustRequirementResponse.Description);
    }
}
