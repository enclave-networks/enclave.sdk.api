using Enclave.Api.Modules.SystemManagement.Dns.Models;
using Enclave.Api.Modules.SystemManagement.EnrolmentKeys;
using Enclave.Api.Modules.SystemManagement.EnrolmentKeys.Models;
using Enclave.Api.Modules.SystemManagement.Tags.Models;
using Enclave.Api.Scaffolding.Pagination.Models;
using Enclave.Configuration.Data.Identifiers;
using Enclave.Configuration.Data.Modules.EnrolmentKeys.Enums;
using Enclave.Sdk.Api.Clients;
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

    private PaginatedResponseModel<EnrolmentKeySummaryModel> _paginatedResponse;
    private EnrolmentKeyModel _enrolmentKeyResponse;

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

        _enrolmentKeysClient = new EnrolmentKeysClient(httpClient, $"org/{organisationId}");

        _paginatedResponse = new(
                new(0, 0, 0, 0, 0),
                new(new Uri("http://enclave.io"), new Uri("http://enclave.io"), new Uri("http://enclave.io"), new Uri("http://enclave.io")),
                new List<EnrolmentKeySummaryModel>
                {
                    new EnrolmentKeySummaryModel(EnrolmentKeyId.FromInt(1),
                    DateTime.Now,
                    DateTime.Now,
                    EnrolmentKeyType.GeneralPurpose,
                    ApprovalMode.Automatic,
                    string.Empty,
                    string.Empty,
                    true,
                    int.MaxValue,
                    1,
                    0,
                    Array.Empty<IUsedTagModel>(),
                    null,
                    null),
                }.ToAsyncEnumerable());

        _enrolmentKeyResponse = new(EnrolmentKeyId.FromInt(1),
                    DateTime.Now,
                    DateTime.Now,
                    EnrolmentKeyType.GeneralPurpose,
                    ApprovalMode.Automatic,
                    string.Empty,
                    string.Empty,
                    true,
                    int.MaxValue,
                    1,
                    0,
                    Array.Empty<IUsedTagModel>(),
                    null,
                    Array.Empty<EnrolmentKeyIpConstraintInputModel>(),
                    null,
                    null);
    }

    [Test]
    public async Task Should_return_paginated_response_when_calling_GetEnrolmentKeysAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/enrolment-keys").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _paginatedResponse.ToJsonAsync(_serializerOptions)));

        // Act
        var result = await _enrolmentKeysClient.GetEnrolmentKeysAsync();

        // Assert
        result.Should().NotBe(null);
        (await result.Items.ToListAsync()).Should().HaveCount(1);

    }

    [Test]
    public async Task Should_make_call_to_api_with_search_queryString_when_calling_GetEnrolmentKeysAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/enrolment-keys").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _paginatedResponse.ToJsonAsync(_serializerOptions)));

        var searchTerm = "term";

        // Act
        await _enrolmentKeysClient.GetEnrolmentKeysAsync(searchTerm: searchTerm);

        _server.Should().HaveReceivedACall().AtAbsoluteUrl($"{_server.Urls[0]}{_orgRoute}/enrolment-keys?search={searchTerm}");
    }


    [Test]
    public async Task Should_make_call_to_api_with_include_disabled_queryString_when_calling_GetEnrolmentKeysAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/enrolment-keys").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _paginatedResponse.ToJsonAsync(_serializerOptions)));

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
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/enrolment-keys").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _paginatedResponse.ToJsonAsync(_serializerOptions)));

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
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/enrolment-keys").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _paginatedResponse.ToJsonAsync(_serializerOptions)));

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
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/enrolment-keys").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _paginatedResponse.ToJsonAsync(_serializerOptions)));

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
        var createModel = new EnrolmentKeyCreateModel();

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/enrolment-keys").UsingPost())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _enrolmentKeyResponse.ToJsonAsync(_serializerOptions)));



        // Act
        var result = await _enrolmentKeysClient.CreateAsync(createModel);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public async Task Should_return_a_full_enrolment_key_model_when_calling_GetAsync()
    {
        // Arrange
        var enrolmentKeyId = EnrolmentKeyId.FromInt(12);

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/enrolment-keys/{enrolmentKeyId}").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _enrolmentKeyResponse.ToJsonAsync(_serializerOptions)));



        // Act
        var result = await _enrolmentKeysClient.GetAsync(enrolmentKeyId);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public async Task Should_return_a_full_enrolment_key_model_when_calling_UpdateAsync()
    {
        // Arrange
        var enrolmentKeyId = EnrolmentKeyId.FromInt(12);

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/enrolment-keys/{enrolmentKeyId}").UsingPatch())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _enrolmentKeyResponse.ToJsonAsync(_serializerOptions)));


        // Act
        var result = await _enrolmentKeysClient.Update(enrolmentKeyId).Set(e => e.Description, "New Value").ApplyAsync();

        // Assert
        result.Should().NotBeNull();
   }

    [Test]
    public async Task Should_return_a_full_enrolment_key_model_when_calling_EnableAsync()
    {
        // Arrange
        var enrolmentKeyId = EnrolmentKeyId.FromInt(12);

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/enrolment-keys/{enrolmentKeyId}/enable").UsingPut())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _enrolmentKeyResponse.ToJsonAsync(_serializerOptions)));

        // Act
        var result = await _enrolmentKeysClient.EnableAsync(enrolmentKeyId);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public async Task Should_return_a_full_enrolment_key_model_when_calling_DisableAsync()
    {
        // Arrange
        var enrolmentKeyId = EnrolmentKeyId.FromInt(12);

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/enrolment-keys/{enrolmentKeyId}/disable").UsingPut())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _enrolmentKeyResponse.ToJsonAsync(_serializerOptions)));

        // Act
        var result = await _enrolmentKeysClient.DisableAsync(enrolmentKeyId);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public async Task Should_return_number_of_keys_modified_when_calling_BulkEnableAsync()
    {
        // Arrange
        var response = new BulkKeyActionResult(2);
        
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/enrolment-keys/enable").UsingPut())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await response.ToJsonAsync(_serializerOptions)));

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
        var response = new BulkKeyActionResult(2);

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/enrolment-keys/disable").UsingPut())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await response.ToJsonAsync(_serializerOptions)));

        var keys = new EnrolmentKeyId[] { EnrolmentKeyId.FromInt(1), EnrolmentKeyId.FromInt(2) };

        // Act
        var result = await _enrolmentKeysClient.BulkDisableAsync(keys);

        // Assert
        result.Should().Be(keys.Length);
    }
}
