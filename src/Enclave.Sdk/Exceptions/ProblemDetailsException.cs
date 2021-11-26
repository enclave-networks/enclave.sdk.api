namespace Enclave.Sdk.Api.Exceptions;

internal class ProblemDetailsException : Exception
{
    public ProblemDetails ProblemDetails { get; }

    public HttpResponseMessage Response { get; }

    public ProblemDetailsException(ProblemDetails problemDetails, HttpResponseMessage response)
        : base(problemDetails.Title)
    {
        ProblemDetails = problemDetails;
        Response = response;
    }
}