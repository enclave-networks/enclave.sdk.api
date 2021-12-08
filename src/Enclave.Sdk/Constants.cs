using System.Text.Json;
using System.Text.Json.Serialization;

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

            options.Converters.Add(new JsonStringEnumConverter());
            return options;
        }
    }

    public const string ApiUrl = "https://api.enclave.io";
}