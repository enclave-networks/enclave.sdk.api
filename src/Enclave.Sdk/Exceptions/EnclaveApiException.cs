namespace Enclave.Sdk.Api.Exceptions;

/// <summary>
/// An Exception to display the details of an Http Problem Details.
/// </summary>
public class EnclaveApiException : Exception
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
    public EnclaveApiException(ProblemDetails problemDetails, HttpResponseMessage response)
        : base(GetMessage(problemDetails))
    {
        ProblemDetails = problemDetails!;
        Response = response;
    }

    private static string GetMessage(ProblemDetails problemDetails)
    {
        if (problemDetails is not null
            && problemDetails.Errors.Count == 1)
        {
            return string.Join("\n", problemDetails.Errors.First().Value);
        }

        return $"{problemDetails?.Title} - Check Problem Details for more details";
    }
}