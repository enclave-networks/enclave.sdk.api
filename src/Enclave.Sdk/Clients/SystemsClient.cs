namespace Enclave.Sdk.Api.Clients;

public class SystemsClient : ClientBase
{
    private string _orgRoute;

    public SystemsClient(HttpClient httpClient, string orgRoute)
    : base(httpClient)
    {
        _orgRoute = orgRoute;
    }
}
