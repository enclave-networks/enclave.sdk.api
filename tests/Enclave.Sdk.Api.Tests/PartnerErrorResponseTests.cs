using System.Globalization;
using System.Net;
using Enclave.Configuration.Data.Identifiers;
using Enclave.Sdk.Api.Clients.Interfaces;
using Enclave.Sdk.Api.Exceptions;
using Enclave.Sdk.Api.Partner.Models;
using FluentAssertions;
using NUnit.Framework;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Enclave.Sdk.Api.Tests;

// Every partner API call, sent through an EnclaveClient so the handlers a caller gets are in place, to a
// partner API host that answers every request with the same failure. The partner calls have their own
// list, beside ErrorResponseTests, because they need a partner API base URL and a partner ID. A call
// that ignored the response status would report the failure as success, or fail later with an error that
// has no status code, so each call is listed here.
public class PartnerErrorResponseTests
{
    private static readonly OrganisationGuid CustomerId = OrganisationGuid.New();

    private static readonly (string Name, Func<ICustomersClient, Task> Call)[] Calls =
    {
        ("Customers.GetCustomersAsync", customers => customers.GetCustomersAsync()),
        ("Customers.GetAsync", customers => customers.GetAsync(CustomerId)),
        ("Customers.CreateAsync", customers => customers.CreateAsync(new CustomerCreateModel { Name = "name", InitialSystemsCount = 2 })),
        ("Customers.Update", customers => customers.Update(CustomerId).Set(c => c.Name, "name").ApplyAsync()),
        ("Customers.ConvertAsync", customers => customers.ConvertAsync(CustomerId, 12)),
        ("Customers.GetAdminsAsync", customers => customers.GetAdminsAsync(CustomerId)),
        ("Customers.AddAdminAsync", customers => customers.AddAdminAsync(CustomerId, AccountGuid.New())),
        ("Customers.RemoveAdminAsync", customers => customers.RemoveAdminAsync(CustomerId, AccountGuid.New())),
        ("Customers.GetPendingInvitesAsync", customers => customers.GetPendingInvitesAsync(CustomerId)),
        ("Customers.InviteAdminAsync", customers => customers.InviteAdminAsync(CustomerId, "user@example.com")),
        ("Customers.CancelInviteAsync", customers => customers.CancelInviteAsync(CustomerId, OrganisationInviteId.FromString($"{CustomerId}-1"))),
        ("Customers.EnableAutoSyncAsync", customers => customers.EnableAutoSyncAsync(CustomerId)),
        ("Customers.DisableAutoSyncAsync", customers => customers.DisableAutoSyncAsync(CustomerId)),
    };

    private WireMockServer _server;
    private ICustomersClient _customers;

    [SetUp]
    public void Setup()
    {
        _server = WireMockServer.Start();

        var client = new EnclaveClient(new EnclaveClientOptions
        {
            BaseUrl = "https://api.enclave.invalid",
            PartnerApiBaseUrl = _server.Urls[0],
            PersonalAccessToken = "TOKEN",
        });

        _customers = client.CreatePartnerClient(PartnerId.New()).Customers;
    }

    [TearDown]
    public void TearDown()
    {
        _server.Stop();
    }

    // A 502 from a proxy in front of the API, with an HTML page, and a 404 with no body, as a server gives
    // for a route it does not have. Neither is problem+json, so it is the client, not the problem details
    // handler, that turns them into an exception.
    private static IEnumerable<TestCaseData> PlainErrorCases() =>
        from call in Calls
        from status in new[] { HttpStatusCode.BadGateway, HttpStatusCode.NotFound }
        select new TestCaseData(call.Call, status).SetArgDisplayNames(call.Name, ((int)status).ToString(CultureInfo.InvariantCulture));

    private static IEnumerable<TestCaseData> AllCalls() =>
        Calls.Select(call => new TestCaseData(call.Call).SetArgDisplayNames(call.Name));

    // A failure that is not problem+json throws HttpRequestException carrying the status code, as
    // HttpResponseMessage.EnsureSuccessStatusCode gives, so a caller can tell a failed call from a
    // successful one and act on the status: retry a 502, report a 404.
    [TestCaseSource(nameof(PlainErrorCases))]
    public async Task Should_throw_an_http_request_exception_with_the_status_code_for_a_failure_that_is_not_problem_json(Func<ICustomersClient, Task> call, HttpStatusCode status)
    {
        // Arrange
        var response = Response.Create().WithStatusCode(status);

        if (status == HttpStatusCode.BadGateway)
        {
            response = response
                .WithHeader("Content-Type", "text/html")
                .WithBody("<html><body><h1>502 Bad Gateway</h1></body></html>");
        }

        _server.Given(Request.Create().WithPath("/*").UsingAnyMethod()).RespondWith(response);

        // Act
        var act = () => call(_customers);

        // Assert
        (await act.Should().ThrowAsync<HttpRequestException>()).Which.StatusCode.Should().Be(status);
        _server.LogEntries.Should().ContainSingle();
    }

    // A problem+json failure carries the API's own account of what went wrong, as the partner API gives for
    // a customer it cannot find (portal CustomersController.cs, CustomerNotFoundResult), so it throws
    // EnclaveApiException with those details, whatever the call does with other failures.
    [TestCaseSource(nameof(AllCalls))]
    public async Task Should_throw_an_enclave_api_exception_for_a_problem_json_failure(Func<ICustomersClient, Task> call)
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath("/*").UsingAnyMethod())
          .RespondWith(
            Response.Create()
              .WithStatusCode(HttpStatusCode.NotFound)
              .WithHeader("Content-Type", "application/problem+json")
              .WithBody("{\"title\":\"Customer cannot be found\",\"status\":404}"));

        // Act
        var act = () => call(_customers);

        // Assert
        (await act.Should().ThrowAsync<EnclaveApiException>()).Which.ProblemDetails.Title.Should().Be("Customer cannot be found");
    }
}
