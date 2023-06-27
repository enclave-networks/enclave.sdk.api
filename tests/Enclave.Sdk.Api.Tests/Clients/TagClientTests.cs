using System.Text.Json;
using Enclave.Api.Modules.SystemManagement.Systems.Models;
using Enclave.Api.Modules.SystemManagement.Tags.Models;
using Enclave.Api.Modules.SystemManagement.TrustRequirements.Models;
using Enclave.Api.Scaffolding.Pagination.Models;
using Enclave.Configuration.Data.Identifiers;
using Enclave.Configuration.Data.Modules.Systems.Enums;
using Enclave.Configuration.Data.Modules.Tags.Enums;
using Enclave.Sdk.Api.Clients;
using FluentAssertions;
using NUnit.Framework;
using WireMock.FluentAssertions;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Enclave.Sdk.Api.Tests.Clients;

public class TagClientTests
{
    private TagsClient _tagsClient;
    private WireMockServer _server;
    private string _orgRoute;
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private PaginatedResponseModel<TagSummaryModel> _paginatedResponse;
    private TagModel _tagResponse;

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


        _tagsClient = new TagsClient(httpClient, $"org/{organisationId}");

        _paginatedResponse = new(
                new(0, 0, 0, 0, 0),
                new(new Uri("http://enclave.io"), new Uri("http://enclave.io"), new Uri("http://enclave.io"), new Uri("http://enclave.io")),
                new List<TagSummaryModel>
                {
                    new("tag1", TagRefId.FromString("tag1"), null, null, 0, 0, 0, 0),
                    new("tag2", TagRefId.FromString("tag2"), null, null, 0, 0, 0, 0),
                }.ToAsyncEnumerable());

        _tagResponse = new(
            "tag1",
            TagRefId.FromString("tag1"),
            null,
            DateTime.Now,
            DateTime.Now,
            null,
            0,
            0,
            0,
            0,
            null,
            Array.Empty<IUsedTrustRequirementModel>());
    }

    [Test]
    public async Task Should_return_a_list_of_tags_in_pagination_format()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/tags").UsingGet())
          .RespondWith(
            Response.Create()
              .WithStatusCode(200)
              .WithBody(await _paginatedResponse.ToJsonAsync(_serializerOptions)));

        // Act
        var result = await _tagsClient.GetAsync();

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().NotBeNull();
    }

    [Test]
    public async Task Should_make_call_to_api_with_search_queryString()
    {
        // Arrange
        var searchTerm = "test";

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/tags").UsingGet())
          .RespondWith(
            Response.Create()
              .WithStatusCode(200)
              .WithBody(await _paginatedResponse.ToJsonAsync(_serializerOptions)));

        // Act
        await _tagsClient.GetAsync(searchTerm: searchTerm);

        // Assert
        _server.Should().HaveReceivedACall().AtAbsoluteUrl($"{_server.Urls[0]}{_orgRoute}/tags?search={searchTerm}");
    }

    [Test]
    public async Task Should_make_call_to_api_with_sort_queryString()
    {
        // Arrange
        var sortEnum = TagQuerySortOrder.Alphabetical;

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/tags").UsingGet())
          .RespondWith(
            Response.Create()
              .WithStatusCode(200)
              .WithBody(await _paginatedResponse.ToJsonAsync(_serializerOptions)));

        // Act
        await _tagsClient.GetAsync(sortOrder: sortEnum);

        // Assert
        _server.Should().HaveReceivedACall().AtAbsoluteUrl($"{_server.Urls[0]}{_orgRoute}/tags?sort={sortEnum}");
    }

    [Test]
    public async Task Should_make_call_to_api_with_page_queryString()
    {
        // Arrange
        var pageNumber = 1;

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/tags").UsingGet())
          .RespondWith(
            Response.Create()
              .WithStatusCode(200)
              .WithBody(await _paginatedResponse.ToJsonAsync(_serializerOptions)));

        // Act
        await _tagsClient.GetAsync(pageNumber: pageNumber);

        // Assert
        _server.Should().HaveReceivedACall().AtAbsoluteUrl($"{_server.Urls[0]}{_orgRoute}/tags?page={pageNumber}");
    }

    [Test]
    public async Task Should_make_call_to_api_with_per_page_queryString()
    {
        // Arrange
        var perPage = 1;

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/tags").UsingGet())
          .RespondWith(
            Response.Create()
              .WithStatusCode(200)
              .WithBody(await _paginatedResponse.ToJsonAsync(_serializerOptions)));

        // Act
        await _tagsClient.GetAsync(perPage: perPage);

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

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/tags").UsingGet())
          .RespondWith(
            Response.Create()
              .WithStatusCode(200)
              .WithBody(await _paginatedResponse.ToJsonAsync(_serializerOptions)));

        // Act
        await _tagsClient.GetAsync(searchTerm: searchTerm, sortOrder: sortEnum, pageNumber: pageNumber, perPage: perPage);

        // Assert
        _server.Should().HaveReceivedACall()
            .AtAbsoluteUrl($"{_server.Urls[0]}{_orgRoute}/tags?search={searchTerm}&sort={sortEnum}&page={pageNumber}&per_page={perPage}");
    }

    [Test]
    public async Task Should_return_a_detailed_tag_model_when_calling_CreateAsync()
    {
        // Arrange
        var createModel = new TagCreateModel();

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/tags").UsingPost())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _tagResponse.ToJsonAsync(_serializerOptions)));



        // Act
        var result = await _tagsClient.CreateAsync(createModel);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public async Task Should_return_a_detailed_tag_model_when_calling_GetAsync()
    {
        // Arrange
        var tagRefId = TagRefId.FromString("tagref");

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/tags/{tagRefId}").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _tagResponse.ToJsonAsync(_serializerOptions)));



        // Act
        var result = await _tagsClient.GetAsync(tagRefId);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public async Task Should_return_a_detailed_tag_model_when_calling_UpdateAsync()
    {
        // Arrange
        var tagRefId = TagRefId.FromString("tagref");

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/tags/{tagRefId}").UsingPatch())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _tagResponse.ToJsonAsync(_serializerOptions)));


        // Act
        var result = await _tagsClient.Update(tagRefId).Set(e => e.Notes, "New Value").ApplyAsync();

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public async Task Should_return_a_detailed_tag_model_when_calling_DeleteAsync()
    {
        // Arrange
        var tagRefId = TagRefId.FromString("tagref");

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/tags/{tagRefId}").UsingDelete())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _tagResponse.ToJsonAsync(_serializerOptions)));

        // Act
        var result = await _tagsClient.DeleteAsync(tagRefId);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public async Task Should_return_number_of_keys_modified_when_calling_DeleteTagsAsync()
    {
        // Arrange
        var response = new BulkTagDeleteResult(2);

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/tags").UsingDelete())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await response.ToJsonAsync(_serializerOptions)));

        var tagRefs = new string[] { "tagref1", "tagref2" };

        // Act
        var result = await _tagsClient.DeleteTagsAsync(tagRefs);

        // Assert
        result.Should().Be(tagRefs.Length);
    }
}