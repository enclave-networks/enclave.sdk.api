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
        BaseUrl = Constants.ApiUrl;
    }

    /// <summary>
    /// the Personal access token from the Enclave portal.
    /// </summary>
    public string? PersonalAccessToken { get; set; }

    /// <summary>
    /// The base URL of the Enclave API endpoint.
    /// </summary>
    public string BaseUrl { get; set; }
}
