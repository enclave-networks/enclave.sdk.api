using Enclave.Sdk.Api.Clients;
using Enclave.Sdk.Api.Data.EnrolmentKeys;
using Enclave.Sdk.Api.Data.EnrolmentKeys.Enum;
using Enclave.Sdk.Api.Data.Organisations;
using Enclave.Sdk.Api.Data.Pagination;
using FluentAssertions;
using NUnit.Framework;
using System.Text.Json;
using WireMock.FluentAssertions;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Enclave.Sdk.Api.Tests.Clients;

public class EnrolmentKeyClientTests
{
    private EnrolmentKeysClient _enrolmentKeysClient;
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

        _enrolmentKeysClient = new EnrolmentKeysClient(httpClient, $"org/{organisationId}");
    }

    [Test]
    public async Task Should_return_paginated_response_when_calling_GetEnrolmentKeysAsync()
    {
        // Arrange
        var response = new PaginatedResponseModel<EnrolmentKeySummary>()
        {
            Items = new List<EnrolmentKeySummary>
            {
                new EnrolmentKeySummary(),
            },            
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/enrolment-keys").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        // Act
        var result = await _enrolmentKeysClient.GetEnrolmentKeysAsync();

        // Assert
        result.Should().NotBe(null);
        result.Items.Should().HaveCount(1);

    }

    [Test]
    public async Task Should_make_call_to_api_with_search_queryString_when_calling_GetEnrolmentKeysAsync()
    {
        // Arrange
        var response = new PaginatedResponseModel<EnrolmentKeySummary>()
        {
            Items = new List<EnrolmentKeySummary>
            {
                new EnrolmentKeySummary(),
            },
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/enrolment-keys").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        var searchTerm = "term";

        // Act
        await _enrolmentKeysClient.GetEnrolmentKeysAsync(searchTerm: searchTerm);

        _server.Should().HaveReceivedACall().AtAbsoluteUrl($"{_server.Urls[0]}{_orgRoute}/enrolment-keys?search={searchTerm}");
    }


    [Test]
    public async Task Should_make_call_to_api_with_include_disabled_queryString_when_calling_GetEnrolmentKeysAsync()
    {
        // Arrange
        var response = new PaginatedResponseModel<EnrolmentKeySummary>()
        {
            Items = new List<EnrolmentKeySummary>
            {
                new EnrolmentKeySummary(),
            },
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/enrolment-keys").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        var includeDisabled = true;

        // Act
        await _enrolmentKeysClient.GetEnrolmentKeysAsync(includeDisabled: includeDisabled);

        // Assert
        _server.Should().HaveReceivedACall().AtAbsoluteUrl($"{_server.Urls[0]}{_orgRoute}/enrolment-keys?include_disabled={includeDisabled}");
    }

    [Test]
    public async Task Should_make_call_to_api_with_sort_queryString_when_calling_GetEnrolmentKeysAsync()
    {
        // Arrange
        var response = new PaginatedResponseModel<EnrolmentKeySummary>()
        {
            Items = new List<EnrolmentKeySummary>
            {
                new EnrolmentKeySummary(),
            },
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/enrolment-keys").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        var sort = EnrolmentKeySortOrder.UsesRemaining;

        // Act
        await _enrolmentKeysClient.GetEnrolmentKeysAsync(sortOrder: sort);

        // Assert
        _server.Should().HaveReceivedACall().AtAbsoluteUrl($"{_server.Urls[0]}{_orgRoute}/enrolment-keys?sort={sort}");
    }

    [Test]
    public async Task Should_make_call_to_api_with_page_queryString_when_calling_GetEnrolmentKeysAsync()
    {
        // Arrange
        var response = new PaginatedResponseModel<EnrolmentKeySummary>()
        {
            Items = new List<EnrolmentKeySummary>
            {
                new EnrolmentKeySummary(),
            },
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/enrolment-keys").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        var page = 2;

        // Act
        await _enrolmentKeysClient.GetEnrolmentKeysAsync(pageNumber: page);

        // Assert
        _server.Should().HaveReceivedACall().AtAbsoluteUrl($"{_server.Urls[0]}{_orgRoute}/enrolment-keys?page={page}");
    }

    [Test]
    public async Task Should_make_call_to_api_with_per_page_queryString_when_calling_GetEnrolmentKeysAsync()
    {
        // Arrange
        var response = new PaginatedResponseModel<EnrolmentKeySummary>()
        {
            Items = new List<EnrolmentKeySummary>
            {
                new EnrolmentKeySummary(),
            },
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/enrolment-keys").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        var perPage = 2;

        // Act
        await _enrolmentKeysClient.GetEnrolmentKeysAsync(perPage: perPage);

        // Assert
        _server.Should().HaveReceivedACall().AtAbsoluteUrl($"{_server.Urls[0]}{_orgRoute}/enrolment-keys?per_page={perPage}");
    }

    [Test]
    public async Task Should_return_a_full_enrolment_key_model_when_calling_CreateAsync()
    {
        // Arrange
        var response = new EnrolmentKey();

        var createModel = new EnrolmentKeyCreate();

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/enrolment-keys").UsingPost())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));



        // Act
        var result = await _enrolmentKeysClient.CreateAsync(createModel);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public async Task Should_return_a_full_enrolment_key_model_when_calling_GetAsync()
    {
        // Arrange
        var response = new EnrolmentKey();

        var enrolmentKeyId = EnrolmentKeyId.FromInt(12);

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/enrolment-keys/{enrolmentKeyId}").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));



        // Act
        var result = await _enrolmentKeysClient.GetAsync(enrolmentKeyId);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public async Task Should_return_a_full_enrolment_key_model_when_calling_UpdateAsync()
    {
        // Arrange
        var response = new EnrolmentKey();

        var enrolmentKeyId = EnrolmentKeyId.FromInt(12);

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/enrolment-keys/{enrolmentKeyId}").UsingPatch())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));


        // Act
        var result = await _enrolmentKeysClient.Update(enrolmentKeyId).Set(e => e.Description, "New Value").ApplyAsync();

        // Assert
        result.Should().NotBeNull();
   }

    [Test]
    public async Task Should_return_a_full_enrolment_key_model_when_calling_EnableAsync()
    {
        // Arrange
        var response = new EnrolmentKey();

        var enrolmentKeyId = EnrolmentKeyId.FromInt(12);

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/enrolment-keys/{enrolmentKeyId}/enable").UsingPut())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        // Act
        var result = await _enrolmentKeysClient.EnableAsync(enrolmentKeyId);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public async Task Should_return_a_full_enrolment_key_model_when_calling_DisableAsync()
    {
        // Arrange
        var response = new EnrolmentKey();

        var enrolmentKeyId = EnrolmentKeyId.FromInt(12);

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/enrolment-keys/{enrolmentKeyId}/disable").UsingPut())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        // Act
        var result = await _enrolmentKeysClient.DisableAsync(enrolmentKeyId);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public async Task Should_return_number_of_keys_modified_when_calling_BulkEnableAsync()
    {
        // Arrange
        var response = new BulkKeyActionResult()
        {
            KeysModified = 2,
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/enrolment-keys/enable").UsingPut())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        var keys = new EnrolmentKeyId[] { EnrolmentKeyId.FromInt(1), EnrolmentKeyId.FromInt(2) };

        // Act
        var result = await _enrolmentKeysClient.BulkEnableAsync(keys);

        // Assert
        result.Should().Be(keys.Length);
    }

    [Test]
    public async Task Should_return_number_of_keys_modified_when_calling_BulkDisableAsync()
    {
        // Arrange
        var response = new BulkKeyActionResult()
        {
            KeysModified = 2,
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/enrolment-keys/disable").UsingPut())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        var keys = new EnrolmentKeyId[] { EnrolmentKeyId.FromInt(1), EnrolmentKeyId.FromInt(2) };

        // Act
        var result = await _enrolmentKeysClient.BulkDisableAsync(keys);

        // Assert
        result.Should().Be(keys.Length);
    }
}
