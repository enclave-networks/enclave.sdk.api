using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Enclave.Configuration.Data.Identifiers;
using Enclave.Sdk.Api.Exceptions;
using FluentAssertions;
using NUnit.Framework;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Enclave.Sdk.Api.Tests;

// The partner API is a separate service on its own host, with every route under /partner/{partnerId}/
// (portal Enclave.Partner.Api/Scaffolding/PartnerRouteAttribute.cs). Production serves it at
// https://partner-api.enclave.io and staging at https://staging-partner-api.enclave.io, so the partner
// client has its own base URL option, defaulting to production as BaseUrl does, and its own HttpClient,
// which sends the same token and User-Agent through the same handlers as the client for the main API.
public class PartnerClientTests
{
    private const string CustomerJson = "{\"id\":\"00000000000000000000000000000001\",\"name\":\"Globex Ltd\",\"status\":\"InPoC\"}";

    private static IEnumerable<TestCaseData> ClientsWithoutPartnerApiBaseUrl()
    {
        foreach (var url in new[] { null, "", " " })
        {
            yield return new TestCaseData(new Func<EnclaveClient>(() => new EnclaveClient(new EnclaveClientOptions
            {
                PersonalAccessToken = "TOKEN",
                PartnerApiBaseUrl = url,
            }))).SetArgDisplayNames(url is null ? "null" : $"\"{url}\"");
        }
    }

    // A caller that clears PartnerApiBaseUrl has asked for no partner API. Falling back to BaseUrl would send
    // partner calls to the main API, where no partner route exists, so the error names the option and the
    // caller knows what to set.
    [TestCaseSource(nameof(ClientsWithoutPartnerApiBaseUrl))]
    public void Should_throw_an_invalid_operation_exception_naming_PartnerApiBaseUrl_when_it_is_not_set(Func<EnclaveClient> createClient)
    {
        // Arrange
        var client = createClient();

        // Act
        var act = () => client.CreatePartnerClient(PartnerId.New());

        // Assert
        act.Should().Throw<InvalidOperationException>().WithMessage("*PartnerApiBaseUrl*");
    }

    // The default PartnerId is no partner, so a client made from it would send every call to a partner
    // that does not exist; it is refused where the mistake is made.
    [Test]
    public void Should_throw_an_argument_exception_when_created_from_an_empty_partner_id()
    {
        // Arrange
        var client = new EnclaveClient(new EnclaveClientOptions
        {
            PersonalAccessToken = "TOKEN",
            PartnerApiBaseUrl = "https://partner-api.enclave.invalid",
        });

        // Act
        var act = () => client.CreatePartnerClient(default(PartnerId));

        // Assert
        act.Should().Throw<ArgumentException>().WithParameterName("partnerId");
    }

    // Two servers stand in for the two hosts, so a partner call sent to BaseUrl, or an organisation call
    // sent to PartnerApiBaseUrl, reaches the wrong server and the counts show it.
    [Test]
    public async Task Should_send_partner_calls_to_the_partner_api_base_url_and_other_calls_to_the_base_url()
    {
        // Arrange
        using var apiServer = WireMockServer.Start();
        using var partnerServer = WireMockServer.Start();

        apiServer
          .Given(Request.Create().WithPath("/account/orgs").UsingGet())
          .RespondWith(Response.Create().WithStatusCode(200).WithBody("{\"orgs\":[]}"));

        partnerServer
          .Given(Request.Create().WithPath("/*").UsingGet())
          .RespondWith(Response.Create().WithStatusCode(200).WithHeader("Content-Type", "application/json").WithBody(CustomerJson));

        var client = new EnclaveClient(new EnclaveClientOptions
        {
            BaseUrl = apiServer.Urls[0],
            PartnerApiBaseUrl = partnerServer.Urls[0],
            PersonalAccessToken = "TOKEN",
        });

        var partnerId = PartnerId.New();
        var customerId = OrganisationGuid.New();

        // Act
        var partnerClient = client.CreatePartnerClient(partnerId);
        await partnerClient.Customers.GetAsync(customerId);
        await client.GetOrganisationsAsync();

        // Assert
        partnerClient.PartnerId.Should().Be(partnerId);

        var partnerRequest = partnerServer.LogEntries.Should().ContainSingle().Subject.RequestMessage;
        partnerRequest.Path.Should().Be($"/partner/{partnerId}/customers/{customerId}");
        partnerRequest.Headers["Authorization"].Should().Equal("Bearer TOKEN");
        partnerRequest.Headers["User-Agent"].Should().ContainSingle().Which.Should().StartWith("Enclave.Sdk.Api/");

        apiServer.LogEntries.Should().ContainSingle().Which.RequestMessage.Path.Should().Be("/account/orgs");
    }

    // A caller's handler (a dry run, or request logging) sees partner requests as it sees the others,
    // with the partner API's URL and the Authorization header. It answers here, and both base URLs use the
    // reserved .invalid domain (RFC 2606), which resolvers report as nonexistent (RFC 6761 section 6.4),
    // so a request that bypassed the handler would fail.
    [Test]
    public async Task Should_send_partner_calls_through_the_given_handler_with_the_authorization_header()
    {
        // Arrange
        var handler = new RecordingHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(CustomerJson, Encoding.UTF8, "application/json"),
        });

        var client = new EnclaveClient(new EnclaveClientOptions
        {
            BaseUrl = "https://api.enclave.invalid",
            PartnerApiBaseUrl = "https://partner-api.enclave.invalid",
            PersonalAccessToken = "TOKEN",
            HttpMessageHandler = handler,
        });

        var partnerId = PartnerId.New();
        var customerId = OrganisationGuid.New();

        // Act
        var result = await client.CreatePartnerClient(partnerId).Customers.GetAsync(customerId);

        // Assert
        result.Name.Should().Be("Globex Ltd");
        var request = handler.Requests.Should().ContainSingle().Subject;
        request.Method.Should().Be(HttpMethod.Get);
        request.Uri.Should().Be(new Uri($"https://partner-api.enclave.invalid/partner/{partnerId}/customers/{customerId}"));
        request.Authorization.Should().Be(new AuthenticationHeaderValue("Bearer", "TOKEN"));
    }

    // Options left as they are send partner calls to the production partner API, as they send other calls
    // to the production API. The handler answers, so nothing reaches the network.
    [Test]
    public async Task Should_send_partner_calls_to_the_production_partner_api_by_default()
    {
        // Arrange
        var handler = new RecordingHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(CustomerJson, Encoding.UTF8, "application/json"),
        });

        var client = new EnclaveClient(new EnclaveClientOptions
        {
            PersonalAccessToken = "TOKEN",
            HttpMessageHandler = handler,
        });

        var partnerId = PartnerId.New();
        var customerId = OrganisationGuid.New();

        // Act
        await client.CreatePartnerClient(partnerId).Customers.GetAsync(customerId);

        // Assert
        handler.Requests.Should().ContainSingle().Which.Uri
            .Should().Be(new Uri($"https://partner-api.enclave.io/partner/{partnerId}/customers/{customerId}"));
    }

    // A credentials file written before the partner API existed holds no partnerApiBaseUrl, and still gives
    // the production partner API, as it gives the production API when it holds no baseUrl.
    [Test]
    public void Should_use_the_production_partner_api_when_the_credentials_file_does_not_name_one()
    {
        // Arrange
        var path = Path.Combine(Path.GetTempPath(), "enclave-sdk-api-tests", Guid.NewGuid().ToString("N"), "credentials.json");
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        File.WriteAllText(path, "{\"personalAccessToken\": \"TOKEN\"}");

        try
        {
            // Act
            var options = EnclaveClient.ReadCredentialsFile(path);

            // Assert
            options.PartnerApiBaseUrl.Should().Be("https://partner-api.enclave.io");
        }
        finally
        {
            Directory.Delete(Path.GetDirectoryName(path), recursive: true);
        }
    }

    // The given handler takes the place of the network beneath the problem details handler on the partner
    // client too, so a problem+json response from it still throws EnclaveApiException.
    [Test]
    public async Task Should_throw_an_enclave_api_exception_for_a_problem_response_to_a_partner_call_from_the_given_handler()
    {
        // Arrange
        var handler = new RecordingHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = new StringContent("{\"title\":\"Customer cannot be found\",\"status\":404}", Encoding.UTF8, "application/problem+json"),
        });

        var client = new EnclaveClient(new EnclaveClientOptions
        {
            BaseUrl = "https://api.enclave.invalid",
            PartnerApiBaseUrl = "https://partner-api.enclave.invalid",
            PersonalAccessToken = "TOKEN",
            HttpMessageHandler = handler,
        });

        // Act
        var act = () => client.CreatePartnerClient(PartnerId.New()).Customers.GetAsync(OrganisationGuid.New());

        // Assert
        (await act.Should().ThrowAsync<EnclaveApiException>()).Which.ProblemDetails.Title.Should().Be("Customer cannot be found");
        handler.Requests.Should().ContainSingle();
    }

    // The parameterless EnclaveClient constructor reads its options from the credentials file, so the
    // partner API's URL can be saved there beside the token.
    [Test]
    public void Should_read_the_partner_api_base_url_from_the_credentials_file()
    {
        // Arrange
        var path = Path.Combine(Path.GetTempPath(), "enclave-sdk-api-tests", Guid.NewGuid().ToString("N"), "credentials.json");
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        File.WriteAllText(path, "{\"personalAccessToken\": \"TOKEN\", \"partnerApiBaseUrl\": \"https://partner-api.example\"}");

        try
        {
            // Act
            var options = EnclaveClient.ReadCredentialsFile(path);

            // Assert
            options.PartnerApiBaseUrl.Should().Be("https://partner-api.example");
        }
        finally
        {
            Directory.Delete(Path.GetDirectoryName(path), recursive: true);
        }
    }
}
