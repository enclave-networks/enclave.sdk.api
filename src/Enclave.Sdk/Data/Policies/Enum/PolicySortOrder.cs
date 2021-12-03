using System.Text.Json.Serialization;

namespace Enclave.Sdk.Api.Data.Policies;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PolicySortOrder
{
    Description,
    RecentlyCreated,
}