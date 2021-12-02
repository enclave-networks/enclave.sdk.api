using Enclave.Sdk.Api.Clients;
using Enclave.Sdk.Api.Data.Account;
using System.Text.Json;
using WireMock.Server;

namespace Enclave.Sdk.Api.Tests.Clients;

public class UnapprovedSystemsClientTests
{
    private UnapprovedSystemsClient _unapprovedSystemsClient;
    private WireMockServer _server;
    private string _orgRoute;
    private JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public UnapprovedSystemsClientTests()
    {
        _server = WireMockServer.Start();

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri(_server.Urls[0]),
        };

        _orgRoute = $"/org/{OrganisationId.New()}";

        _unapprovedSystemsClient = new UnapprovedSystemsClient(httpClient, _orgRoute);
    }
}
