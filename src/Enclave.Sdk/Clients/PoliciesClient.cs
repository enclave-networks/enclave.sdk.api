namespace Enclave.Sdk.Api.Clients;

public class PoliciesClient : ClientBase
{
    private string _orgRoute;

    public PoliciesClient(HttpClient httpClient, string orgRoute)
    : base(httpClient)
    {
        _orgRoute = orgRoute;
    }
}
