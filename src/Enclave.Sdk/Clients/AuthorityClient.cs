using Enclave.Sdk.Api.Clients.Interfaces;

namespace Enclave.Sdk.Api.Clients;

public class AuthorityClient : ClientBase, IAuthorityClient
{
    private readonly string _orgRoute;

    public AuthorityClient(HttpClient httpClient, string orgRoute)
    : base(httpClient)
    {
        _orgRoute = orgRoute;
    }
}
