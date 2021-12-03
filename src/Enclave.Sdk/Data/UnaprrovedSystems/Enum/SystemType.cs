using System.Text.Json.Serialization;

namespace Enclave.Sdk.Api.Data.UnaprrovedSystems.Enum;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SystemType
{
    GeneralPurpose,
    Ephemeral,
}
