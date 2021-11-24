using System.Text.Json.Serialization;

namespace Enclave.Sdk.Api;

/// <summary>
/// A representation of options used when creating <see cref="EnclaveClient"/>.
/// </summary>
public class EnclaveClientOptions
{
    /// <summary>
    /// Default constructor which sets the BaseURL to the production version of the Enclave API.
    /// </summary>
    public EnclaveClientOptions()
    {
        BaseUrl = "https://api.enclave.io";
    }

    /// <summary>
    /// the Personal access token from the Enclave portal.
    /// </summary>
    public string? PersonalAccessToken { get; set; }

    /// <summary>
    /// The base URL of the Enclave API endpoint.
    /// </summary>
    public string BaseUrl { get; set; }

    /// <summary>
    /// Custom HttpClient for use with all Client classes.
    /// </summary>
    [JsonIgnore]
    public HttpClient? HttpClient { get; set; }
}
