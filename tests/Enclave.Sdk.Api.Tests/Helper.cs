using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Enclave.Sdk.Api.Tests;
internal static class Helper
{
    internal static async Task<string> ToJsonAsync<T>(this T value, JsonSerializerOptions options)
    {
        var jsonStream = new MemoryStream();
        await JsonSerializer.SerializeAsync(jsonStream, value, options);

        jsonStream.Position = 0;

        using var reader = new StreamReader(jsonStream);
        var readResult = await reader.ReadToEndAsync();
        return readResult;
    }
}
