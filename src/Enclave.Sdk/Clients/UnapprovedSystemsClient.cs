namespace Enclave.Sdk.Api.Clients;

public class UnapprovedSystemsClient : ClientBase
{
    private string _orgRoute;

    public UnapprovedSystemsClient(HttpClient httpClient, string orgRoute)
    : base(httpClient)
    {
        _orgRoute = orgRoute;
    }
}
