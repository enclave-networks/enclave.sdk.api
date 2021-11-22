using Enclave.Sdk.Api.Clients.Interfaces;

namespace Enclave.Sdk.Api.Clients;

public class UnapprovedSystemsClient : ClientBase, IUnapprovedSystemsClient
{
    private string _orgRoute;

    public UnapprovedSystemsClient(HttpClient httpClient, string orgRoute)
    : base(httpClient)
    {
        _orgRoute = orgRoute;
    }


}
