using Enclave.Sdk.Api.Clients;
using Enclave.Sdk.Api.Data.Pagination;
using Enclave.Sdk.Api.Data.Policies;
using FluentAssertions;
using NUnit.Framework;
using System.Text.Json;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.FluentAssertions;
using Enclave.Sdk.Api.Data;
using Enclave.Sdk.Api.Data.PatchModel;
using Enclave.Sdk.Api.Data.Policies.Enum;
using Enclave.Sdk.Api.Data.Organisations;

namespace Enclave.Sdk.Api.Tests.Clients;

public class PoliciesClientTests
{
    private PoliciesClient _policiesClient;
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

        _policiesClient = new PoliciesClient(httpClient, $"org/{organisationId}");
    }

    [Test]
    public async Task Should_return_a_paginated_response_model_when_calling_GetPoliciesAsync()
    {
        // Arrange
        var response = new PaginatedResponseModel<Policy>
        {
            Items = new List<Policy>
            {
                new Policy { Description = "test"}
            },
            Links = new PaginationLinks(),
            Metadata = new PaginationMetadata(),
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/policies").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        // Act
        var result = await _policiesClient.GetPoliciesAsync();

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public async Task Should_make_a_call_to_api_with_include_disabled_quertString_when_calling_GetPoliciesAsync()
    {
        // Arrange
        var response = new PaginatedResponseModel<Policy>
        {
            Items = new List<Policy>
            {
                new Policy { Description = "test"}
            },
            Links = new PaginationLinks(),
            Metadata = new PaginationMetadata(),
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/policies").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        var includeDisabled = true;

        // Act
        await _policiesClient.GetPoliciesAsync(includeDisabled: includeDisabled);

        // Assert
        _server.Should().HaveReceivedACall().AtAbsoluteUrl($"{_server.Urls[0]}{_orgRoute}/policies?include_disabled={includeDisabled}");
    }

    [Test]
    public async Task Should_make_a_call_to_api_with_search_quertString_when_calling_GetPoliciesAsync()
    {
        // Arrange
        var response = new PaginatedResponseModel<Policy>
        {
            Items = new List<Policy>
            {
                new Policy { Description = "test"}
            },
            Links = new PaginationLinks(),
            Metadata = new PaginationMetadata(),
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/policies").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        var searchTerm = "term";

        // Act
        await _policiesClient.GetPoliciesAsync(searchTerm: searchTerm);

        // Assert
        _server.Should().HaveReceivedACall().AtAbsoluteUrl($"{_server.Urls[0]}{_orgRoute}/policies?search={searchTerm}");
    }


    [Test]
    public async Task Should_make_a_call_to_api_with_sort_quertString_when_calling_GetPoliciesAsync()
    {
        // Arrange
        var response = new PaginatedResponseModel<Policy>
        {
            Items = new List<Policy>
            {
                new Policy { Description = "test"}
            },
            Links = new PaginationLinks(),
            Metadata = new PaginationMetadata(),
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/policies").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        var sortOrder = PolicySortOrder.RecentlyCreated;

        // Act
        await _policiesClient.GetPoliciesAsync(sortOrder: sortOrder);

        // Assert
        _server.Should().HaveReceivedACall().AtAbsoluteUrl($"{_server.Urls[0]}{_orgRoute}/policies?sort={sortOrder}");
    }

    [Test]
    public async Task Should_make_a_call_to_api_with_page_quertString_when_calling_GetPoliciesAsync()
    {
        // Arrange
        var response = new PaginatedResponseModel<Policy>
        {
            Items = new List<Policy>
            {
                new Policy { Description = "test"}
            },
            Links = new PaginationLinks(),
            Metadata = new PaginationMetadata(),
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/policies").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        var page = 12;

        // Act
        await _policiesClient.GetPoliciesAsync(pageNumber: page);

        // Assert
        _server.Should().HaveReceivedACall().AtAbsoluteUrl($"{_server.Urls[0]}{_orgRoute}/policies?page={page}");
    }

    [Test]
    public async Task Should_make_a_call_to_api_with_per_page_quertString_when_calling_GetPoliciesAsync()
    {
        // Arrange
        var response = new PaginatedResponseModel<Policy>
        {
            Items = new List<Policy>
            {
                new Policy { Description = "test"}
            },
            Links = new PaginationLinks(),
            Metadata = new PaginationMetadata(),
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/policies").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        var page = 12;

        // Act
        await _policiesClient.GetPoliciesAsync(perPage: page);

        // Assert
        _server.Should().HaveReceivedACall().AtAbsoluteUrl($"{_server.Urls[0]}{_orgRoute}/policies?per_page={page}");
    }

    [Test]
    public async Task Should_return_policy_model_when_calling_CreateAsync()
    {
        // Arrange
        var response = new Policy
        {
            Description = "test",
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/policies").UsingPost())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        var createModel = new PolicyCreate
        {
            Description = "test",
        };

        // Act
        var result = await _policiesClient.CreateAsync(createModel);

        // Assert
        result.Should().NotBeNull();
        result.Description.Should().Be(response.Description);
    }

    [Test]
    public async Task Should_return_number_of_deleted_policies_when_calling_DeletePoliciesAsync()
    {
        // Arrange
        var response = new BulkPolicyDeleteResult
        {
            PoliciesDeleted = 3,
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/policies").UsingDelete())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        // Act
        var result = await _policiesClient.DeletePoliciesAsync(PolicyId.FromInt(2), PolicyId.FromInt(3), PolicyId.FromInt(5));

        // Assert
        result.Should().Be(3);
    }

    [Test]
    public async Task Should_return_policy_model_when_calling_GetAsync()
    {
        // Arrange
        var response = new Policy
        {
            Id = PolicyId.FromInt(12),
            Description = "test",
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/policies/{response.Id}").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        // Act
        var result = await _policiesClient.GetAsync(PolicyId.FromInt(12));

        // Assert
        result.Should().NotBeNull();
        result.Description.Should().Be(response.Description);
    }

    [Test]
    public async Task Should_return_policy_model_when_calling_UpdateAsync()
    {
        // Arrange
        var response = new Policy
        {
            Id = PolicyId.FromInt(12),
            Description = "test",
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/policies/{response.Id}").UsingPatch())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        var builder = new PatchBuilder<PolicyPatch>().Set(p => p.IsEnabled, false);

        // Act
        var result = await _policiesClient.UpdateAsync(PolicyId.FromInt(12), builder);

        // Assert
        result.Should().NotBeNull();
        result.Description.Should().Be(response.Description);
    }

    [Test]
    public async Task Should_return_policy_model_when_calling_DeleteAsync()
    {
        // Arrange
        var response = new Policy
        {
            Id = PolicyId.FromInt(12),
            Description = "test",
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/policies/{response.Id}").UsingDelete())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        // Act
        var result = await _policiesClient.DeleteAsync(PolicyId.FromInt(12));

        // Assert
        result.Should().NotBeNull();
        result.Description.Should().Be(response.Description);
    }

    [Test]
    public async Task Should_return_policy_model_when_calling_EnableAsync()
    {
        // Arrange
        var response = new Policy
        {
            Id = PolicyId.FromInt(12),
            Description = "test",
            IsEnabled = true,
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/policies/{response.Id}/enable").UsingPut())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        // Act
        var result = await _policiesClient.EnableAsync(PolicyId.FromInt(12));

        // Assert
        result.Should().NotBeNull();
        result.IsEnabled.Should().Be(response.IsEnabled);
    }

    [Test]
    public async Task Should_return_policy_model_when_calling_DisableAsync()
    {
        // Arrange
        var response = new Policy
        {
            Id = PolicyId.FromInt(12),
            Description = "test",
            IsEnabled = false,
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/policies/{response.Id}/disable").UsingPut())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        // Act
        var result = await _policiesClient.DisableAsync(PolicyId.FromInt(12));

        // Assert
        result.Should().NotBeNull();
        result.IsEnabled.Should().Be(response.IsEnabled);
    }

    [Test]
    public async Task Should_return_number_of_deleted_policies_when_calling_EnablePoliciesAsync()
    {
        // Arrange
        var response = new BulkPolicyUpdateResult
        {
            PoliciesUpdated = 3,
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/policies/enable").UsingPut())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        // Act
        var result = await _policiesClient.EnablePoliciesAsync(PolicyId.FromInt(2), PolicyId.FromInt(3), PolicyId.FromInt(5));

        // Assert
        result.Should().Be(3);
    }

    [Test]
    public async Task Should_return_number_of_deleted_policies_when_calling_DisablePoliciesAsync()
    {
        // Arrange
        var response = new BulkPolicyUpdateResult
        {
            PoliciesUpdated = 3,
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/policies/disable").UsingPut())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        // Act
        var result = await _policiesClient.DisablePoliciesAsync(PolicyId.FromInt(2), PolicyId.FromInt(3), PolicyId.FromInt(5));

        // Assert
        result.Should().Be(3);
    }
}
