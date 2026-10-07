using System.Text.Json;
using System.Text.Json.Nodes;
using Enclave.Sdk.Api.Data;
using FluentAssertions;
using NUnit.Framework;

namespace Enclave.Sdk.Api.Tests;

// SearchKey is the API's search key model (portal src/Enclave.Configuration.Data/Modules/Search/SearchKey.cs),
// which Enclave.Sdk.Api.Data does not include. Each test reads JSON as the API writes it, with the options
// every client uses: camelCase names, enums as names, and null for a list that is not set (portal
// src/Enclave.Api.Scaffolding/CommonWebStartup.cs, AddJsonOptions).
public class SearchKeyTests
{
    // The keys are the "version" and "description" search keys for systems (portal
    // src/Enclave.Configuration.Data/Modules/Systems/SystemSearchKeyService.cs), as the API writes them.
    // Reading them and writing them again gives the same JSON only if the model has each of the API's
    // properties under the API's name and type, and no other.
    [Test]
    public void Should_write_the_json_the_api_writes_after_reading_it()
    {
        // Arrange
        const string apiJson = """
            [
              {
                "name": "version",
                "modifiers": [
                  "LessThan",
                  "GreaterThan"
                ],
                "dataType": "None",
                "description": "Filter your search by the Enclave agent version.",
                "hintText": "Filter by the Enclave agent version",
                "hintValues": null,
                "canHaveMultiple": false,
                "isDefault": false,
                "useExactMatch": false
              },
              {
                "name": "description",
                "modifiers": null,
                "dataType": "None",
                "description": "Filter your search by description.",
                "hintText": "Search by description or hostname",
                "hintValues": null,
                "canHaveMultiple": false,
                "isDefault": true,
                "useExactMatch": false
              }
            ]
            """;

        // Act
        var keys = JsonSerializer.Deserialize<List<SearchKey>>(apiJson, Constants.JsonSerializerOptions);
        var written = JsonSerializer.Serialize(keys, Constants.JsonSerializerOptions);

        // Assert
        JsonNode.DeepEquals(JsonNode.Parse(written), JsonNode.Parse(apiJson)).Should().BeTrue("the JSON written was {0}", written);
    }

    // Every member of the API's SearchModifier (portal src/Enclave.Configuration.Data/Modules/Search/SearchModifier.cs;
    // portal-spa src/types/api.ts, SearchModifier).
    [TestCase("LessThan", SearchModifier.LessThan)]
    [TestCase("GreaterThan", SearchModifier.GreaterThan)]
    [TestCase("Not", SearchModifier.Not)]
    [TestCase("Or", SearchModifier.Or)]
    public void Should_read_each_modifier_the_api_writes(string name, SearchModifier modifier)
    {
        // Arrange
        var json = $$"""{ "name": "uses", "modifiers": [ "{{name}}" ] }""";

        // Act
        var key = JsonSerializer.Deserialize<SearchKey>(json, Constants.JsonSerializerOptions);

        // Assert
        key.Modifiers.Should().Equal(modifier);
    }

    // Every member of the API's SearchKeyDataType (portal src/Enclave.Configuration.Data/Modules/Search/SearchKeyDataType.cs;
    // portal-spa src/types/api.ts, SearchKeyDataType).
    [TestCase("None", SearchKeyDataType.None)]
    [TestCase("Tags", SearchKeyDataType.Tags)]
    [TestCase("System", SearchKeyDataType.System)]
    [TestCase("EnrolmentKey", SearchKeyDataType.EnrolmentKey)]
    public void Should_read_each_data_type_the_api_writes(string name, SearchKeyDataType dataType)
    {
        // Arrange
        var json = $$"""{ "name": "tags", "dataType": "{{name}}" }""";

        // Act
        var key = JsonSerializer.Deserialize<SearchKey>(json, Constants.JsonSerializerOptions);

        // Assert
        key.DataType.Should().Be(dataType);
    }
}
