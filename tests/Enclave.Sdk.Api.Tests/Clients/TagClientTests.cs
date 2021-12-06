using System.Text.Json;
using Enclave.Sdk.Api.Clients;
using Enclave.Sdk.Api.Data.Organisations;
using Enclave.Sdk.Api.Data.Pagination;
using Enclave.Sdk.Api.Data.Tags;
using FluentAssertions;
using NUnit.Framework;
using WireMock.FluentAssertions;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Enclave.Sdk.Api.Tests.Clients;

public class TagClientTests
{
    private TagsClient _tagClient;
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


        _tagClient = new TagsClient(httpClient, $"org/{organisationId}");
    }

    [Test]
    public async Task Should_return_a_list_of_tags_in_pagination_format()
    {
        // Arrange
        var responseModel = new PaginatedResponseModel<TagItem>
        {
            Items = new List<TagItem>
            {
                new TagItem { Tag = "tag1", Keys = 12, DnsRecords = 1, Policies = 0, Systems = 3 },
                new TagItem { Tag = "tag2", Keys = 13, DnsRecords = 0, Policies = 43, Systems = 0 },
            },
            Links = new PaginationLinks(),
            Metadata = new PaginationMetadata(),
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/tags").UsingGet())
          .RespondWith(
            Response.Create()
              .WithStatusCode(200)
              .WithBody(JsonSerializer.Serialize(responseModel, _serializerOptions)));

        // Act
        var result = await _tagClient.GetAsync();

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().NotBeNull();
    }

    [Test]
    public async Task Should_make_call_to_api_with_search_queryString()
    {
        // Arrange
        var searchTerm = "test";

        var responseModel = new PaginatedResponseModel<TagItem>
        {
            Items = new List<TagItem>(),
            Links = new PaginationLinks(),
            Metadata = new PaginationMetadata(),
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/tags").UsingGet())
          .RespondWith(
            Response.Create()
              .WithStatusCode(200)
              .WithBody(JsonSerializer.Serialize(responseModel, _serializerOptions)));

        // Act
        var result = await _tagClient.GetAsync(searchTerm: searchTerm);

        // Assert
        _server.Should().HaveReceivedACall().AtAbsoluteUrl($"{_server.Urls[0]}{_orgRoute}/tags?search={searchTerm}");
    }

    [Test]
    public async Task Should_make_call_to_api_with_sort_queryString()
    {
        // Arrange
        var sortEnum = TagQuerySortOrder.Alphabetical;

        var responseModel = new PaginatedResponseModel<TagItem>
        {
            Items = new List<TagItem>(),
            Links = new PaginationLinks(),
            Metadata = new PaginationMetadata(),
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/tags").UsingGet())
          .RespondWith(
            Response.Create()
              .WithStatusCode(200)
              .WithBody(JsonSerializer.Serialize(responseModel, _serializerOptions)));

        // Act
        var result = await _tagClient.GetAsync(sortOrder: sortEnum);

        // Assert
        _server.Should().HaveReceivedACall().AtAbsoluteUrl($"{_server.Urls[0]}{_orgRoute}/tags?sort={sortEnum}");
    }

    [Test]
    public async Task Should_make_call_to_api_with_page_queryString()
    {
        // Arrange
        var pageNumber = 1;

        var responseModel = new PaginatedResponseModel<TagItem>
        {
            Items = new List<TagItem>(),
            Links = new PaginationLinks(),
            Metadata = new PaginationMetadata(),
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/tags").UsingGet())
          .RespondWith(
            Response.Create()
              .WithStatusCode(200)
              .WithBody(JsonSerializer.Serialize(responseModel, _serializerOptions)));

        // Act
        var result = await _tagClient.GetAsync(pageNumber: pageNumber);

        // Assert
        _server.Should().HaveReceivedACall().AtAbsoluteUrl($"{_server.Urls[0]}{_orgRoute}/tags?page={pageNumber}");
    }

    [Test]
    public async Task Should_make_call_to_api_with_per_page_queryString()
    {
        // Arrange
        var perPage = 1;

        var responseModel = new PaginatedResponseModel<TagItem>
        {
            Items = new List<TagItem>(),
            Links = new PaginationLinks(),
            Metadata = new PaginationMetadata(),
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/tags").UsingGet())
          .RespondWith(
            Response.Create()
              .WithStatusCode(200)
              .WithBody(JsonSerializer.Serialize(responseModel, _serializerOptions)));

        // Act
        var result = await _tagClient.GetAsync(perPage: perPage);

        // Assert
        _server.Should().HaveReceivedACall().AtAbsoluteUrl($"{_server.Urls[0]}{_orgRoute}/tags?per_page={perPage}");
    }

    [Test]
    public async Task Should_make_call_to_api_with_all_queryStrings()
    {
        // Arrange
        var searchTerm = "test";
        var sortEnum = TagQuerySortOrder.Alphabetical;
        var perPage = 1;
        var pageNumber = 1;

        var responseModel = new PaginatedResponseModel<TagItem>
        {
            Items = new List<TagItem>(),
            Links = new PaginationLinks(),
            Metadata = new PaginationMetadata(),
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/tags").UsingGet())
          .RespondWith(
            Response.Create()
              .WithStatusCode(200)
              .WithBody(JsonSerializer.Serialize(responseModel, _serializerOptions)));

        // Act
        var result = await _tagClient.GetAsync(searchTerm: searchTerm, sortOrder: sortEnum, pageNumber: pageNumber, perPage: perPage);

        // Assert
        _server.Should().HaveReceivedACall()
            .AtAbsoluteUrl($"{_server.Urls[0]}{_orgRoute}/tags?search={searchTerm}&sort={sortEnum}&page={pageNumber}&per_page={perPage}");
    }
}