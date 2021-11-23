using Enclave.Sdk.Api.Clients;
using Enclave.Sdk.Api.Data.Account;
using Enclave.Sdk.Api.Data.Organisations;
using NUnit.Framework;
using System.Text.Json;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Enclave.Sdk.Api.Tests.Clients;

public class OrganisationClientTests
{
    private OrganisationClient _organisationClient;
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

        _organisationClient = new OrganisationClient(httpClient, currentOrganisation);
    }

    [Test]
    public async Task Should_return_a_detailed_organisation_model_when_calling_GetAsync()
    {
        // Arrange
        var org = new Organisation
        {
            Id = OrganisationId.New(),
        };

        _server
          .Given(Request.Create().WithPath(_orgRoute).UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(org, _serializerOptions)));

        // Act
        var result = await _organisationClient.GetAsync();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(org.Id));
    }
}
