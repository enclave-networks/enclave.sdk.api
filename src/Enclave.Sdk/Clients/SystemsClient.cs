using Enclave.Sdk.Api.Clients.Interfaces;

namespace Enclave.Sdk.Api.Clients;

public class SystemsClient : ClientBase, ISystemsClient
{
    private string _orgRoute;

    public SystemsClient(HttpClient httpClient, string orgRoute)
    : base(httpClient)
    {
        _orgRoute = orgRoute;
    }
}
