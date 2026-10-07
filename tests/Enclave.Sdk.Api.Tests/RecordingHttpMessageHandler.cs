using System.Net.Http.Headers;

namespace Enclave.Sdk.Api.Tests;

// Keeps a copy of every request sent through it, so a test can check the exact URL, Authorization
// header and body a client sent. HttpClient has already combined the request's path with its base
// address when a handler sees the request, and the URI's AbsolutePath keeps the escaping that goes
// on the wire. Built with an inner handler, it passes each request on; built with a responder, it
// answers each request itself and nothing leaves the process.
internal sealed class RecordingHttpMessageHandler : DelegatingHandler
{
    private readonly Func<HttpRequestMessage, HttpResponseMessage> _respond;

    public RecordingHttpMessageHandler(HttpMessageHandler innerHandler)
        : base(innerHandler)
    {
    }

    public RecordingHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> respond)
    {
        _respond = respond;
    }

    public List<RecordedRequest> Requests { get; } = new();

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var body = request.Content is null ? null : await request.Content.ReadAsStringAsync(cancellationToken);

        Requests.Add(new RecordedRequest(request.Method, request.RequestUri, request.Headers.Authorization, body));

        return _respond is null
            ? await base.SendAsync(request, cancellationToken)
            : _respond(request);
    }
}

internal sealed record RecordedRequest(HttpMethod Method, Uri Uri, AuthenticationHeaderValue Authorization, string Body);
