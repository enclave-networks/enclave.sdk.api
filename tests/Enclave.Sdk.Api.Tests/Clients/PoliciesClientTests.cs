using Enclave.Sdk.Api.Clients;
using Enclave.Sdk.Api.Data;
using FluentAssertions;
using NUnit.Framework;
using System.Text.Json;
using System.Text.Json.Nodes;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.FluentAssertions;
using Enclave.Configuration.Data.Identifiers;
using Enclave.Api.Modules.SystemManagement.EnrolmentKeys.Models;
using Enclave.Api.Modules.SystemManagement.Tags.Models;
using Enclave.Configuration.Data.Modules.EnrolmentKeys.Enums;
using Enclave.Api.Modules.SystemManagement.Policies.Models;
using Enclave.Api.Scaffolding.Pagination.Models;
using Enclave.Configuration.Data.Modules.Policies.Models;
using Enclave.Configuration.Data.Modules.Policies.Enums;
using Enclave.Api.Modules.SystemManagement.TrustRequirements.Models;
using Enclave.Sdk.Network.NetworkPolicy;

namespace Enclave.Sdk.Api.Tests.Clients;

public class PoliciesClientTests
{
    private PoliciesClient _policiesClient;
    private WireMockServer _server;
    private string _orgRoute;
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private PaginatedResponseModel<PolicySummaryModel> _paginatedResponse;
    private PolicyModel _policyResponse;

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

        _policiesClient = new PoliciesClient(httpClient, $"org/{organisationId}");

        _paginatedResponse = new(
        new(0, 0, 0, 0, 0),
        new(new Uri("http://enclave.io"), new Uri("http://enclave.io"), new Uri("http://enclave.io"), new Uri("http://enclave.io")),
        new List<PolicySummaryModel>
        {
                    new PolicySummaryModel(PolicyId.FromInt(1),
                    DateTime.Now,
                    PolicyType.General,
                    "policy",
                    true,
                    PolicyState.Active,
                    Array.Empty<IUsedTagModel>(),
                    Array.Empty<IUsedTagModel>(),
                    Array.Empty<PolicyAclModel>(),
                    Array.Empty<PolicyGatewayAllowedIpRange>(),
                    Array.Empty<PolicyGatewayDetailModel>(),
                    null,
                    null,
                    null,
                    null),
        }.ToAsyncEnumerable());

        _policyResponse = new PolicyModel(PolicyId.FromInt(12),
                    DateTime.Now,
                    PolicyType.General,
                    "policy",
                    true,
                    PolicyState.Active,
                    Array.Empty<IUsedTagModel>(),
                    Array.Empty<IUsedTagModel>(),
                    Array.Empty<PolicyAclModel>(),
                    Array.Empty<IUsedTrustRequirementModel>(),
                    Array.Empty<PolicyGatewayAllowedIpRange>(),
                    Array.Empty<PolicyGatewayDetailModel>(),
                    null,
                    null,
                    null,
                    null,
                    null);
    }

    [Test]
    public async Task Should_return_a_paginated_response_model_when_calling_GetPoliciesAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/policies").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _paginatedResponse.ToJsonAsync(_serializerOptions)));

        // Act
        var result = await _policiesClient.GetPoliciesAsync();

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public async Task Should_make_a_call_to_api_with_include_disabled_quertString_when_calling_GetPoliciesAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/policies").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _paginatedResponse.ToJsonAsync(_serializerOptions)));

        var includeDisabled = true;

        // Act
        await _policiesClient.GetPoliciesAsync(includeDisabled: includeDisabled);

        // Assert
        _server.Should().HaveReceivedACall().AtAbsoluteUrl($"{_server.Urls[0]}{_orgRoute}/policies?include_disabled={includeDisabled}");
    }

    [Test]
    public async Task Should_make_a_call_to_api_with_search_quertString_when_calling_GetPoliciesAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/policies").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _paginatedResponse.ToJsonAsync(_serializerOptions)));

        var searchTerm = "term";

        // Act
        await _policiesClient.GetPoliciesAsync(searchTerm: searchTerm);

        // Assert
        _server.Should().HaveReceivedACall().AtAbsoluteUrl($"{_server.Urls[0]}{_orgRoute}/policies?search={searchTerm}");
    }


    [Test]
    public async Task Should_make_a_call_to_api_with_sort_quertString_when_calling_GetPoliciesAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/policies").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _paginatedResponse.ToJsonAsync(_serializerOptions)));

        var sortOrder = PolicySortOrder.RecentlyCreated;

        // Act
        await _policiesClient.GetPoliciesAsync(sortOrder: sortOrder);

        // Assert
        _server.Should().HaveReceivedACall().AtAbsoluteUrl($"{_server.Urls[0]}{_orgRoute}/policies?sort={sortOrder}");
    }

    [Test]
    public async Task Should_make_a_call_to_api_with_page_quertString_when_calling_GetPoliciesAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/policies").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _paginatedResponse.ToJsonAsync(_serializerOptions)));

        var page = 12;

        // Act
        await _policiesClient.GetPoliciesAsync(pageNumber: page);

        // Assert
        _server.Should().HaveReceivedACall().AtAbsoluteUrl($"{_server.Urls[0]}{_orgRoute}/policies?page={page}");
    }

    [Test]
    public async Task Should_make_a_call_to_api_with_per_page_quertString_when_calling_GetPoliciesAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/policies").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _paginatedResponse.ToJsonAsync(_serializerOptions)));

        var page = 12;

        // Act
        await _policiesClient.GetPoliciesAsync(perPage: page);

        // Assert
        _server.Should().HaveReceivedACall().AtAbsoluteUrl($"{_server.Urls[0]}{_orgRoute}/policies?per_page={page}");
    }

    [Test]
    public async Task Should_return_policy_model_when_calling_CreateAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/policies").UsingPost())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _policyResponse.ToJsonAsync(_serializerOptions)));

        var createModel = new PolicyCreateModel
        {
            Description = "test",
        };

        // Act
        var result = await _policiesClient.CreateAsync(createModel);

        // Assert
        result.Should().NotBeNull();
        result.Description.Should().Be(_policyResponse.Description);
    }

    [Test]
    public async Task Should_return_number_of_deleted_policies_when_calling_DeletePoliciesAsync()
    {
        // Arrange
        var response = new BulkPolicyDeleteResult(3);

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/policies").UsingDelete())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await response.ToJsonAsync(_serializerOptions)));

        // Act
        var result = await _policiesClient.DeletePoliciesAsync(PolicyId.FromInt(2), PolicyId.FromInt(3), PolicyId.FromInt(5));

        // Assert
        result.Should().Be(3);
    }

    [Test]
    public async Task Should_return_policy_model_when_calling_GetAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/policies/{_policyResponse.Id}").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _policyResponse.ToJsonAsync(_serializerOptions)));

        // Act
        var result = await _policiesClient.GetAsync(PolicyId.FromInt(12));

        // Assert
        result.Should().NotBeNull();
        result.Description.Should().Be(_policyResponse.Description);
    }

    [Test]
    public async Task Should_return_policy_model_when_calling_UpdateAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/policies/{_policyResponse.Id}").UsingPatch())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _policyResponse.ToJsonAsync(_serializerOptions)));

        // Act
        var result = await _policiesClient.Update(PolicyId.FromInt(12)).Set(p => p.IsEnabled, false).ApplyAsync();

        // Assert
        result.Should().NotBeNull();
        result.Description.Should().Be(_policyResponse.Description);
    }

    [Test]
    public async Task Should_return_policy_model_when_calling_DeleteAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/policies/{_policyResponse.Id}").UsingDelete())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _policyResponse.ToJsonAsync(_serializerOptions)));

        // Act
        var result = await _policiesClient.DeleteAsync(PolicyId.FromInt(12));

        // Assert
        result.Should().NotBeNull();
        result.Description.Should().Be(_policyResponse.Description);
    }

    [Test]
    public async Task Should_return_policy_model_when_calling_EnableAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/policies/{_policyResponse.Id}/enable").UsingPut())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _policyResponse.ToJsonAsync(_serializerOptions)));

        // Act
        var result = await _policiesClient.EnableAsync(PolicyId.FromInt(12));

        // Assert
        result.Should().NotBeNull();
        result.IsEnabled.Should().Be(_policyResponse.IsEnabled);
    }

    [Test]
    public async Task Should_return_policy_model_when_calling_DisableAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/policies/{_policyResponse.Id}/disable").UsingPut())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _policyResponse.ToJsonAsync(_serializerOptions)));

        // Act
        var result = await _policiesClient.DisableAsync(PolicyId.FromInt(12));

        // Assert
        result.Should().NotBeNull();
        result.IsEnabled.Should().Be(_policyResponse.IsEnabled);
    }

    [Test]
    public async Task Should_return_number_of_deleted_policies_when_calling_EnablePoliciesAsync()
    {
        // Arrange
        var response = new BulkPolicyUpdateResult(3);

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/policies/enable").UsingPut())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await response.ToJsonAsync(_serializerOptions)));

        // Act
        var result = await _policiesClient.EnablePoliciesAsync(PolicyId.FromInt(2), PolicyId.FromInt(3), PolicyId.FromInt(5));

        // Assert
        result.Should().Be(3);
    }

    [Test]
    public async Task Should_return_number_of_deleted_policies_when_calling_DisablePoliciesAsync()
    {
        // Arrange
        var response = new BulkPolicyUpdateResult(3);

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/policies/disable").UsingPut())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await response.ToJsonAsync(_serializerOptions)));

        // Act
        var result = await _policiesClient.DisablePoliciesAsync(PolicyId.FromInt(2), PolicyId.FromInt(3), PolicyId.FromInt(5));

        // Assert
        result.Should().Be(3);
    }

    // A patch sends only the fields it sets. The API applies every field the body holds, a null one
    // included (portal PatchModel.WasSet, PolicyModifyHandler), so a field set to null is sent as JSON
    // null, and that is how a policy's active hours are cleared. The field set to a value checks the null
    // sits beside other fields rather than replacing the body.
    [Test]
    public async Task Should_send_a_field_set_to_null_as_json_null_when_calling_UpdateAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/policies/{_policyResponse.Id}").UsingPatch())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _policyResponse.ToJsonAsync(_serializerOptions)));

        // Act
        await _policiesClient.Update(PolicyId.FromInt(12))
            .Set(p => p.Description, "policy")
            .Set(p => p.ActiveHours, null)
            .ApplyAsync();

        // Assert
        using var body = JsonDocument.Parse(_server.LogEntries.Should().ContainSingle().Subject.RequestMessage.Body);
        body.RootElement.EnumerateObject().Select(p => (p.Name, p.Value.ValueKind))
            .Should().Equal(("Description", JsonValueKind.String), ("ActiveHours", JsonValueKind.Null));
    }

    // The API names the gateway priority that follows the gateway list's order "Ordered" (sdk
    // Enclave.Sdk.Network/NetworkPolicy/GatewayPriorityType.cs, which the API uses; portal-spa
    // src/types/api.ts), and Enclave.Sdk.Api.Data names the same member Prioritised (portal
    // Enclave.Sdk.Api.Data/Duplicated/GatewayPriorityType.cs). The API does not accept "Prioritised", so
    // a create sends "Ordered".
    [Test]
    public async Task Should_send_the_ordered_gateway_priority_as_Ordered_when_calling_CreateAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/policies").UsingPost())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _policyResponse.ToJsonAsync(_serializerOptions)));

        // Act
        await _policiesClient.CreateAsync(new PolicyCreateModel
        {
            Description = "test",
            GatewayPriority = GatewayPriorityType.Prioritised,
        });

        // Assert
        using var body = JsonDocument.Parse(_server.LogEntries.Should().ContainSingle().Subject.RequestMessage.Body);
        body.RootElement.GetProperty("gatewayPriority").GetString().Should().Be("Ordered");
    }

    // A patch sends the API's name too, for the same reason as a create.
    [Test]
    public async Task Should_send_the_ordered_gateway_priority_as_Ordered_when_calling_UpdateAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/policies/{_policyResponse.Id}").UsingPatch())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _policyResponse.ToJsonAsync(_serializerOptions)));

        // Act
        await _policiesClient.Update(PolicyId.FromInt(12)).Set(p => p.GatewayPriority, GatewayPriorityType.Prioritised).ApplyAsync();

        // Assert
        using var body = JsonDocument.Parse(_server.LogEntries.Should().ContainSingle().Subject.RequestMessage.Body);
        body.RootElement.GetProperty("GatewayPriority").GetString().Should().Be("Ordered");
    }

    // A policy the API returns with "gatewayPriority": "Ordered" reads as the Enclave.Sdk.Api.Data member
    // for that priority, Prioritised.
    [Test]
    public async Task Should_read_the_Ordered_gateway_priority_when_calling_GetAsync()
    {
        // Arrange
        var policy = JsonNode.Parse(await _policyResponse.ToJsonAsync(_serializerOptions));
        policy["gatewayPriority"] = "Ordered";

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/policies/{_policyResponse.Id}").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(policy.ToJsonString()));

        // Act
        var result = await _policiesClient.GetAsync(PolicyId.FromInt(12));

        // Assert
        result.GatewayPriority.Should().Be(GatewayPriorityType.Prioritised);
    }

    // GET policies/meta/search-keys answers with the keys the search term of GetPoliciesAsync accepts
    // (portal PoliciesController.GetSearchKeyMetadata). The keys are two of the policies' search keys
    // (portal PolicySearchKeyService.cs) as the API writes them, with enums as names.
    [Test]
    public async Task Should_return_the_search_keys_when_calling_GetSearchKeysAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/policies/meta/search-keys").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody("""
                [
                  {
                    "name": "state",
                    "modifiers": null,
                    "dataType": "None",
                    "description": "Filter your search by state, either enabled or disabled.",
                    "hintText": "Search for policies that are enabled or disabled",
                    "hintValues": [ "Enabled", "Disabled" ],
                    "canHaveMultiple": false,
                    "isDefault": false,
                    "useExactMatch": false
                  },
                  {
                    "name": "tags",
                    "modifiers": [ "Or" ],
                    "dataType": "Tags",
                    "description": "Filter your search by one or more tags.",
                    "hintText": "Filter by the tags assigned to the policy",
                    "hintValues": null,
                    "canHaveMultiple": true,
                    "isDefault": false,
                    "useExactMatch": false
                  }
                ]
                """));

        // Act
        var result = await _policiesClient.GetSearchKeysAsync();

        // Assert
        result.Should().BeEquivalentTo(
            new[]
            {
                new SearchKey
                {
                    Name = "state",
                    DataType = SearchKeyDataType.None,
                    Description = "Filter your search by state, either enabled or disabled.",
                    HintText = "Search for policies that are enabled or disabled",
                    HintValues = new() { "Enabled", "Disabled" },
                },
                new SearchKey
                {
                    Name = "tags",
                    Modifiers = new() { SearchModifier.Or },
                    DataType = SearchKeyDataType.Tags,
                    Description = "Filter your search by one or more tags.",
                    HintText = "Filter by the tags assigned to the policy",
                    CanHaveMultiple = true,
                },
            },
            options => options.WithStrictOrdering());
    }
}
