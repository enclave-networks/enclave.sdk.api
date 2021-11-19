namespace Enclave.Sdk.Api.Clients;

public class EnrolmentKeysClient : ClientBase
{
    private string _orgRoute;

    public EnrolmentKeysClient(HttpClient httpClient, string orgRoute)
    : base(httpClient)
    {
        _orgRoute = orgRoute;
    }
}
