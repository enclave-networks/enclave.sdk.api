using Enclave.Sdk.Api.Clients;
using Enclave.Sdk.Api.Data.Account;
using Enclave.Sdk.Api.Data.Organisations;
using NUnit.Framework;
using System.Text.Json;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Enclave.Sdk.Api.Tests.Clients
{
    public class OrganisationClientTests
    {
        private OrganisationClient _organisationClient;
        private WireMockServer _server;
        private string _orgRoute;

        [SetUp]
        public void Setup()
        {
            _server = WireMockServer.Start();

            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(_server.Urls[0])
            };

            var currentOrganisation = new AccountOrganisation
            {
                OrgId = "testId",
                OrgName = "TestName",
                Role = UserOrganisationRole.Admin,
            };

            _orgRoute = $"/org/{currentOrganisation.OrgId}";

            _organisationClient = new OrganisationClient(httpClient, currentOrganisation);
        }

        public async Task should_return_a_detailed_organisation_model_when_calling_GetAsync()
        {
            // Arrange
            var org = new Organisation();

            _server
              .Given(Request.Create().WithPath(_orgRoute).UsingGet())
              .RespondWith(
                Response.Create()
                  .WithStatusCode(200)
                  .WithBody(JsonSerializer.Serialize(org)));

            // Act

            // Assert
        }
    }
}
