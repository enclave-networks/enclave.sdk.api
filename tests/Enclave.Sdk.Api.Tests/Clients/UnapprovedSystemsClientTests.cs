using Enclave.Sdk.Api.Clients;
using FluentAssertions;
using NUnit.Framework;
using System.Text.Json;
using WireMock.Server;
using WireMock.FluentAssertions;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using Enclave.Configuration.Data.Identifiers;
using Enclave.Api.Scaffolding.Pagination.Models;
using Enclave.Api.Modules.SystemManagement.Systems.Models;
using Enclave.Api.Modules.SystemManagement.Tags.Models;
using Enclave.Configuration.Data.Modules.Systems.Enums;
using Enclave.Api.Modules.SystemManagement.UnapprovedSystems.Models;

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


    private PaginatedResponseModel<UnapprovedSystemSummaryModel> _paginatedResponse;
    private UnapprovedSystemModel _unapprovedSystemResponse;

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

        _paginatedResponse = new(
                new(0, 0, 0, 0, 0),
                new(new Uri("http://enclave.io"), new Uri("http://enclave.io"), new Uri("http://enclave.io"), new Uri("http://enclave.io")),
                new List<UnapprovedSystemSummaryModel>
                {
                    new("1",
                        SystemType.GeneralPurpose,
                        "system",
                        string.Empty,
                        DateTime.Now,
                        EnrolmentKeyId.FromInt(1),
                        string.Empty,
                        Array.Empty<IUsedTagModel>())
                }.ToAsyncEnumerable());

        _unapprovedSystemResponse = new UnapprovedSystemModel("1",
                        SystemType.GeneralPurpose,
                        "system",
                        string.Empty,
                        DateTime.Now,
                        EnrolmentKeyId.FromInt(1),
                        string.Empty,
                        false,
                        Array.Empty<IUsedTagModel>(),
                        null);
    }

    [Test]
    public async Task Should_return_a_paginated_response_model_when_calling_GetSystemsAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/unapproved-systems").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _paginatedResponse.ToJsonAsync(_serializerOptions)));

        // Act
        var result = await _unapprovedSystemsClient.GetSystemsAsync();

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public async Task Should_make_a_call_to_api_with_enrolment_key_quertString_when_calling_GetSystemsAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/unapproved-systems").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _paginatedResponse.ToJsonAsync(_serializerOptions)));

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
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/unapproved-systems").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _paginatedResponse.ToJsonAsync(_serializerOptions)));

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
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/unapproved-systems").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _paginatedResponse.ToJsonAsync(_serializerOptions)));

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
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/unapproved-systems").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _paginatedResponse.ToJsonAsync(_serializerOptions)));

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
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/unapproved-systems").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _paginatedResponse.ToJsonAsync(_serializerOptions)));

        var page = 12;

        // Act
        await _unapprovedSystemsClient.GetSystemsAsync(perPage: page);

        // Assert
        _server.Should().HaveReceivedACall().AtAbsoluteUrl($"{_server.Urls[0]}{_orgRoute}/unapproved-systems?per_page={page}");
    }

    [Test]
    public async Task Should_return_number_of_declined_systems_when_calling_DeclineSystems()
    {
        var response = new BulkUnapprovedSystemDeclineResult(2);

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/unapproved-systems").UsingDelete())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await response.ToJsonAsync(_serializerOptions)));

        // Act
        var result = await _unapprovedSystemsClient.DeclineSystems("system1", "system2");

        // Assert
        result.Should().Be(2);
    }

    [Test]
    public async Task Should_return_unapproved_system_detail_model_when_calling_GetAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/unapproved-systems/{_unapprovedSystemResponse.SystemId}").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _unapprovedSystemResponse.ToJsonAsync(_serializerOptions)));

        // Act
        var result = await _unapprovedSystemsClient.GetAsync("newId");

        // Assert
        result.Should().NotBeNull();
        result.SystemId.Should().Be(_unapprovedSystemResponse.SystemId);
    }

    [Test]
    public async Task Should_return_unapproved_system_detail_model_when_calling_UpdateAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/unapproved-systems/{_unapprovedSystemResponse.SystemId}").UsingPatch())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _unapprovedSystemResponse.ToJsonAsync(_serializerOptions)));

        // Act
        var result = await _unapprovedSystemsClient.Update("newId").Set(u => u.Description, "New System").ApplyAsync();

        // Assert
        result.Should().NotBeNull();
        result.SystemId.Should().Be(_unapprovedSystemResponse.SystemId);
    }

    [Test]
    public async Task Should_return_unapproved_system_detail_model_when_calling_DeclineAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/unapproved-systems/{_unapprovedSystemResponse.SystemId}").UsingDelete())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _unapprovedSystemResponse.ToJsonAsync(_serializerOptions)));

        // Act
        var result = await _unapprovedSystemsClient.DeclineAsync("newId");

        // Assert
        result.Should().NotBeNull();
        result.SystemId.Should().Be(_unapprovedSystemResponse.SystemId);
    }

    [Test]
    public async Task Should_not_throw_an_error_when_calling_ApproveAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/unapproved-systems/{_unapprovedSystemResponse.SystemId}/approve").UsingPut())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _unapprovedSystemResponse.ToJsonAsync(_serializerOptions)));

        // Act
        await _unapprovedSystemsClient.ApproveAsync("newId");
    }

    [Test]
    public async Task Should_return_number_of_approved_systems_when_calling_ApproveSystemsAsync()
    {
        var response = new BulkUnapprovedSystemApproveResult(2);

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/unapproved-systems/approve").UsingPut())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await response.ToJsonAsync(_serializerOptions)));

        // Act
        var result = await _unapprovedSystemsClient.ApproveSystemsAsync("system1" , "system2");

        // Assert
        result.Should().Be(2);
    }
}