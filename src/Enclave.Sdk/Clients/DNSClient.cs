using Enclave.Sdk.Api.Data.Organisations;

namespace Enclave.Sdk.Api.Clients;

public class DnsClient : ClientBase
{
    private string _orgRoute;

    public DnsClient(HttpClient httpClient, string orgRoute)
    : base(httpClient)
    {
        _orgRoute = orgRoute;
    }
}
