using System.Net;
using System.Text.Json;
using Enclave.Api.Modules.SystemManagement.Systems.Models;
using Enclave.Api.Modules.SystemManagement.Tags.Models;
using Enclave.Api.Modules.SystemManagement.TrustRequirements.Models;
using Enclave.Api.Scaffolding.Pagination.Models;
using Enclave.Configuration.Data.Identifiers;
using Enclave.Configuration.Data.Modules.Systems.Enums;
using Enclave.Configuration.Data.Modules.Tags.Enums;
using Enclave.Sdk.Api.Clients;
using Enclave.Sdk.Api.Clients.Interfaces;
using Enclave.Sdk.Api.Data;
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
                    new("tag1", TagRefId.FromString("tag1"), null, null, 0, 0, 0, 0, 0),
                    new("tag2", TagRefId.FromString("tag2"), null, null, 0, 0, 0, 0, 0),
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

    // GET tags/meta/search-keys answers with the keys the search term of GetAsync accepts (portal
    // TagsController.GetSearchKeyMetadata). The keys are the tags' search keys (portal TagSearchKeyService.cs)
    // as the API writes them, with enums as names.
    [Test]
    public async Task Should_return_the_search_keys_when_calling_GetSearchKeysAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/tags/meta/search-keys").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody("""
                [
                  {
                    "name": "tag",
                    "modifiers": null,
                    "dataType": "None",
                    "description": "Filter your search by a partial tag name.",
                    "hintText": "Filter by tag name",
                    "hintValues": null,
                    "canHaveMultiple": false,
                    "isDefault": true,
                    "useExactMatch": false
                  },
                  {
                    "name": "uses",
                    "modifiers": [ "LessThan", "GreaterThan" ],
                    "dataType": "None",
                    "description": "Filter your search by the number of places a tag is used, select either less than, greater than or input a value.",
                    "hintText": "Filter by the number of places a tag is used",
                    "hintValues": null,
                    "canHaveMultiple": false,
                    "isDefault": false,
                    "useExactMatch": false
                  }
                ]
                """));

        // Act
        var result = await _tagsClient.GetSearchKeysAsync();

        // Assert
        result.Should().BeEquivalentTo(
            new[]
            {
                new SearchKey
                {
                    Name = "tag",
                    DataType = SearchKeyDataType.None,
                    Description = "Filter your search by a partial tag name.",
                    HintText = "Filter by tag name",
                    IsDefault = true,
                },
                new SearchKey
                {
                    Name = "uses",
                    Modifiers = new() { SearchModifier.LessThan, SearchModifier.GreaterThan },
                    DataType = SearchKeyDataType.None,
                    Description = "Filter your search by the number of places a tag is used, select either less than, greater than or input a value.",
                    HintText = "Filter by the number of places a tag is used",
                },
            },
            options => options.WithStrictOrdering());
    }

    private static readonly (string Name, string Method, Func<ITagsClient, string, Task> Call)[] TagCalls =
    {
        ("GetAsync(string)", "GET", (client, tag) => client.GetAsync(tag)),
        ("Update(string)", "PATCH", (client, tag) => client.Update(tag).Set(t => t.Notes, "notes").ApplyAsync()),
        ("DeleteAsync(string)", "DELETE", (client, tag) => client.DeleteAsync(tag)),
    };

    // The TagRefId overloads put the ref's text into the same path segment.
    private static readonly (string Name, string Method, Func<ITagsClient, string, Task> Call)[] TagRefIdCalls =
    {
        ("GetAsync(TagRefId)", "GET", (client, tag) => client.GetAsync(TagRefId.FromString(tag))),
        ("Update(TagRefId)", "PATCH", (client, tag) => client.Update(TagRefId.FromString(tag)).Set(t => t.Notes, "notes").ApplyAsync()),
        ("DeleteAsync(TagRefId)", "DELETE", (client, tag) => client.DeleteAsync(TagRefId.FromString(tag))),
    };

    private static IEnumerable<TestCaseData> TagEscapingCases() =>
        TagCalls.Concat(TagRefIdCalls).Select(c => new TestCaseData(c.Method, c.Call).SetArgDisplayNames(c.Name));

    private static IEnumerable<TestCaseData> RejectedTagCases() =>
        from c in TagCalls
        from tag in new[] { null, "", ".", ".." }
        select new TestCaseData(c.Call, tag).SetArgDisplayNames(c.Name, tag is null ? "null" : $"\"{tag}\"");

    // A tag name is one segment of the URL path. .NET removes ".." segments when it combines a relative
    // path with the base address (RFC 3986 section 5.2.4), so "../policies/1" left unescaped would send
    // DeleteAsync's DELETE to org/<id>/policies/1, which deletes policy 1. Escaped, the whole value stays
    // one segment under tags, where it names no tag. The request is checked as it left the client, where
    // the escaping is visible, and the server is checked for any request that reached the policies route.
    [TestCaseSource(nameof(TagEscapingCases))]
    public async Task Should_send_a_tag_as_one_escaped_path_segment(string method, Func<ITagsClient, string, Task> call)
    {
        // Arrange
        using var server = WireMockServer.Start();
        server
          .Given(Request.Create().WithPath("/*").UsingAnyMethod())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _tagResponse.ToJsonAsync(_serializerOptions)));

        var organisationId = OrganisationGuid.New();
        var recorder = new RecordingHttpMessageHandler(new HttpClientHandler());
        var client = new TagsClient(new HttpClient(recorder) { BaseAddress = new Uri(server.Urls[0]) }, $"org/{organisationId}");

        // Act
        await call(client, "../policies/1");

        // Assert
        var request = recorder.Requests.Should().ContainSingle().Subject;
        request.Method.Method.Should().Be(method);
        request.Uri.AbsolutePath.Should().Be($"/org/{organisationId}/tags/..%2Fpolicies%2F1");
        server.LogEntries.Should().ContainSingle()
            .Which.RequestMessage.Path.Should().NotStartWith($"/org/{organisationId}/policies");
    }

    // Escaping leaves "." and ".." as they are, and .NET resolves them as dot-segments, so DeleteAsync("..")
    // would DELETE org/<id>. An empty tag drops the segment and addresses the tag list, where DELETE is the
    // bulk delete. None of these names a tag, so each is refused before a request is sent.
    [TestCaseSource(nameof(RejectedTagCases))]
    public async Task Should_refuse_a_tag_that_is_not_a_path_segment_without_sending_a_request(Func<ITagsClient, string, Task> call, string tag)
    {
        // Arrange
        var recorder = new RecordingHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));
        var client = new TagsClient(new HttpClient(recorder) { BaseAddress = new Uri("http://localhost/") }, $"org/{OrganisationGuid.New()}");

        // Act
        var act = () => call(client, tag);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>().WithParameterName("tag");
        recorder.Requests.Should().BeEmpty();
    }
}