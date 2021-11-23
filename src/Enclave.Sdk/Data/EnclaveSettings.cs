using System.Text.Json.Serialization;

namespace Enclave.Sdk.Api.Data;

/// <summary>
/// A representation of the enclave settings json file.
/// </summary>
public class EnclaveSettings
{
    /// <summary>
    /// the bearer token from encalve.
    /// </summary>
    public string? BearerToken { get; set; }

    /// <summary>
    /// The Api base url.
    /// </summary>
    public string? BaseUrl { get; set; }

    /// <summary>
    /// Custom httpClient for use with all Client classes.
    /// </summary>
    [JsonIgnore]
    public HttpClient? HttpClient { get; set; }
}
