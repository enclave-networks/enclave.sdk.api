using Enclave.Sdk.Api.Clients;
using Enclave.Sdk.Api.Data.Account;
using Enclave.Sdk.Api.Data.EnrolmentKeys;
using Enclave.Sdk.Api.Data.Pagination;
using FluentAssertions;
using NUnit.Framework;
using System.Text.Json;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.FluentAssertions;
using WireMock.Server;
using Enclave.Sdk.Api.Data.PatchModel;
using Enclave.Sdk.Api.Data.EnrolmentKeys.Enum;

namespace Enclave.Sdk.Api.Tests.Clients;

public class EnrolmentKeyClientTests
{
    private EnrolmentKeysClient _enrolmentKeysClient;
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

        var currentOrganisation = new AccountOrganisation
        {
            OrgId = OrganisationId.New(),
            OrgName = "TestName",
            Role = UserOrganisationRole.Admin,
        };

        _orgRoute = $"/org/{currentOrganisation.OrgId}";

        // Not sure if this is the best way of doing this
        var organisationClient = new OrganisationClient(httpClient, currentOrganisation);
        _enrolmentKeysClient = (EnrolmentKeysClient)organisationClient.EnrolmentKeys;
    }

    [Test]
    public async Task Should_return_paginated_response_when_calling_GetEnrolmentKeysAsync()
    {
        // Arrange
        var response = new PaginatedResponseModel<SimpleEnrolmentKey>()
        {
            Items = new List<SimpleEnrolmentKey>
            {
                new SimpleEnrolmentKey(),
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
        var response = new PaginatedResponseModel<SimpleEnrolmentKey>()
        {
            Items = new List<SimpleEnrolmentKey>
            {
                new SimpleEnrolmentKey(),
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
        var response = new PaginatedResponseModel<SimpleEnrolmentKey>()
        {
            Items = new List<SimpleEnrolmentKey>
            {
                new SimpleEnrolmentKey(),
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
        var response = new PaginatedResponseModel<SimpleEnrolmentKey>()
        {
            Items = new List<SimpleEnrolmentKey>
            {
                new SimpleEnrolmentKey(),
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
        var response = new PaginatedResponseModel<SimpleEnrolmentKey>()
        {
            Items = new List<SimpleEnrolmentKey>
            {
                new SimpleEnrolmentKey(),
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
        var response = new PaginatedResponseModel<SimpleEnrolmentKey>()
        {
            Items = new List<SimpleEnrolmentKey>
            {
                new SimpleEnrolmentKey(),
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
}
