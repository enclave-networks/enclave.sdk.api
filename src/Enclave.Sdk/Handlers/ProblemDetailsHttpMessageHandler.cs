using System.Net.Http.Json;
using Enclave.Sdk.Api.Exceptions;

namespace Enclave.Sdk.Api.Handlers;

internal sealed class ProblemDetailsHttpMessageHandler : DelegatingHandler
{
    public ProblemDetailsHttpMessageHandler()
#pragma warning disable CA2000 // Dispose objects before losing scope
        : base(new HttpClientHandler()) { }
#pragma warning restore CA2000 // Dispose objects before losing scope

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        var response = await base.SendAsync(request, ct);

        var mediaType = response.Content.Headers.ContentType?.MediaType;
        if (mediaType != null && mediaType.Equals("application/problem+json", StringComparison.OrdinalIgnoreCase))
        {
            var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>(Constants.JsonSerializerOptions, ct) ?? new ProblemDetails();
            throw new ProblemDetailsException(problemDetails, response);
        }

        return response;
    }
}