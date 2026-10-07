using System.Globalization;
using System.Net;
using Enclave.Api.Modules.SystemManagement.Authority;
using Enclave.Api.Modules.SystemManagement.Dns.Models;
using Enclave.Api.Modules.SystemManagement.EnrolmentKeys.Models;
using Enclave.Api.Modules.SystemManagement.Policies.Models;
using Enclave.Api.Modules.SystemManagement.Tags.Models;
using Enclave.Api.Modules.SystemManagement.TrustRequirements.Models;
using Enclave.Configuration.Data.Enums;
using Enclave.Configuration.Data.Identifiers;
using Enclave.Sdk.Api.Clients.Interfaces;
using Enclave.Sdk.Api.Exceptions;
using Enclave.Sdk.Network.Abstractions.NetworkPolicy;
using FluentAssertions;
using NUnit.Framework;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Enclave.Sdk.Api.Tests;

// Every API call on every client, sent through an EnclaveClient so the handlers a caller gets are in place,
// to a server that answers every request with the same failure. A call that ignored the response status
// would report the failure as success, or fail later with an error that has no status code, so each call
// is listed here rather than only the ones known to check the status.
public class ErrorResponseTests
{
    private static readonly OrganisationGuid OrgId = OrganisationGuid.New();

    private static readonly (string Name, Func<EnclaveClient, Task> Call)[] Calls =
    {
        ("GetOrganisationsAsync", client => client.GetOrganisationsAsync()),
        ("Authority.EnrolAsync", client => client.CreateAuthorityClient().EnrolAsync(new EnrolRequestModel { EnrolmentKey = "key", Nonce = "nonce", PublicKey = "" })),

        ("Organisation.GetAsync", client => Org(client).GetAsync()),
        ("Organisation.Update", client => Org(client).Update().Set(o => o.Website, "website").ApplyAsync()),
        ("Organisation.GetOrganisationUsersAsync", client => Org(client).GetOrganisationUsersAsync()),
        ("Organisation.RemoveUserAsync", client => Org(client).RemoveUserAsync("account")),
        ("Organisation.GetPendingInvitesAsync", client => Org(client).GetPendingInvitesAsync()),
        ("Organisation.InviteUserAsync", client => Org(client).InviteUserAsync("user@example.com")),
        ("Organisation.CancelInviteAync", client => Org(client).CancelInviteAync("user@example.com")),

        ("Dns.GetPropertiesSummaryAsync", client => Org(client).Dns.GetPropertiesSummaryAsync()),
        ("Dns.GetZonesAsync", client => Org(client).Dns.GetZonesAsync()),
        ("Dns.CreateZoneAsync", client => Org(client).Dns.CreateZoneAsync(new DnsZoneCreateModel())),
        ("Dns.GetZoneAsync", client => Org(client).Dns.GetZoneAsync(DnsZoneId.FromInt(1))),
        ("Dns.UpdateZone", client => Org(client).Dns.UpdateZone(DnsZoneId.FromInt(1)).Set(z => z.Name, "name").ApplyAsync()),
        ("Dns.DeleteZoneAsync", client => Org(client).Dns.DeleteZoneAsync(DnsZoneId.FromInt(1))),
        ("Dns.GetRecordsAsync", client => Org(client).Dns.GetRecordsAsync()),
        ("Dns.CreateRecordAsync", client => Org(client).Dns.CreateRecordAsync(new DnsRecordCreateModel { Name = "name" })),
        ("Dns.DeleteRecordsAsync", client => Org(client).Dns.DeleteRecordsAsync(DnsRecordId.FromInt(1))),
        ("Dns.GetRecordAsync", client => Org(client).Dns.GetRecordAsync(DnsRecordId.FromInt(1))),
        ("Dns.UpdateRecord", client => Org(client).Dns.UpdateRecord(DnsRecordId.FromInt(1)).Set(r => r.Name, "name").ApplyAsync()),
        ("Dns.DeleteRecordAsync", client => Org(client).Dns.DeleteRecordAsync(DnsRecordId.FromInt(1))),

        ("EnrolmentKeys.GetEnrolmentKeysAsync", client => Org(client).EnrolmentKeys.GetEnrolmentKeysAsync()),
        ("EnrolmentKeys.CreateAsync", client => Org(client).EnrolmentKeys.CreateAsync(new EnrolmentKeyCreateModel())),
        ("EnrolmentKeys.GetAsync", client => Org(client).EnrolmentKeys.GetAsync(EnrolmentKeyId.FromInt(1))),
        ("EnrolmentKeys.Update", client => Org(client).EnrolmentKeys.Update(EnrolmentKeyId.FromInt(1)).Set(k => k.Description, "description").ApplyAsync()),
        ("EnrolmentKeys.EnableAsync", client => Org(client).EnrolmentKeys.EnableAsync(EnrolmentKeyId.FromInt(1))),
        ("EnrolmentKeys.DisableAsync", client => Org(client).EnrolmentKeys.DisableAsync(EnrolmentKeyId.FromInt(1))),
        ("EnrolmentKeys.BulkEnableAsync", client => Org(client).EnrolmentKeys.BulkEnableAsync(EnrolmentKeyId.FromInt(1))),
        ("EnrolmentKeys.BulkDisableAsync", client => Org(client).EnrolmentKeys.BulkDisableAsync(EnrolmentKeyId.FromInt(1))),
        ("EnrolmentKeys.EnableUntilAsync", client => Org(client).EnrolmentKeys.EnableUntilAsync(EnrolmentKeyId.FromInt(1), DateTimeOffset.UtcNow.AddHours(1), ExpiryAction.Disable)),
        ("EnrolmentKeys.DeleteAsync", client => Org(client).EnrolmentKeys.DeleteAsync(EnrolmentKeyId.FromInt(1))),
        ("EnrolmentKeys.BulkDeleteAsync", client => Org(client).EnrolmentKeys.BulkDeleteAsync(EnrolmentKeyId.FromInt(1))),
        ("EnrolmentKeys.GetSearchKeysAsync", client => Org(client).EnrolmentKeys.GetSearchKeysAsync()),

        ("Logs.GetLogsAsync", client => Org(client).Logs.GetLogsAsync()),

        ("Policies.GetPoliciesAsync", client => Org(client).Policies.GetPoliciesAsync()),
        ("Policies.CreateAsync", client => Org(client).Policies.CreateAsync(new PolicyCreateModel())),
        ("Policies.DeletePoliciesAsync", client => Org(client).Policies.DeletePoliciesAsync(PolicyId.FromInt(1))),
        ("Policies.GetAsync", client => Org(client).Policies.GetAsync(PolicyId.FromInt(1))),
        ("Policies.Update", client => Org(client).Policies.Update(PolicyId.FromInt(1)).Set(p => p.Description, "description").ApplyAsync()),
        ("Policies.DeleteAsync", client => Org(client).Policies.DeleteAsync(PolicyId.FromInt(1))),
        ("Policies.EnableAsync", client => Org(client).Policies.EnableAsync(PolicyId.FromInt(1))),
        ("Policies.DisableAsync", client => Org(client).Policies.DisableAsync(PolicyId.FromInt(1))),
        ("Policies.EnablePoliciesAsync", client => Org(client).Policies.EnablePoliciesAsync(PolicyId.FromInt(1))),
        ("Policies.DisablePoliciesAsync", client => Org(client).Policies.DisablePoliciesAsync(PolicyId.FromInt(1))),
        ("Policies.EnableUntilAsync", client => Org(client).Policies.EnableUntilAsync(PolicyId.FromInt(1), DateTimeOffset.UtcNow.AddHours(1), ExpiryAction.Disable)),
        ("Policies.GetSearchKeysAsync", client => Org(client).Policies.GetSearchKeysAsync()),

        ("EnrolledSystems.GetSystemsAsync", client => Org(client).EnrolledSystems.GetSystemsAsync()),
        ("EnrolledSystems.RevokeSystemsAsync", client => Org(client).EnrolledSystems.RevokeSystemsAsync("system")),
        ("EnrolledSystems.GetAsync", client => Org(client).EnrolledSystems.GetAsync("system")),
        ("EnrolledSystems.Update", client => Org(client).EnrolledSystems.Update("system").Set(s => s.Description, "description").ApplyAsync()),
        ("EnrolledSystems.RevokeAsync", client => Org(client).EnrolledSystems.RevokeAsync("system")),
        ("EnrolledSystems.EnableAsync", client => Org(client).EnrolledSystems.EnableAsync("system")),
        ("EnrolledSystems.DisableAsync", client => Org(client).EnrolledSystems.DisableAsync("system")),
        ("EnrolledSystems.BulkEnableAsync", client => Org(client).EnrolledSystems.BulkEnableAsync("system")),
        ("EnrolledSystems.BulkDisableAsync", client => Org(client).EnrolledSystems.BulkDisableAsync("system")),
        ("EnrolledSystems.EnableUntilAsync", client => Org(client).EnrolledSystems.EnableUntilAsync("system", DateTimeOffset.UtcNow.AddHours(1), ExpiryAction.Disable)),
        ("EnrolledSystems.GetSearchKeysAsync", client => Org(client).EnrolledSystems.GetSearchKeysAsync()),

        ("Tags.GetAsync", client => Org(client).Tags.GetAsync()),
        ("Tags.CreateAsync", client => Org(client).Tags.CreateAsync(new TagCreateModel())),
        ("Tags.DeleteTagsAsync", client => Org(client).Tags.DeleteTagsAsync("tag")),
        ("Tags.GetAsync(tag)", client => Org(client).Tags.GetAsync("tag")),
        ("Tags.Update", client => Org(client).Tags.Update("tag").Set(t => t.Notes, "notes").ApplyAsync()),
        ("Tags.DeleteAsync", client => Org(client).Tags.DeleteAsync("tag")),
        ("Tags.GetSearchKeysAsync", client => Org(client).Tags.GetSearchKeysAsync()),

        ("TrustRequirements.GetTrustRequirementsAsync", client => Org(client).TrustRequirements.GetTrustRequirementsAsync()),
        ("TrustRequirements.CreateAsync", client => Org(client).TrustRequirements.CreateAsync(new TrustRequirementCreateModel(
            "description",
            TrustRequirementType.PublicIp,
            null,
            new TrustRequirementSettingsModel(new Dictionary<string, string>(), new List<IReadOnlyDictionary<string, string>>())))),
        ("TrustRequirements.DeleteTrustRequirementsAsync", client => Org(client).TrustRequirements.DeleteTrustRequirementsAsync(TrustRequirementId.FromInt(1))),
        ("TrustRequirements.GetAsync", client => Org(client).TrustRequirements.GetAsync(TrustRequirementId.FromInt(1))),
        ("TrustRequirements.Update", client => Org(client).TrustRequirements.Update(TrustRequirementId.FromInt(1)).Set(t => t.Description, "description").ApplyAsync()),
        ("TrustRequirements.DeleteAsync", client => Org(client).TrustRequirements.DeleteAsync(TrustRequirementId.FromInt(1))),

        ("UnapprovedSystems.GetSystemsAsync", client => Org(client).UnapprovedSystems.GetSystemsAsync()),
        ("UnapprovedSystems.DeclineSystems", client => Org(client).UnapprovedSystems.DeclineSystems("system")),
        ("UnapprovedSystems.GetAsync", client => Org(client).UnapprovedSystems.GetAsync("system")),
        ("UnapprovedSystems.Update", client => Org(client).UnapprovedSystems.Update("system").Set(u => u.Description, "description").ApplyAsync()),
        ("UnapprovedSystems.DeclineAsync", client => Org(client).UnapprovedSystems.DeclineAsync("system")),
        ("UnapprovedSystems.ApproveAsync", client => Org(client).UnapprovedSystems.ApproveAsync("system")),
        ("UnapprovedSystems.ApproveSystemsAsync", client => Org(client).UnapprovedSystems.ApproveSystemsAsync("system")),
        ("UnapprovedSystems.GetSearchKeysAsync", client => Org(client).UnapprovedSystems.GetSearchKeysAsync()),
    };

    private WireMockServer _server;
    private EnclaveClient _client;

    [SetUp]
    public void Setup()
    {
        _server = WireMockServer.Start();

        _client = new EnclaveClient(new EnclaveClientOptions
        {
            BaseUrl = _server.Urls[0],
            PersonalAccessToken = "TOKEN",
        });
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
    public async Task Should_throw_an_http_request_exception_with_the_status_code_for_a_failure_that_is_not_problem_json(Func<EnclaveClient, Task> call, HttpStatusCode status)
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
        var act = () => call(_client);

        // Assert
        (await act.Should().ThrowAsync<HttpRequestException>()).Which.StatusCode.Should().Be(status);
        _server.LogEntries.Should().ContainSingle();
    }

    // A problem+json failure carries the API's own account of what went wrong, so it throws
    // EnclaveApiException with those details, whatever the call does with other failures.
    [TestCaseSource(nameof(AllCalls))]
    public async Task Should_throw_an_enclave_api_exception_for_a_problem_json_failure(Func<EnclaveClient, Task> call)
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath("/*").UsingAnyMethod())
          .RespondWith(
            Response.Create()
              .WithStatusCode(HttpStatusCode.NotFound)
              .WithHeader("Content-Type", "application/problem+json")
              .WithBody("{\"title\":\"Not Found\",\"status\":404}"));

        // Act
        var act = () => call(_client);

        // Assert
        (await act.Should().ThrowAsync<EnclaveApiException>()).Which.ProblemDetails.Title.Should().Be("Not Found");
    }

    private static IOrganisationScopedClient Org(EnclaveClient client) => client.CreateOrganisationClient(OrgId);
}
