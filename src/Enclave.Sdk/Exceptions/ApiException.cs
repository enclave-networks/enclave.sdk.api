using System.Net;
using System.Net.Http.Headers;

namespace Enclave.Sdk.Api.Exceptions;

public class ApiException : Exception
{
    public HttpStatusCode StatusCode { get; private set; }

    public string Response { get; private set; }

    public HttpResponseHeaders Headers { get; private set; }

    public ApiException(string message, HttpStatusCode statusCode, string response, HttpResponseHeaders headers)
        : base(message + "\n\nStatus: " + statusCode + "\nResponse: \n" + (response == null ? "(null)" : response.Substring(0, response.Length >= 512 ? 512 : response.Length)))
    {
        StatusCode = statusCode;
        Response = response;
        Headers = headers;
    }

    public override string ToString()
    {
        return string.Format("HTTP Response: \n\n{0}\n\n{1}", Response, base.ToString());
    }
}