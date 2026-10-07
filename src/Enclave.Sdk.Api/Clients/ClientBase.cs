using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace Enclave.Sdk.Api.Clients;

/// <summary>
/// Base class used for commonly accessed methods and properties for all clients.
/// </summary>
internal abstract class ClientBase
{
    /// <summary>
    /// HttpClient used for all clients API calls.
    /// </summary>
    protected HttpClient HttpClient { get; }

    /// <summary>
    /// Constructor to setup all required fields this is called by all child classes.
    /// </summary>
    /// <param name="httpClient">HttpClient with baseUrl of the API used for all calls.</param>
    protected ClientBase(HttpClient httpClient)
    {
        HttpClient = httpClient;
    }

    /// <summary>
    /// Get a string content for use with HttpClient.
    /// </summary>
    /// <typeparam name="TModel">the object type to encode.</typeparam>
    /// <param name="data">the object to encode.</param>
    /// <returns>String content of object.</returns>
    /// <exception cref="ArgumentNullException">throws if data provided is null.</exception>
    protected static StringContent CreateJsonContent<TModel>(TModel data)
    {
        if (data is null)
        {
            throw new ArgumentNullException(nameof(data), "Data should not be null");
        }

        var json = JsonSerializer.Serialize(data, Constants.JsonSerializerOptions);
        var stringContent = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);

        return stringContent;
    }

    /// <summary>
    /// Desreialize the httpContent.
    /// </summary>
    /// <typeparam name="TModel">the object type to deserialise to.</typeparam>
    /// <param name="httpContent">httpContent from the API call.</param>
    /// <returns>the object of type specified.</returns>
    protected static async Task<TModel?> DeserialiseAsync<TModel>(HttpContent httpContent)
    {
        if (httpContent is null)
        {
            return default;
        }

        return await httpContent.ReadFromJsonAsync<TModel>(Constants.JsonSerializerOptions);
    }

    /// <summary>
    /// Checks model is not null.
    /// </summary>
    /// <typeparam name="TModel">Type being checked.</typeparam>
    /// <param name="model">object being checked.</param>
    protected static void EnsureNotNull<TModel>([NotNull] TModel? model)
    {
        if (model is null)
        {
            Throw();
        }
    }

    /// <summary>
    /// Escapes a caller's value, such as a system ID or a tag name, for use as one segment of a URL path.
    /// </summary>
    /// <param name="value">The value to put in the path.</param>
    /// <param name="paramName">The caller's parameter name, given to the exception.</param>
    /// <returns>The escaped value.</returns>
    /// <exception cref="ArgumentNullException">Throws if <paramref name="value"/> is null.</exception>
    /// <exception cref="ArgumentException">Throws if <paramref name="value"/> is empty, "." or "..".</exception>
    protected static string PathSegment(string value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        // A value put into a path unescaped can move the request to another route: .NET removes ".."
        // segments when it combines the path with the base address (RFC 3986 section 5.2.4), so declining
        // the unapproved system "../systems/ABCDE" would revoke enrolled system ABCDE.
        //
        // Uri.EscapeDataString escapes every character except the unreserved ones (letters, digits, '-',
        // '.', '_' and '~'; RFC 3986 section 2.3; https://learn.microsoft.com/dotnet/api/system.uri.escapedatastring),
        // so '/', '\', '?', '#' and '%' cannot end the segment. It leaves "." and ".." as they are, and they
        // are still dot-segments, and an empty value drops the segment. None of the three is an ID, so they
        // are refused before a request is built.
        ArgumentNullException.ThrowIfNull(value, paramName);

        if (value is "" or "." or "..")
        {
            throw new ArgumentException($"\"{value}\" is not a valid ID. An empty value, \".\" and \"..\" cannot be sent as a segment of a URL path.", paramName);
        }

        return Uri.EscapeDataString(value);
    }

    /// <summary>
    /// Throws an error every time it's called.
    /// </summary>
    /// <exception cref="InvalidOperationException">Throws every time this is called.</exception>
    [DoesNotReturn]
    private static void Throw() =>
        throw new InvalidOperationException("Return from API is null please ensure you've entered the correct data or raise an issue");
}