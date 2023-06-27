using Enclave.Api.Modules.SystemManagement.Dns.Models;
using Enclave.Api.Modules.SystemManagement.Systems.Models;
using Enclave.Api.Modules.SystemManagement.Tags.Models;
using Enclave.Api.Scaffolding.Pagination.Models;
using Enclave.Configuration.Data.Identifiers;
using Enclave.Sdk.Api.Clients;
using FluentAssertions;
using NUnit.Framework;
using System.Text.Json;
using WireMock.FluentAssertions;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Enclave.Sdk.Api.Tests.Clients;

public class DnsClientTests
{
    private DnsClient _dnsClient;
    private WireMockServer _server;
    private string _orgRoute;
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private PaginatedResponseModel<DnsZoneSummaryModel> _paginatedResponse;
    private DnsZoneModel _dnsZoneResponse;
    private DnsRecordModel _dnsRecordResponse;

    [SetUp]
    public void Setup()
    {
        _server = WireMockServer.Start();

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri(_server.Urls[0]),
        };

        var organisationId = OrganisationGuid.New();
        _orgRoute = $"/org/{organisationId}";

        _dnsClient = new DnsClient(httpClient, $"org/{organisationId}");

        _paginatedResponse = new(
                new(0, 0, 0, 0, 0),
                new(new Uri("http://enclave.io"), new Uri("http://enclave.io"), new Uri("http://enclave.io"), new Uri("http://enclave.io")),
                new List<DnsZoneSummaryModel>
                {
                    new DnsZoneSummaryModel(DnsZoneId.FromInt(1), "Test", DateTime.Now, 1, new Dictionary<string, int>()),
                }.ToAsyncEnumerable());

        _dnsZoneResponse = new(
            DnsZoneId.FromInt(123),
            "Test",
            DateTime.Now,
            1,
            new Dictionary<string, int>(),
            null);

        _dnsRecordResponse = new(
            DnsRecordId.FromInt(1),
            "Test",
            string.Empty,
            DnsZoneId.FromInt(123),
            "Zone1",
            "test.com",
            Array.Empty<IUsedTagModel>(),
            Array.Empty<ISystemReferenceModel>().ToAsyncEnumerable(),
            null);
    }

    [Test]
    public async Task Should_return_a_Dns_summary_when_calling_GetPropertiesSummaryAsync()
    {
        // Arrange
        var response = new DnsSummaryModel(0, 12);

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/dns").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await response.ToJsonAsync(_serializerOptions)));

        // Act
        var result = await _dnsClient.GetPropertiesSummaryAsync();

        // Assert
        result.TotalRecordCount.Should().Be(response.TotalRecordCount);
    }

    [Test]
    public async Task Should_return_a_paginated_response_model_when_calling_GetZonesAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/dns/zones").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _paginatedResponse.ToJsonAsync(_serializerOptions)));

        // Act
        var result = await _dnsClient.GetZonesAsync();

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public async Task Should_make_a_call_to_api_with_page_quertString_when_calling_GetZonesAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/dns/zones").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _paginatedResponse.ToJsonAsync(_serializerOptions)));

        var page = 12;

        // Act
        await _dnsClient.GetZonesAsync(pageNumber: page);

        // Assert
        _server.Should().HaveReceivedACall().AtAbsoluteUrl($"{_server.Urls[0]}{_orgRoute}/dns/zones?page={page}");
    }

    [Test]
    public async Task Should_make_a_call_to_api_with_per_page_quertString_when_calling_GetZonesAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/dns/zones").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _paginatedResponse.ToJsonAsync(_serializerOptions)));

        var page = 12;

        // Act
        await _dnsClient.GetZonesAsync(perPage: page);

        // Assert
        _server.Should().HaveReceivedACall().AtAbsoluteUrl($"{_server.Urls[0]}{_orgRoute}/dns/zones?per_page={page}");
    }

    [Test]
    public async Task Should_return_a_full_dns_zone_when_calling_CreateZoneAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/dns/zones").UsingPost())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _dnsZoneResponse.ToJsonAsync(_serializerOptions)));

        // Act
        var result = await _dnsClient.CreateZoneAsync(new DnsZoneCreateModel());

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(_dnsZoneResponse.Name);
    }

    [Test]
    public async Task Should_return_a_full_dns_zone_when_calling_GetZoneAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/dns/zones/{123}").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _dnsZoneResponse.ToJsonAsync(_serializerOptions)));

        // Act
        var result = await _dnsClient.GetZoneAsync(DnsZoneId.FromInt(123));

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(_dnsZoneResponse.Name);
    }

    [Test]
    public async Task Should_return_a_full_dns_zone_when_calling_UpdateZoneAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/dns/zones/{123}").UsingPatch())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _dnsZoneResponse.ToJsonAsync(_serializerOptions)));

        // Act
        var result = await _dnsClient.UpdateZone(DnsZoneId.FromInt(123)).Set(d => d.Name, "New Name").ApplyAsync();

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(_dnsZoneResponse.Name);
    }

    [Test]
    public async Task Should_return_a_full_dns_zone_when_calling_DeleteZoneAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/dns/zones/{123}").UsingDelete())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _dnsZoneResponse.ToJsonAsync(_serializerOptions)));

        // Act
        var result = await _dnsClient.DeleteZoneAsync(DnsZoneId.FromInt(123));

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(_dnsZoneResponse.Name);
    }

    [Test]
    public async Task Should_return_a_paginated_response_model_when_calling_GetRecordsAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/dns/records").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _paginatedResponse.ToJsonAsync(_serializerOptions)));

        // Act
        var result = await _dnsClient.GetRecordsAsync();

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public async Task Should_make_a_call_to_api_with_zoneId_quertString_when_calling_GetRecordsAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/dns/records").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _paginatedResponse.ToJsonAsync(_serializerOptions)));

        var zonedId = DnsZoneId.FromInt(12);

        // Act
        await _dnsClient.GetRecordsAsync(dnsZoneId: zonedId);

        // Assert
        _server.Should().HaveReceivedACall().AtAbsoluteUrl($"{_server.Urls[0]}{_orgRoute}/dns/records?zoneId={zonedId}");
    }

    [Test]
    public async Task Should_make_a_call_to_api_with_search_quertString_when_calling_GetRecordsAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/dns/records").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _paginatedResponse.ToJsonAsync(_serializerOptions)));

        var hostname = "name";

        // Act
        await _dnsClient.GetRecordsAsync(searchTerm: hostname);

        // Assert
        _server.Should().HaveReceivedACall().AtAbsoluteUrl($"{_server.Urls[0]}{_orgRoute}/dns/records?search={hostname}");
    }


    [Test]
    public async Task Should_make_a_call_to_api_with_page_quertString_when_calling_GetRecordsAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/dns/records").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _paginatedResponse.ToJsonAsync(_serializerOptions)));

        var page = 12;

        // Act
        await _dnsClient.GetRecordsAsync(pageNumber: page);

        // Assert
        _server.Should().HaveReceivedACall().AtAbsoluteUrl($"{_server.Urls[0]}{_orgRoute}/dns/records?page={page}");
    }

    [Test]
    public async Task Should_make_a_call_to_api_with_per_page_quertString_when_calling_GetRecordsAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/dns/records").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _paginatedResponse.ToJsonAsync(_serializerOptions)));

        var page = 12;

        // Act
        await _dnsClient.GetRecordsAsync(perPage: page);

        // Assert
        _server.Should().HaveReceivedACall().AtAbsoluteUrl($"{_server.Urls[0]}{_orgRoute}/dns/records?per_page={page}");
    }

    [Test]
    public async Task Should_return_a_full_dns_record_model_when_calling_CreateRecordAsync()
    {
        // Arrange
        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/dns/records").UsingPost())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _dnsRecordResponse.ToJsonAsync(_serializerOptions)));

        var createModel = new DnsRecordCreateModel
        {
            Name = "Name",
        };

        // Act
        var result = await _dnsClient.CreateRecordAsync(createModel);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(_dnsRecordResponse.Id);
    }

    [Test]
    public async Task Should_return_number_of_deleted_records_when_calling_DeleteRecordsAsync()
    {
        // Arrange
        var response = new BulkDnsRecordDeleteResult(3);

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/dns/records").UsingDelete())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await response.ToJsonAsync(_serializerOptions)));

        var records = new DnsRecordId[] { DnsRecordId.FromInt(1), DnsRecordId.FromInt(2), DnsRecordId.FromInt(3) };


        // Act
        var result = await _dnsClient.DeleteRecordsAsync(records);

        // Assert
        result.Should().Be(3);
    }

    [Test]
    public async Task Should_return_full_dns_record_when_calling_GetRecordAsync()
    {
        // Arrange
        var id = _dnsRecordResponse.Id;

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/dns/records/{id}").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _dnsRecordResponse.ToJsonAsync(_serializerOptions)));

        // Act
        var result = await _dnsClient.GetRecordAsync(id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(id);
    }

    [Test]
    public async Task Should_return_full_dns_record_when_calling_UpdateRecordAsync()
    {
        // Arrange
        var id = _dnsRecordResponse.Id;

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/dns/records/{id}").UsingPatch())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _dnsRecordResponse.ToJsonAsync(_serializerOptions)));

        // Act
        var result = await _dnsClient.UpdateRecord(id).Set(d => d.Name, "New Name").ApplyAsync();

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(_dnsRecordResponse.Name);
    }


    [Test]
    public async Task Should_return_full_dns_record_when_calling_DeleteRecord()
    {
        // Arrange
        var id = _dnsRecordResponse.Id;

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/dns/records/{id}").UsingDelete())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(await _dnsRecordResponse.ToJsonAsync(_serializerOptions)));

        // Act
        var result = await _dnsClient.DeleteRecordAsync(id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(id);
    }
}
