using Enclave.Sdk.Api.Clients;
using Enclave.Sdk.Api.Data;
using Enclave.Sdk.Api.Data.Account;
using Enclave.Sdk.Api.Data.Dns;
using Enclave.Sdk.Api.Data.Pagination;
using Enclave.Sdk.Api.Data.PatchModel;
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
    private JsonSerializerOptions _serializerOptions = new()
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
        _dnsClient = (DnsClient)organisationClient.Dns;
    }

    [Test]
    public async Task Should_return_a_Dns_summary_when_calling_GetPropertiesSummaryAsync()
    {
        // Arrange
        var response = new DnsSummary
        {
            TotalRecordCount = 12,
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/dns").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        // Act
        var result = await _dnsClient.GetPropertiesSummaryAsync();

        // Assert
        result.TotalRecordCount.Should().Be(response.TotalRecordCount);
    }

    [Test]
    public async Task Should_return_a_paginated_response_model_when_calling_GetZonesAsync()
    {
        // Arrange
        var response = new PaginatedResponseModel<BasicDnsZone>
        {
            Items = new List<BasicDnsZone>
            {
                new BasicDnsZone { Name = "test"}
            },
            Links = new PaginationLinks(),
            Metadata = new PaginationMetadata(),
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/dns/zones").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        // Act
        var result = await _dnsClient.GetZonesAsync();

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public async Task Should_make_a_call_to_api_with_page_quertString_when_calling_GetZonesAsync()
    {
        // Arrange
        var response = new PaginatedResponseModel<BasicDnsZone>
        {
            Items = new List<BasicDnsZone>
            {
                new BasicDnsZone { Name = "test"}
            },
            Links = new PaginationLinks(),
            Metadata = new PaginationMetadata(),
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/dns/zones").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

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
        var response = new PaginatedResponseModel<BasicDnsZone>
        {
            Items = new List<BasicDnsZone>
            {
                new BasicDnsZone { Name = "test"}
            },
            Links = new PaginationLinks(),
            Metadata = new PaginationMetadata(),
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/dns/zones").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

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
        var response = new FullDnsZone
        {
            Id = DnsZoneId.FromInt(123),
            Name = "test",
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/dns/zones").UsingPost())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        // Act
        var result = await _dnsClient.CreateZoneAsync(new DnsZoneCreate());

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(response.Name);
    }

    [Test]
    public async Task Should_return_a_full_dns_zone_when_calling_GetZoneAsync()
    {
        // Arrange
        var response = new FullDnsZone
        {
            Id = DnsZoneId.FromInt(123),
            Name = "test",
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/dns/zones/{123}").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        // Act
        var result = await _dnsClient.GetZoneAsync(DnsZoneId.FromInt(123));

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(response.Name);
    }

    [Test]
    public async Task Should_return_a_full_dns_zone_when_calling_UpdateZoneAsync()
    {
        // Arrange
        var response = new FullDnsZone
        {
            Id = DnsZoneId.FromInt(123),
            Name = "New Name",
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/dns/zones/{123}").UsingPatch())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        var builder = new PatchBuilder<DnsZonePatch>().Set(d => d.Name, "New Name");

        // Act
        var result = await _dnsClient.UpdateZoneAsync(DnsZoneId.FromInt(123), builder);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(response.Name);
    }

    [Test]
    public async Task Should_return_a_full_dns_zone_when_calling_DeleteZoneAsync()
    {
        // Arrange
        var response = new FullDnsZone
        {
            Id = DnsZoneId.FromInt(123),
            Name = "New Name",
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/dns/zones/{123}").UsingDelete())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        // Act
        var result = await _dnsClient.DeleteZoneAsync(DnsZoneId.FromInt(123));

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(response.Name);
    }

    [Test]
    public async Task Should_return_a_paginated_response_model_when_calling_GetRecordsAsync()
    {
        // Arrange
        var response = new PaginatedResponseModel<BasicDnsRecord>
        {
            Items = new List<BasicDnsRecord>
            {
                new BasicDnsRecord { Name = "test"}
            },
            Links = new PaginationLinks(),
            Metadata = new PaginationMetadata(),
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/dns/records").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        // Act
        var result = await _dnsClient.GetRecordsAsync();

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public async Task Should_make_a_call_to_api_with_zoneId_quertString_when_calling_GetRecordsAsync()
    {
        // Arrange
        var response = new PaginatedResponseModel<BasicDnsRecord>
        {
            Items = new List<BasicDnsRecord>
            {
                new BasicDnsRecord { Name = "test"}
            },
            Links = new PaginationLinks(),
            Metadata = new PaginationMetadata(),
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/dns/records").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        var zonedId = DnsZoneId.FromInt(12);

        // Act
        await _dnsClient.GetRecordsAsync(dnsZoneId: zonedId);

        // Assert
        _server.Should().HaveReceivedACall().AtAbsoluteUrl($"{_server.Urls[0]}{_orgRoute}/dns/records?zoneId={zonedId}");
    }

    [Test]
    public async Task Should_make_a_call_to_api_with_hostname_quertString_when_calling_GetRecordsAsync()
    {
        // Arrange
        var response = new PaginatedResponseModel<BasicDnsRecord>
        {
            Items = new List<BasicDnsRecord>
            {
                new BasicDnsRecord { Name = "test"}
            },
            Links = new PaginationLinks(),
            Metadata = new PaginationMetadata(),
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/dns/records").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        var hostname = "name";

        // Act
        await _dnsClient.GetRecordsAsync(hostname: hostname);

        // Assert
        _server.Should().HaveReceivedACall().AtAbsoluteUrl($"{_server.Urls[0]}{_orgRoute}/dns/records?hostname={hostname}");
    }


    [Test]
    public async Task Should_make_a_call_to_api_with_page_quertString_when_calling_GetRecordsAsync()
    {
        // Arrange
        var response = new PaginatedResponseModel<BasicDnsRecord>
        {
            Items = new List<BasicDnsRecord>
            {
                new BasicDnsRecord { Name = "test"}
            },
            Links = new PaginationLinks(),
            Metadata = new PaginationMetadata(),
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/dns/records").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

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
        var response = new PaginatedResponseModel<BasicDnsRecord>
        {
            Items = new List<BasicDnsRecord>
            {
                new BasicDnsRecord { Name = "test"}
            },
            Links = new PaginationLinks(),
            Metadata = new PaginationMetadata(),
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/dns/records").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

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
        var response = new FullDnsRecord
        {
            Id = DnsRecordId.FromInt(123),
            Name = "Name",
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/dns/records").UsingPost())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        var createModel = new DnsRecordCreate
        {
            Name = "Name",
        };

        // Act
        var result = await _dnsClient.CreateRecordAsync(createModel);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(response.Id);
    }

    [Test]
    public async Task Should_return_number_of_deleted_records_when_calling_DeleteRecordsAsync()
    {
        // Arrange
        var response = new BulkDnsRecordDeleteResult
        {
             DnsRecordsDeleted = 3,
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/dns/records").UsingDelete())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

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
        var id = DnsRecordId.FromInt(123);
        var response = new FullDnsRecord
        {
            Id = id,
            Name = "Name",
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/dns/records/{id}").UsingGet())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

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
        var id = DnsRecordId.FromInt(123);
        var response = new FullDnsRecord
        {
            Id = id,
            Name = "New Name",
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/dns/records/{id}").UsingPatch())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        var builder = new PatchBuilder<DnsRecordPatch>().Set(d => d.Name, "New Name");

        // Act
        var result = await _dnsClient.UpdateRecordAsync(id, builder);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(response.Name);
    }


    [Test]
    public async Task Should_return_full_dns_record_when_calling_DeleteRecord()
    {
        // Arrange
        var id = DnsRecordId.FromInt(123);
        var response = new FullDnsRecord
        {
            Id = id,
            Name = "Name",
        };

        _server
          .Given(Request.Create().WithPath($"{_orgRoute}/dns/records/{id}").UsingDelete())
          .RespondWith(
            Response.Create()
              .WithSuccess()
              .WithHeader("Content-Type", "application/json")
              .WithBody(JsonSerializer.Serialize(response, _serializerOptions)));

        // Act
        var result = await _dnsClient.DeleteRecordAsync(id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(id);
    }
}
