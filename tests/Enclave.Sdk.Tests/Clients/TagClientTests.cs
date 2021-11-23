using System.Text.Json;
using Enclave.Sdk.Api.Clients;
using Enclave.Sdk.Api.Data.Account;
using Enclave.Sdk.Api.Data.Pagination;
using Enclave.Sdk.Api.Data.Tags;
using FluentAssertions;
using NUnit.Framework;
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

        var orgId = "testId";

        _orgRoute = $"/org/{orgId}";

        _tagClient = new TagsClient(httpClient, _orgRoute);
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
          .Given(Request.Create().WithPath($"{_orgRoute}/tags").WithParam("search").UsingGet())
          .RespondWith(
            Response.Create()
              .WithStatusCode(200)
              .WithBody(JsonSerializer.Serialize(responseModel, _serializerOptions)));

        // Act
        var result = await _tagClient.GetAsync(searchTerm: "test");

        // Assert
        result.Should().NotBeNull();
    }
}