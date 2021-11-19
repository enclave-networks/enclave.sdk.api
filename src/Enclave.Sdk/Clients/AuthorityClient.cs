namespace Enclave.Sdk.Api.Clients;

public class AuthorityClient : ClientBase
{
    private string _orgRoute;

    public AuthorityClient(HttpClient httpClient, string orgRoute)
    : base(httpClient)
    {
        _orgRoute = orgRoute;
    }
}
