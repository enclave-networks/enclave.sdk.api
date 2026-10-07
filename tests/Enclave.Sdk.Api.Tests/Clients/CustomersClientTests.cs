using System.Net;
using System.Text.Json;
using Enclave.Configuration.Data.Enums;
using Enclave.Configuration.Data.Identifiers;
using Enclave.Configuration.Data.Modules.Organisation.Enums;
using Enclave.Sdk.Api.Clients.Interfaces;
using Enclave.Sdk.Api.Partner.Models;
using FluentAssertions;
using NUnit.Framework;
using WireMock;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Enclave.Sdk.Api.Tests.Clients;

// Each customer call goes through an EnclaveClient, so the partner HttpClient it builds is the one under
// test. The server answers every request, and each test checks the method, path, query and body the
// server received, so a call sent to the wrong route fails on those checks rather than on a missing
// mapping. BaseUrl is a reserved .invalid name (RFC 2606), so a call sent to the main API fails.
//
// Response bodies are written out as the partner API writes them: camelCase names, enums by name, and
// typed IDs as 32 hex digits (portal Enclave.Api.Scaffolding/CommonWebStartup.cs:91-99, AddJsonOptions).
// They are not serialised from this package's models, so a model whose names or types differ from the
// API's fails to read.
public class CustomersClientTests
{
    private WireMockServer _server;
    private ICustomersClient _customers;
    private PartnerId _partnerId;
    private OrganisationGuid _customerId;
    private string _customerPath;

    [SetUp]
    public void Setup()
    {
        _server = WireMockServer.Start();
        _partnerId = PartnerId.New();
        _customerId = OrganisationGuid.New();
        _customerPath = $"/partner/{_partnerId}/customers/{_customerId}";

        _customers = CreateClient(_server.Urls[0]).CreatePartnerClient(_partnerId).Customers;
    }

    [TearDown]
    public void TearDown()
    {
        _server.Stop();
    }

    [Test]
    public async Task Should_get_the_partners_customers_from_the_customers_route()
    {
        // Arrange
        Respond($$"""
            {
              "metadata": { "total": 1, "firstPage": 0, "prevPage": null, "lastPage": 0, "nextPage": null },
              "links": { "first": "https://partner-api.example/first", "prev": null, "next": null, "last": "https://partner-api.example/last" },
              "items": [ {{CustomerJson(_customerId)}} ]
            }
            """);

        // Act
        var result = await _customers.GetCustomersAsync();

        // Assert
        var request = SingleRequest();
        request.Method.Should().Be("GET");
        request.Path.Should().Be($"/partner/{_partnerId}/customers");
        request.RawQuery.Should().BeNullOrEmpty();

        result.Metadata.Total.Should().Be(1);
        (await result.Items.ToListAsync()).Should().ContainSingle().Which.Should().BeEquivalentTo(ExpectedCustomer(_customerId));
    }

    // The names are those the API binds (portal CustomerRequestModel.cs and PaginatedRequestModel.cs):
    // "sort" and "per_page" differ from the property names, and "CanAdminOnly" is bound by that name.
    // The search term holds characters that must be escaped in a query string.
    [Test]
    public async Task Should_send_every_customer_list_filter_as_the_query_parameter_the_api_reads()
    {
        // Arrange
        Respond("""{ "metadata": { "total": 0, "firstPage": 0, "lastPage": 0 }, "links": {}, "items": [] }""");

        // Act
        await _customers.GetCustomersAsync(
            searchTerm: "Globex & Co",
            sortOrder: PartnerOrganisationSortOrder.UnusedBillableSystems,
            canAdminOnly: true,
            year: 2026,
            month: 9,
            day: 15,
            pageNumber: 2,
            perPage: 10);

        // Assert
        var request = SingleRequest();
        request.Path.Should().Be($"/partner/{_partnerId}/customers");
        request.Query.ToDictionary(q => q.Key, q => string.Join(",", q.Value)).Should().BeEquivalentTo(new Dictionary<string, string>
        {
            ["search"] = "Globex & Co",
            ["sort"] = "UnusedBillableSystems",
            ["CanAdminOnly"] = "true",
            ["year"] = "2026",
            ["month"] = "9",
            ["day"] = "15",
            ["page"] = "2",
            ["per_page"] = "10",
        });
    }

    [Test]
    public async Task Should_get_a_customer_by_its_organisation_id()
    {
        // Arrange
        Respond(CustomerJson(_customerId));

        // Act
        var result = await _customers.GetAsync(_customerId);

        // Assert
        var request = SingleRequest();
        request.Method.Should().Be("GET");
        request.Path.Should().Be(_customerPath);

        result.Should().BeEquivalentTo(ExpectedCustomer(_customerId));
    }

    // Every field of the API's CustomerCreateModel (portal CustomerCreateModel.cs) is given a value other
    // than its default, so a field left out of the body, or sent under another name, fails the check.
    [Test]
    public async Task Should_post_every_field_of_the_create_model_and_read_the_created_customer()
    {
        // Arrange
        Respond(CustomerJson(_customerId));

        var createModel = new CustomerCreateModel
        {
            Name = "Initech",
            OwnerEmail = "it@initech.example",
            Domain = "initech.example",
            IndustryDiscount = true,
            InitialSystemsCount = 20,
            InitialGatewaysCount = 1,
            ContactName = "Bill Lumbergh",
            HardLimit = true,
            AdminAutoSyncIsEnabled = true,
        };

        // Act
        var result = await _customers.CreateAsync(createModel);

        // Assert
        var request = SingleRequest();
        request.Method.Should().Be("POST");
        request.Path.Should().Be($"/partner/{_partnerId}/customers");
        JsonBody(request).Should().BeEquivalentTo(new Dictionary<string, string>
        {
            ["name"] = "\"Initech\"",
            ["ownerEmail"] = "\"it@initech.example\"",
            ["domain"] = "\"initech.example\"",
            ["industryDiscount"] = "true",
            ["initialSystemsCount"] = "20",
            ["initialGatewaysCount"] = "1",
            ["contactName"] = "\"Bill Lumbergh\"",
            ["hardLimit"] = "true",
            ["adminAutoSyncIsEnabled"] = "true",
        });

        result.Should().BeEquivalentTo(ExpectedCustomer(_customerId));
    }

    // A patch sends only the fields set, and a field set to null as JSON null, which the API applies as
    // "clear this field" (portal Enclave.Api.Scaffolding PatchModel.WasSet). The API reads property names
    // ignoring case (ASP.NET Core's JsonSerializerDefaults.Web), so the names are compared in camel case.
    [Test]
    public async Task Should_patch_the_customer_with_only_the_fields_set()
    {
        // Arrange
        Respond(CustomerJson(_customerId));

        // Act
        var result = await _customers.Update(_customerId)
            .Set(c => c.LicensedAgentsCount, 20)
            .Set(c => c.ContactName, null)
            .ApplyAsync();

        // Assert
        var request = SingleRequest();
        request.Method.Should().Be("PATCH");
        request.Path.Should().Be(_customerPath);
        JsonBody(request).ToDictionary(p => JsonNamingPolicy.CamelCase.ConvertName(p.Key), p => p.Value).Should().BeEquivalentTo(new Dictionary<string, string>
        {
            ["licensedAgentsCount"] = "20",
            ["contactName"] = "null",
        });

        result.Should().BeEquivalentTo(ExpectedCustomer(_customerId));
    }

    [Test]
    public async Task Should_put_the_billing_period_to_the_convert_route()
    {
        // Arrange
        Respond(CustomerJson(_customerId));

        // Act
        var result = await _customers.ConvertAsync(_customerId, 12);

        // Assert
        var request = SingleRequest();
        request.Method.Should().Be("PUT");
        request.Path.Should().Be($"{_customerPath}/convert");
        JsonBody(request).Should().BeEquivalentTo(new Dictionary<string, string>
        {
            ["billingPeriodMonths"] = "12",
        });

        result.Should().BeEquivalentTo(ExpectedCustomer(_customerId));
    }

    [Test]
    public async Task Should_get_the_customer_admins()
    {
        // Arrange
        var accountId = AccountGuid.New();
        Respond($$"""{ "users": [ {{AdminJson(accountId, "Owner")}} ] }""");

        // Act
        var result = await _customers.GetAdminsAsync(_customerId);

        // Assert
        var request = SingleRequest();
        request.Method.Should().Be("GET");
        request.Path.Should().Be($"{_customerPath}/admins");

        result.Should().ContainSingle().Which.Should().BeEquivalentTo(ExpectedAdmin(accountId, UserOrganisationRole.Owner));
    }

    // The route takes the account in the path and has no body (portal CustomersController.cs, AddCustomerAdmins).
    [Test]
    public async Task Should_put_the_account_to_the_customer_admins_route()
    {
        // Arrange
        var accountId = AccountGuid.New();
        Respond(AdminJson(accountId, "Admin"));

        // Act
        var result = await _customers.AddAdminAsync(_customerId, accountId);

        // Assert
        var request = SingleRequest();
        request.Method.Should().Be("PUT");
        request.Path.Should().Be($"{_customerPath}/admins/{accountId}");
        request.Body.Should().BeNullOrEmpty();

        result.Should().BeEquivalentTo(ExpectedAdmin(accountId, UserOrganisationRole.Admin));
    }

    [Test]
    public async Task Should_delete_the_account_from_the_customer_admins_route()
    {
        // Arrange
        var accountId = AccountGuid.New();
        Respond(AdminJson(accountId, "Admin"));

        // Act
        var result = await _customers.RemoveAdminAsync(_customerId, accountId);

        // Assert
        var request = SingleRequest();
        request.Method.Should().Be("DELETE");
        request.Path.Should().Be($"{_customerPath}/admins/{accountId}");

        result.Should().BeEquivalentTo(ExpectedAdmin(accountId, UserOrganisationRole.Admin));
    }

    // An invite ID is the organisation ID and a number (portal OrganisationInviteId.Create), and the
    // API writes it as a string.
    [Test]
    public async Task Should_get_the_customer_pending_invites()
    {
        // Arrange
        Respond($$"""{ "invites": [ { "id": "{{_customerId}}-3", "emailAddress": "sam@globex.example" } ] }""");

        // Act
        var result = await _customers.GetPendingInvitesAsync(_customerId);

        // Assert
        var request = SingleRequest();
        request.Method.Should().Be("GET");
        request.Path.Should().Be($"{_customerPath}/invites");

        var invite = result.Should().ContainSingle().Subject;
        invite.Id.Should().Be(OrganisationInviteId.FromString($"{_customerId}-3"));
        invite.EmailAddress.Should().Be("sam@globex.example");
    }

    [Test]
    public async Task Should_post_the_email_address_to_the_customer_invites_route()
    {
        // Arrange
        Respond($$"""{ "id": "{{_customerId}}-4", "emailAddress": "sam@globex.example" }""");

        // Act
        var result = await _customers.InviteAdminAsync(_customerId, "sam@globex.example");

        // Assert
        var request = SingleRequest();
        request.Method.Should().Be("POST");
        request.Path.Should().Be($"{_customerPath}/invites");
        JsonBody(request).Should().BeEquivalentTo(new Dictionary<string, string>
        {
            ["emailAddress"] = "\"sam@globex.example\"",
        });

        result.Id.Should().Be(OrganisationInviteId.FromString($"{_customerId}-4"));
        result.EmailAddress.Should().Be("sam@globex.example");
    }

    // The API's response to a cancelled invite carries the email address and writes the ID as null (portal
    // DeleteCustomerPendingAdminInviteHandler.cs leaves it unset). A default OrganisationInviteId throws
    // NullReferenceException from Equals and GetHashCode, so a model holding one breaks any comparison. The
    // API deletes the invite with the ID in the route (portal OrganisationRepository.DeleteOrganisationInviteAsync),
    // so the returned invite carries that ID. The IDs are compared as text, which reports a mismatch without
    // calling Equals on a default ID.
    [Test]
    public async Task Should_delete_the_invite_from_the_customer_invites_route_and_return_it_with_its_id()
    {
        // Arrange
        var inviteId = OrganisationInviteId.FromString($"{_customerId}-3");
        Respond("""{ "id": null, "emailAddress": "sam@globex.example" }""");

        // Act
        var result = await _customers.CancelInviteAsync(_customerId, inviteId);

        // Assert
        var request = SingleRequest();
        request.Method.Should().Be("DELETE");
        request.Path.Should().Be($"{_customerPath}/invites/{_customerId}-3");

        result.Id.ToString().Should().Be(inviteId.ToString());
        result.EmailAddress.Should().Be("sam@globex.example");
    }

    // An ID the API does return is the one the model reads.
    [Test]
    public async Task Should_read_the_invite_id_from_the_response_when_the_api_returns_one()
    {
        // Arrange
        Respond($$"""{ "id": "{{_customerId}}-9", "emailAddress": "sam@globex.example" }""");

        // Act
        var result = await _customers.CancelInviteAsync(_customerId, OrganisationInviteId.FromString($"{_customerId}-3"));

        // Assert
        result.Id.ToString().Should().Be($"{_customerId}-9");
    }

    // Auto-sync has a route of its own for each direction, with no body (portal CustomersController.cs,
    // EnableAutoSync and DisableAutoSync).
    [TestCase("enable-auto-sync")]
    [TestCase("disable-auto-sync")]
    public async Task Should_put_to_the_auto_sync_route(string route)
    {
        // Arrange
        Respond(CustomerJson(_customerId));

        // Act
        var result = route == "enable-auto-sync"
            ? await _customers.EnableAutoSyncAsync(_customerId)
            : await _customers.DisableAutoSyncAsync(_customerId);

        // Assert
        var request = SingleRequest();
        request.Method.Should().Be("PUT");
        request.Path.Should().Be($"{_customerPath}/{route}");
        request.Body.Should().BeNullOrEmpty();

        result.Should().BeEquivalentTo(ExpectedCustomer(_customerId));
    }

    // For a customer the partner does not have, the auto-sync routes return Ok(null) (portal
    // CustomersController.cs, EnableAutoSync and DisableAutoSync; ModifyCustomerHandler returns null), which
    // ASP.NET Core sends as 204 No Content with no body
    // (https://learn.microsoft.com/aspnet/core/web-api/advanced/formatting#special-case-formatters). There is
    // no customer to return, so the call throws, naming the customer, rather than returning nothing.
    [TestCase("enable-auto-sync")]
    [TestCase("disable-auto-sync")]
    public async Task Should_throw_an_invalid_operation_exception_naming_the_customer_when_auto_sync_returns_no_content(string route)
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath("/*").UsingAnyMethod())
          .RespondWith(Response.Create().WithStatusCode(HttpStatusCode.NoContent));

        // Act
        Func<Task> act = route == "enable-auto-sync"
            ? () => _customers.EnableAutoSyncAsync(_customerId)
            : () => _customers.DisableAutoSyncAsync(_customerId);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage($"*{_customerId}*");
        SingleRequest().Path.Should().Be($"{_customerPath}/{route}");
    }

    // An invite ID is one segment of the URL path. .NET removes ".." segments when it combines a relative
    // path with the base address (RFC 3986 section 5.2.4), so the ID "../admins/<account>" left unescaped
    // would send the DELETE to the customer's admins route and remove that admin. Escaped, the whole value
    // stays one segment under invites. The request is checked as it left the client, where the escaping is
    // visible, and the server is checked for any request that reached the admins route.
    [Test]
    public async Task Should_send_an_invite_id_as_one_escaped_path_segment()
    {
        // Arrange
        Respond("""{ "id": null, "emailAddress": "sam@globex.example" }""");

        var accountId = AccountGuid.New();
        var recorder = new RecordingHttpMessageHandler(new HttpClientHandler());
        var customers = CreateClient(_server.Urls[0], recorder).CreatePartnerClient(_partnerId).Customers;

        // Act
        await customers.CancelInviteAsync(_customerId, OrganisationInviteId.FromString($"../admins/{accountId}"));

        // Assert
        var request = recorder.Requests.Should().ContainSingle().Subject;
        request.Method.Should().Be(HttpMethod.Delete);
        request.Uri.AbsolutePath.Should().Be($"{_customerPath}/invites/..%2Fadmins%2F{accountId}");
        SingleRequest().Path.Should().NotStartWith($"{_customerPath}/admins");
    }

    private static IEnumerable<TestCaseData> RejectedInviteIds() =>
        from inviteId in new[] { null, "", ".", ".." }
        select new TestCaseData(inviteId).SetArgDisplayNames(inviteId is null ? "default" : $"\"{inviteId}\"");

    // Escaping leaves "." and ".." as they are, and .NET resolves them as dot-segments, so ".." would send
    // the DELETE to the customer itself. An empty or default ID drops the segment and addresses the invite
    // list. None of these names an invite, so each is refused before a request is sent.
    [TestCaseSource(nameof(RejectedInviteIds))]
    public async Task Should_refuse_an_invite_id_that_is_not_a_path_segment_without_sending_a_request(string inviteId)
    {
        // Arrange
        var recorder = new RecordingHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));
        var customers = CreateClient(_server.Urls[0], recorder).CreatePartnerClient(_partnerId).Customers;
        var id = inviteId is null ? default : OrganisationInviteId.FromString(inviteId);

        // Act
        var act = () => customers.CancelInviteAsync(_customerId, id);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>().WithParameterName("inviteId");
        recorder.Requests.Should().BeEmpty();
    }

    private static IEnumerable<TestCaseData> RejectedArguments()
    {
        yield return new TestCaseData(new Func<ICustomersClient, Task>(c => c.CreateAsync(null)), "createModel").SetArgDisplayNames("CreateAsync(null)");

        foreach (var email in new[] { null, "", " " })
        {
            yield return new TestCaseData(new Func<ICustomersClient, Task>(c => c.InviteAdminAsync(OrganisationGuid.New(), email)), "emailAddress")
                .SetArgDisplayNames($"InviteAdminAsync({(email is null ? "null" : $"\"{email}\"")})");
        }
    }

    // A create with no model, or an invite with no email address, has nothing to send, so it is refused
    // before a request is built.
    [TestCaseSource(nameof(RejectedArguments))]
    public async Task Should_refuse_a_missing_argument_without_sending_a_request(Func<ICustomersClient, Task> call, string paramName)
    {
        // Arrange
        var recorder = new RecordingHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));
        var customers = CreateClient(_server.Urls[0], recorder).CreatePartnerClient(_partnerId).Customers;

        // Act
        var act = () => call(customers);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>().WithParameterName(paramName);
        recorder.Requests.Should().BeEmpty();
    }

    private static EnclaveClient CreateClient(string partnerApiBaseUrl, HttpMessageHandler handler = null)
    {
        return new EnclaveClient(new EnclaveClientOptions
        {
            BaseUrl = "https://api.enclave.invalid",
            PartnerApiBaseUrl = partnerApiBaseUrl,
            PersonalAccessToken = "TOKEN",
            HttpMessageHandler = handler,
        });
    }

    private void Respond(string body)
    {
        _server
          .Given(Request.Create().WithPath("/*").UsingAnyMethod())
          .RespondWith(
            Response.Create()
              .WithStatusCode(200)
              .WithHeader("Content-Type", "application/json")
              .WithBody(body));
    }

    private IRequestMessage SingleRequest()
    {
        return _server.LogEntries.Should().ContainSingle().Subject.RequestMessage;
    }

    // Each top-level property of the request body, with its value as raw JSON, so a check compares both
    // the names the API reads and the JSON types of the values.
    private static Dictionary<string, string> JsonBody(IRequestMessage request)
    {
        using var document = JsonDocument.Parse(request.Body);

        return document.RootElement.EnumerateObject().ToDictionary(p => p.Name, p => p.Value.GetRawText());
    }

    // Every field has a value other than its default, so a field the model does not read fails the check.
    private static string CustomerJson(OrganisationGuid id) => $$"""
        {
          "id": "{{id}}",
          "name": "Globex Ltd",
          "enrolledSystems": 12,
          "configuredGateways": 1,
          "licensedSystems": 20,
          "licensedGateways": 2,
          "hardLimit": 25,
          "industryDiscount": true,
          "contactName": "Alex Smith",
          "billingPeriodMonths": 12,
          "status": "PayingDiscounted",
          "trialEndDate": "2026-09-01T00:00:00+00:00",
          "hardLimitForcedOn": true,
          "isSuspended": true,
          "userHasAccess": true,
          "nextInvoiceDate": "2026-11-01T00:00:00+00:00",
          "isNfr": true,
          "adminAutoSyncIsEnabled": true,
          "oldestEnclaveVersion": "2024.10.1"
        }
        """;

    private static CustomerModel ExpectedCustomer(OrganisationGuid id) => new()
    {
        Id = id,
        Name = "Globex Ltd",
        EnrolledSystems = 12,
        ConfiguredGateways = 1,
        LicensedSystems = 20,
        LicensedGateways = 2,
        HardLimit = 25,
        IndustryDiscount = true,
        ContactName = "Alex Smith",
        BillingPeriodMonths = 12,
        Status = CustomerStatusEnum.PayingDiscounted,
        TrialEndDate = new DateTimeOffset(2026, 9, 1, 0, 0, 0, TimeSpan.Zero),
        HardLimitForcedOn = true,
        IsSuspended = true,
        UserHasAccess = true,
        NextInvoiceDate = new DateTimeOffset(2026, 11, 1, 0, 0, 0, TimeSpan.Zero),
        IsNfr = true,
        AdminAutoSyncIsEnabled = true,
        OldestEnclaveVersion = "2024.10.1",
    };

    private static string AdminJson(AccountGuid id, string role) => $$"""
        { "id": "{{id}}", "emailAddress": "alex@globex.example", "fullName": "Alex Smith", "joinDate": "2025-03-14T09:30:00Z", "role": "{{role}}" }
        """;

    private static CustomerUserModel ExpectedAdmin(AccountGuid id, UserOrganisationRole role) => new()
    {
        Id = id,
        EmailAddress = "alex@globex.example",
        FullName = "Alex Smith",
        JoinDate = new DateTime(2025, 3, 14, 9, 30, 0, DateTimeKind.Utc),
        Role = role,
    };
}
