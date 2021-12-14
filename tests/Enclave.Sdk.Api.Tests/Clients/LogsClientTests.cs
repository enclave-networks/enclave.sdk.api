using Enclave.Sdk.Api.Clients;
using Enclave.Sdk.Api.Data.Logging;
using Enclave.Sdk.Api.Data.Organisations;
using Enclave.Sdk.Api.Data.Pagination;
using NUnit.Framework;
using System.Text.Json;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.FluentAssertions;
using FluentAssertions;

namespace Enclave.Sdk.Api.Tests.Clients;

public class LogsClientTests
{
    private LogsClient _logsClient;
    private WireMockServer _server;
    private string _orgRoute;
    private JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
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


        _logsClient = new LogsClient(httpClient, $"org/{organisationId}");
    }

    [Test]
    public async Task Should_return_a_list_of_logs_in_pagination_format()
    {
        // Arrange
        var responseModel = new PaginatedResponseModel<LogEntry>
        {
            Items = new List<LogEntry>
            {
                new LogEntry { },
                new LogEntry { },
            },
            Links = new PaginationLinks(),
            Metadata = new PaginationMetadata(),
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/logs").UsingGet())
          .RespondWith(
            Response.Create()
              .WithStatusCode(200)
              .WithBody(JsonSerializer.Serialize(responseModel, _serializerOptions)));

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

        var responseModel = new PaginatedResponseModel<LogEntry>
        {
            Items = new List<LogEntry>(),
            Links = new PaginationLinks(),
            Metadata = new PaginationMetadata(),
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/logs").UsingGet())
          .RespondWith(
            Response.Create()
              .WithStatusCode(200)
              .WithBody(JsonSerializer.Serialize(responseModel, _serializerOptions)));

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

        var responseModel = new PaginatedResponseModel<LogEntry>
        {
            Items = new List<LogEntry>(),
            Links = new PaginationLinks(),
            Metadata = new PaginationMetadata(),
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/logs").UsingGet())
          .RespondWith(
            Response.Create()
              .WithStatusCode(200)
              .WithBody(JsonSerializer.Serialize(responseModel, _serializerOptions)));

        // Act
        var result = await _logsClient.GetLogsAsync(perPage: perPage);

        // Assert
        _server.Should().HaveReceivedACall().AtAbsoluteUrl($"{_server.Urls[0]}{_orgRoute}/logs?per_page={perPage}");
    }
}
