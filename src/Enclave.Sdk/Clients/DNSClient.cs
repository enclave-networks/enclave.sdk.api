using Enclave.Sdk.Api.Clients.Interfaces;

namespace Enclave.Sdk.Api.Clients;

public class DnsClient : ClientBase, IDnsClient
{
    private string _orgRoute;

    public DnsClient(HttpClient httpClient, string orgRoute)
    : base(httpClient)
    {
        _orgRoute = orgRoute;
    }
}
