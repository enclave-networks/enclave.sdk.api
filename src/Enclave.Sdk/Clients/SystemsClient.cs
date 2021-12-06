using Enclave.Sdk.Api.Clients.Interfaces;

namespace Enclave.Sdk.Api.Clients;

internal class SystemsClient : ClientBase, ISystemsClient
{
    private readonly string _orgRoute;

    public SystemsClient(HttpClient httpClient, string orgRoute)
    : base(httpClient)
    {
        _orgRoute = orgRoute;
    }
}
