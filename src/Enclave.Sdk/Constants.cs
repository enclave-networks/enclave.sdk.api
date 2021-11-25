using System.Text.Json;

namespace Enclave.Sdk.Api;

internal static class Constants
{
    public static JsonSerializerOptions JsonSerializerOptions { get; } = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public const string ApiUrl = "https://api.enclave.io";
}