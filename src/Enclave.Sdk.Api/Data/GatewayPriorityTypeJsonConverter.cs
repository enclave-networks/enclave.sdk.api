using System.Text.Json;
using System.Text.Json.Serialization;
using Enclave.Sdk.Network.NetworkPolicy;

namespace Enclave.Sdk.Api.Data;

/// <summary>
/// Reads and writes <see cref="GatewayPriorityType"/> with the names the Enclave API uses. The API names the priority that
/// follows the order of the gateway list "Ordered", where <see cref="GatewayPriorityType"/> names it
/// <see cref="GatewayPriorityType.Prioritised"/>. This converter writes "Ordered" for it and reads both "Ordered" and
/// "Prioritised" as it, and reads and writes every other value as <see cref="JsonStringEnumConverter"/> does.
/// Every Enclave.Sdk.Api client uses it. To write the models with the API's names in your own
/// <see cref="JsonSerializerOptions"/>, add it to <see cref="JsonSerializerOptions.Converters"/> ahead of any
/// <see cref="JsonStringEnumConverter"/>.
/// </summary>
public sealed class GatewayPriorityTypeJsonConverter : JsonConverter<GatewayPriorityType>
{
    // Enclave.Sdk.Api.Data 304.48.0 compiles its own copy of the enum with the member Prioritised (portal
    // Enclave.Sdk.Api.Data/Duplicated/GatewayPriorityType.cs). The API uses the sdk repository's enum, which
    // has Ordered at the same position (sdk Enclave.Sdk.Network/NetworkPolicy/GatewayPriorityType.cs; portal-spa
    // src/types/api.ts). JsonStringEnumConverter writes and reads by member name, so on its own it sends
    // "Prioritised", which the API does not accept, and cannot read "Ordered".
    //
    // Every other value goes to JsonStringEnumConverter, so this converter changes nothing else about how the
    // enum reads and writes: names match ignoring case, and numbers are accepted.
    private const string OrderedName = "Ordered";

    private static readonly JsonConverter<GatewayPriorityType> EnumConverter =
        (JsonConverter<GatewayPriorityType>)new JsonStringEnumConverter<GatewayPriorityType>()
            .CreateConverter(typeof(GatewayPriorityType), JsonSerializerOptions.Default)!;

    /// <inheritdoc/>
    public override GatewayPriorityType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String
            && string.Equals(reader.GetString(), OrderedName, StringComparison.OrdinalIgnoreCase))
        {
            return GatewayPriorityType.Prioritised;
        }

        return EnumConverter.Read(ref reader, typeToConvert, options);
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, GatewayPriorityType value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);

        if (value == GatewayPriorityType.Prioritised)
        {
            writer.WriteStringValue(OrderedName);
            return;
        }

        EnumConverter.Write(writer, value, options);
    }
}
