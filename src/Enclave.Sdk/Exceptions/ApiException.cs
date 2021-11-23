using System.Net;
using System.Net.Http.Headers;

namespace Enclave.Sdk.Api.Exceptions;

/// <summary>
/// Exception used for Api specific errors.
/// </summary>
public class ApiException : Exception
{
    /// <summary>
    /// Http Status Code.
    /// </summary>
    public HttpStatusCode StatusCode { get; private set; }

    /// <summary>
    /// Response represented as a string.
    /// </summary>
    public string Response { get; private set; }

    /// <summary>
    /// Http Response Headers.
    /// </summary>
    public HttpResponseHeaders Headers { get; private set; }

    /// <summary>
    /// Constructor for generating an ApiException.
    /// </summary>
    /// <param name="message">user defined error message.</param>
    /// <param name="statusCode">http status code.</param>
    /// <param name="response">http response as string.</param>
    /// <param name="headers">HttpResponseHeaders.</param>
    public ApiException(string message, HttpStatusCode statusCode, string response, HttpResponseHeaders headers)
        : base(message + "\n\nStatus: " + statusCode + "\nResponse: \n" + (response == null ? "(null)" : response.Substring(0, response.Length >= 512 ? 512 : response.Length)))
    {
        StatusCode = statusCode;
        Response = response ?? string.Empty;
        Headers = headers;
    }

    /// <summary>
    /// Exception as string.
    /// </summary>
    /// <returns>Exception as a string</returns>
    public override string ToString()
    {
        return string.Format("HTTP Response: \n\n{0}\n\n{1}", Response, base.ToString());
    }
}