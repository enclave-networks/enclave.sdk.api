using Enclave.Sdk.Api.Clients;
using NUnit.Framework;
using System.Text.Json;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.FluentAssertions;
using FluentAssertions;
using Enclave.Configuration.Data.Identifiers;
using Enclave.Api.Scaffolding.Pagination.Models;
using Enclave.Api.Modules.ActivityLogs.Logs.Models;
using Enclave.Api.Modules.SystemManagement.EnrolmentKeys.Models;
using Enclave.Api.Modules.SystemManagement.Tags.Models;
using Enclave.Configuration.Data.Modules.EnrolmentKeys.Enums;

namespace Enclave.Sdk.Api.Tests.Clients;

public class LogsClientTests
{
    private LogsClient _logsClient;
    private WireMockServer _server;
    private string _orgRoute;
    private readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private PaginatedResponseModel<LogEntryModel> _paginatedResponse;

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


        _logsClient = new LogsClient(httpClient, $"org/{organisationId}");

        _paginatedResponse = new(
                new(0, 0, 0, 0, 0),
                new(new Uri("http://enclave.io"), new Uri("http://enclave.io"), new Uri("http://enclave.io"), new Uri("http://enclave.io")),
                new List<LogEntryModel>
                {
                    new LogEntryModel(),
                }.ToAsyncEnumerable());
    }

    [Test]
    public async Task Should_return_a_list_of_logs_in_pagination_format()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/logs").UsingGet())
          .RespondWith(
            Response.Create()
              .WithStatusCode(200)
              .WithBody(await _paginatedResponse.ToJsonAsync(_serializerOptions)));

        // Act
        var result = await _logsClient.GetLogsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().NotBeNull();
    }

    [Test]
    public async Task Should_make_call_to_api_with_page_queryString()
    {
        // Arrange
        var pageNumber = 1;

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/logs").UsingGet())
          .RespondWith(
            Response.Create()
              .WithStatusCode(200)
              .WithBody(await _paginatedResponse.ToJsonAsync(_serializerOptions)));

        // Act
        var result = await _logsClient.GetLogsAsync(pageNumber: pageNumber);

        // Assert
        _server.Should().HaveReceivedACall().AtAbsoluteUrl($"{_server.Urls[0]}{_orgRoute}/logs?page={pageNumber}");
    }

    [Test]
    public async Task Should_make_call_to_api_with_per_page_queryString()
    {
        // Arrange
        var perPage = 1;

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/logs").UsingGet())
          .RespondWith(
            Response.Create()
              .WithStatusCode(200)
              .WithBody(await _paginatedResponse.ToJsonAsync(_serializerOptions)));

        // Act
        var result = await _logsClient.GetLogsAsync(perPage: perPage);

        // Assert
        _server.Should().HaveReceivedACall().AtAbsoluteUrl($"{_server.Urls[0]}{_orgRoute}/logs?per_page={perPage}");
    }
}
