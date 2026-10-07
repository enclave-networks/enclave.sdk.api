using System.Text.Json;
using System.Text.Json.Serialization;
using Enclave.Sdk.Api.Data;

namespace Enclave.Sdk.Api;

internal static class Constants
{
    public static JsonSerializerOptions JsonSerializerOptions
    {
        get
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

            // System.Text.Json uses the first converter in the list that can convert a type
            // (https://learn.microsoft.com/dotnet/standard/serialization/system-text-json/converters-how-to#converter-registration-precedence),
            // and JsonStringEnumConverter converts every enum, so the gateway priority converter goes first.
            options.Converters.Add(new GatewayPriorityTypeJsonConverter());
            options.Converters.Add(new JsonStringEnumConverter());
            return options;
        }
    }

    public const string ApiUrl = "https://api.enclave.io";

    public const string PartnerApiUrl = "https://partner-api.enclave.io";
}