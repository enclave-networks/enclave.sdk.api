using System.Text.Json.Serialization;
using Enclave.Sdk.Api.Exceptions;

namespace Enclave.Sdk.Api;

/// <summary>
/// A representation of options used when creating <see cref="EnclaveClient"/>.
/// </summary>
public class EnclaveClientOptions
{
    /// <summary>
    /// Default constructor which sets the BaseURL and PartnerApiBaseUrl to the production versions of the Enclave API
    /// and the Enclave Partner API.
    /// </summary>
    public EnclaveClientOptions()
    {
        BaseUrl = Constants.ApiUrl;
        PartnerApiBaseUrl = Constants.PartnerApiUrl;
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
    /// The base URL of the Enclave Partner API, which the clients from <see cref="EnclaveClient.CreatePartnerClient"/> call.
    /// The partner API runs on its own host; this defaults to production, <c>https://partner-api.enclave.io</c>.
    /// Staging is <c>https://staging-partner-api.enclave.io</c>, used together with a <see cref="BaseUrl"/> of
    /// <c>https://staging-api.enclave.io</c>. Setting it to null or blank leaves no partner API, and
    /// <see cref="EnclaveClient.CreatePartnerClient"/> then throws. A credentials file can set it as <c>partnerApiBaseUrl</c>.
    /// </summary>
    public string? PartnerApiBaseUrl { get; set; }

    /// <summary>
    /// An optional handler that sends the HTTP requests of the <see cref="EnclaveClient"/> and of every client it creates,
    /// in place of the default <see cref="HttpClientHandler"/>. It can log requests, or answer them itself so nothing is sent.
    /// It sees each request with the Authorization header set, and a problem+json response it returns still throws
    /// <see cref="EnclaveApiException"/>. Enclave.Sdk.Api never disposes it: the caller owns it, and disposes it once no
    /// client created from these options will send another request. It is not read from or written to JSON.
    /// </summary>
    [JsonIgnore]
    public HttpMessageHandler? HttpMessageHandler { get; set; }
}
