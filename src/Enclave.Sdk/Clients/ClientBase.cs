using Enclave.Sdk.Api.Exceptions;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

namespace Enclave.Sdk.Api.Clients;
public class ClientBase
{
    protected HttpClient HttpClient { get; private set; }

    protected JsonSerializerOptions JsonSerializerOptions { get; private set; }

    public ClientBase(HttpClient httpClient)
    {
        HttpClient = httpClient;
        JsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
    }

    protected StringContent Encode<TModel>(TModel data)
    {
        if (data is null)
        {
            throw new ArgumentNullException(nameof(data), "Data should not be null");
        }

        var json = JsonSerializer.Serialize(data, JsonSerializerOptions);
        var stringContent = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json); // use MediaTypeNames.Application.Json in Core 3.0+ and Standard 2.1+

        return stringContent;
    }

    protected async Task<TModel?> DeserializeAsync<TModel>(HttpContent httpContent)
    {
        if (httpContent is null)
        {
            return default;
        }

        var contentStream = await httpContent.ReadAsStreamAsync();
        return await JsonSerializer.DeserializeAsync<TModel>(contentStream, JsonSerializerOptions);
    }

    protected async Task CheckStatusCodes(HttpResponseMessage httpResponse)
    {
        if (httpResponse is null)
        {
            throw new ArgumentNullException(nameof(httpResponse), "httpResponse should not be null are you sure a call has been made");
        }

        var responseText = await httpResponse.Content.ReadAsStringAsync();
        if (httpResponse.StatusCode == HttpStatusCode.BadRequest)
        {
            throw new ApiException("Bad request; ensure you have provided the correct data to the Api", httpResponse.StatusCode, responseText, httpResponse.Headers);
        }
        else if (httpResponse.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new ApiException("Unauthorized request; ensure you have provided a valid Access Token with \'Authorization: Bearer {token}\'.", httpResponse.StatusCode, responseText, httpResponse.Headers);
        }
        else if (httpResponse.StatusCode == HttpStatusCode.Forbidden)
        {
            throw new ApiException("The provided token does not grant rights to this request.", httpResponse.StatusCode, responseText, httpResponse.Headers);
        }
        else if (!httpResponse.IsSuccessStatusCode)
        {
            throw new ApiException($"The HTTP status code of the response was not expected ({httpResponse.StatusCode}).", httpResponse.StatusCode, responseText, httpResponse.Headers);
        }
    }

    protected void CheckModel<TModel>([NotNull] TModel? model)
    {
        if (model is null)
        {
            Throw();
        }
    }

    [DoesNotReturn]
    private void Throw() =>
        throw new InvalidOperationException("Return from API is null please ensure you've entered the correct data or raise an issue");

    protected virtual string PrepareUrl(string url)
    {
        return url;
    }
}