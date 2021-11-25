using System.Text.Json;
using Enclave.Sdk.Api.Data;
using Enclave.Sdk.Api.Data.Account;
using FluentAssertions;
using NUnit.Framework;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Enclave.Sdk.Api.Tests;

public class EnclaveClientTests
{
    private EnclaveClient _client;
    private WireMockServer _server;

    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    [SetUp]
    public void Setup()
    {
        _server = WireMockServer.Start();

        var enclaveSettings = new EnclaveClientOptions
        {
            BaseUrl = _server.Urls[0],
        };

        _client = new EnclaveClient(enclaveSettings);
    }

    [Test]
    public async Task Should_return_list_of_orgs_when_calling_GetOrganisationsAsync()
    {
        // Arrange
        var accountOrg = new AccountOrganisationTopLevel
        {
            Orgs = new List<AccountOrganisation>
            {
                new AccountOrganisation
                {
                    OrgId = OrganisationId.New(),
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
              .WithBody(JsonSerializer.Serialize(accountOrg, _serializerOptions)));

        // Act
        var result = await _client.GetOrganisationsAsync();

        // Assert
        result.FirstOrDefault().OrgId.Should().Be(accountOrg.Orgs.FirstOrDefault().OrgId);
    }

    [Test]
    public async Task Should_throw_invalid_operation_exception_if_response_does_not_contain_an_organisation_model()
    {
        // Arrange
        _server
           .Given(Request.Create().WithPath("/account/orgs").UsingGet())
           .RespondWith(
             Response.Create()
               .WithStatusCode(200)
               .WithBody("null"));

        // Assert
        await _client.Invoking(c => c.GetOrganisationsAsync()).Should().ThrowAsync<InvalidOperationException>();
    }
}