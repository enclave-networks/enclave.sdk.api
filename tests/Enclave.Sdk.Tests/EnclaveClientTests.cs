using Enclave.Sdk.Api.Data.Account;
using NUnit.Framework;
using System.Text.Json;
using System.Text.Json.Serialization;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Enclave.Sdk.Api.Tests;

public class EnclaveClientTests
{
    private EnclaveClient _client;
    private WireMockServer _server;

    [SetUp]
    public void Setup()
    {
        var httpClient = new HttpClient();
        _server = WireMockServer.Start();
        _client = new EnclaveClient(httpClient, _server.Urls[0], string.Empty);
    }

    [Test]
    public async Task should_return_list_of_orgs_when_calling_GetOrganisationsAsync()
    {
        // Arrange
        var accountOrg = new AccountOrganisationTopLevel
        {
            Orgs = new List<AccountOrganisation>
            {
                new AccountOrganisation
                {
                    OrgId = "testId",
                    OrgName = "TestName",
                    Role = UserOrganisationRole.Admin,
                },
            },
        };

        _server
          .Given(Request.Create().WithPath("/account/orgs").UsingGet())
          .RespondWith(
            Response.Create()
              .WithStatusCode(200)
              .WithBody(JsonSerializer.Serialize(accountOrg)));

        // Act
        var result = await _client.GetOrganisationsAsync();

        // Assert
        Assert.That(result.FirstOrDefault().OrgId,
            Is.EqualTo(accountOrg.Orgs.FirstOrDefault().OrgId));
    }

    [Test]
    public async Task should_throw_aggregate_exception_if_server_is_unreachable()
    {
        // Assert
    }

    [Test]
    public async Task should_throw_invalid_operation_exception_if_response_does_not_contain_an_organisation_model()
    {
        // Arrange
        _server
           .Given(Request.Create().WithPath("/account/orgs").UsingGet())
           .RespondWith(
             Response.Create()
               .WithStatusCode(200)
               .WithBody(JsonSerializer.Serialize("{}")));

        // Assert
        Assert.Throws<AggregateException>(() => _client.GetOrganisationsAsync().Wait());
    }
}