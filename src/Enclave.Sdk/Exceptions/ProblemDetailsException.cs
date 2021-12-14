namespace Enclave.Sdk.Api.Exceptions;

/// <summary>
/// An Exception to display the details of an Http Problem Details.
/// </summary>
public class ProblemDetailsException : Exception
{
    /// <summary>
    /// The Problem Details.
    /// </summary>
    public ProblemDetails ProblemDetails { get; }

    /// <summary>
    /// The HttpResponse message.
    /// </summary>
    public HttpResponseMessage Response { get; }

    /// <summary>
    /// Constructor for setting up the problem details exception.
    /// </summary>
    /// <param name="problemDetails">The problem details from the HTTP response.</param>
    /// <param name="response">The HTTP Response message.</param>
    public ProblemDetailsException(ProblemDetails problemDetails, HttpResponseMessage response)
        : base(problemDetails?.Title)
    {
        ProblemDetails = problemDetails!;
        Response = response;
    }
}