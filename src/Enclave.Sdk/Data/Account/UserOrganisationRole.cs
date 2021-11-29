using System.Text.Json.Serialization;

namespace Enclave.Sdk.Api.Data.Account;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UserOrganisationRole
{
    Owner,
    Admin,
}