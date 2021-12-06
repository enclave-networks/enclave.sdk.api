using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using System.Net.Mime;
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
    protected StringContent CreateJsonContent<TModel>(TModel data)
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
    protected async Task<TModel?> DeserialiseAsync<TModel>(HttpContent httpContent)
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
    /// Throws an error every time it's called.
    /// </summary>
    /// <exception cref="InvalidOperationException">Throws every time this is called.</exception>
    [DoesNotReturn]
    private static void Throw() =>
        throw new InvalidOperationException("Return from API is null please ensure you've entered the correct data or raise an issue");
}