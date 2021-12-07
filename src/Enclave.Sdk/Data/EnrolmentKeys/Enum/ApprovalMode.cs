using System.Text.Json.Serialization;

namespace Enclave.Sdk.Api.Data.EnrolmentKeys.Enum;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ApprovalMode
{
    Automatic,
    Manual,
}