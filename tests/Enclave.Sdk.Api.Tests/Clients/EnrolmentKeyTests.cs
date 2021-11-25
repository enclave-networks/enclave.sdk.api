using Enclave.Sdk.Api.Clients;
using Enclave.Sdk.Api.Data.Account;
using NUnit.Framework;
using System.Net.WebSockets;
using System.Text.Json;
using WireMock.Server;

namespace Enclave.Sdk.Api.Tests.Clients;

public class EnrolmentKeyTests
{
    private EnrolmentKeysClient _enrolmentKeysClient;
    private WireMockServer _server;
    private string _orgRoute;
    private JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };


    [SetUp]
    public void Setup()
    {
        _server = WireMockServer.Start();

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri(_server.Urls[0]),
        };

        var currentOrganisation = new AccountOrganisation
        {
            OrgId = OrganisationId.New(),
            OrgName = "TestName",
            Role = UserOrganisationRole.Admin,
        };

        _orgRoute = $"/org/{currentOrganisation.OrgId}";

        // Not sure if this is the best way of doing this
        var organisationClient = new OrganisationClient(httpClient, currentOrganisation);
        _enrolmentKeysClient = (EnrolmentKeysClient)organisationClient.EnrolmentKeys;
    }
}
