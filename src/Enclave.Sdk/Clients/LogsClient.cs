using Enclave.Sdk.Api.Clients.Interfaces;

namespace Enclave.Sdk.Api.Clients;

internal class LogsClient : ClientBase, ILogsClient
{
    private readonly string _orgRoute;

    public LogsClient(HttpClient httpClient, string orgRoute)
    : base(httpClient)
    {
        _orgRoute = orgRoute;
    }
}
