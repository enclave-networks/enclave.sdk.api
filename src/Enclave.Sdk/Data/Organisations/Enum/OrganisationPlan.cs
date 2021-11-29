using System.Text.Json.Serialization;

namespace Enclave.Sdk.Api.Data.Organisations.Enum;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OrganisationPlan
{
    Starter = 0,
    Pro = 1,
    Business = 2,
}
