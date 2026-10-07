using System.Text.Json;
using System.Text.Json.Serialization;
using Enclave.Sdk.Api.Data;
using Enclave.Sdk.Network.NetworkPolicy;
using FluentAssertions;
using NUnit.Framework;

namespace Enclave.Sdk.Api.Tests;

// The API's gateway priorities are Balanced, Ordered and Geographic (sdk
// Enclave.Sdk.Network/NetworkPolicy/GatewayPriorityType.cs, which the API uses; portal-spa src/types/api.ts).
// Enclave.Sdk.Api.Data names Ordered "Prioritised" (portal Enclave.Sdk.Api.Data/Duplicated/GatewayPriorityType.cs).
// These tests use the serializer options every client reads and writes with, so they also check the
// gateway priority mapping takes precedence over the enum converter that writes every other enum by name.
public class GatewayPriorityTypeJsonConverterTests
{
    [TestCase(GatewayPriorityType.Balanced, "\"Balanced\"")]
    [TestCase(GatewayPriorityType.Prioritised, "\"Ordered\"")]
    [TestCase(GatewayPriorityType.Geographic, "\"Geographic\"")]
    public void Should_write_the_api_name_of_each_gateway_priority(GatewayPriorityType value, string expected)
    {
        // Act
        var json = JsonSerializer.Serialize(value, Constants.JsonSerializerOptions);

        // Assert
        json.Should().Be(expected);
    }

    // Model properties hold the priority as GatewayPriorityType?, which System.Text.Json converts with the
    // converter for GatewayPriorityType, writing null as null.
    [TestCase(GatewayPriorityType.Prioritised, "\"Ordered\"")]
    [TestCase(null, "null")]
    public void Should_write_the_api_name_of_a_nullable_gateway_priority(GatewayPriorityType? value, string expected)
    {
        // Act
        var json = JsonSerializer.Serialize(value, Constants.JsonSerializerOptions);

        // Assert
        json.Should().Be(expected);
    }

    // "Prioritised" is read as well as "Ordered", so JSON written with the Enclave.Sdk.Api.Data name, by an
    // earlier Enclave.Sdk.Api or by a caller's own serializer, still reads. Names match ignoring case, and a
    // number reads as the member with that value, as JsonStringEnumConverter reads other enums.
    [TestCase("\"Balanced\"", GatewayPriorityType.Balanced)]
    [TestCase("\"Ordered\"", GatewayPriorityType.Prioritised)]
    [TestCase("\"Prioritised\"", GatewayPriorityType.Prioritised)]
    [TestCase("\"Geographic\"", GatewayPriorityType.Geographic)]
    [TestCase("\"ordered\"", GatewayPriorityType.Prioritised)]
    [TestCase("1", GatewayPriorityType.Prioritised)]
    public void Should_read_each_gateway_priority(string json, GatewayPriorityType expected)
    {
        // Act
        var value = JsonSerializer.Deserialize<GatewayPriorityType>(json, Constants.JsonSerializerOptions);

        // Assert
        value.Should().Be(expected);
    }

    [TestCase("\"Fastest\"")]
    [TestCase("true")]
    public void Should_refuse_a_value_that_is_not_a_gateway_priority(string json)
    {
        // Act
        var act = () => JsonSerializer.Deserialize<GatewayPriorityType>(json, Constants.JsonSerializerOptions);

        // Assert
        act.Should().Throw<JsonException>();
    }

    // A caller that writes the models with its own serializer options, as a CLI printing them does, adds
    // the converter ahead of its enum converter to write the same names the API uses.
    [Test]
    public void Should_write_the_api_name_when_added_to_a_callers_options_ahead_of_the_enum_converter()
    {
        // Arrange
        var options = new JsonSerializerOptions
        {
            Converters = { new GatewayPriorityTypeJsonConverter(), new JsonStringEnumConverter() },
        };

        // Act
        var json = JsonSerializer.Serialize(GatewayPriorityType.Prioritised, options);

        // Assert
        json.Should().Be("\"Ordered\"");
    }
}
