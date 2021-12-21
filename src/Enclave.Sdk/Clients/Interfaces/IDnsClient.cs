using Enclave.Sdk.Api.Data;
using Enclave.Sdk.Api.Data.Dns;
using Enclave.Sdk.Api.Data.Pagination;
using Enclave.Sdk.Api.Data.PatchModel;

namespace Enclave.Sdk.Api.Clients.Interfaces;

/// <summary>
/// Provides operations to get, create, and manipulate DNS rules.
/// </summary>
public interface IDnsClient
{
    /// <summary>
    /// Gets a summary of DNS properties.
    /// </summary>
    /// <returns>A summary of DNS properties.</returns>
    Task<DnsSummary> GetPropertiesSummaryAsync();

    /// <summary>
    /// Gets a paginated list of DNS zones.
    /// </summary>
    /// <param name="pageNumber">Which page number do you want to return.</param>
    /// <param name="perPage">How many items per page.</param>
    /// <returns>A paginated response model with links to get the previous, next, first and last pages.</returns>
    Task<PaginatedResponseModel<DnsZoneSummary>> GetZonesAsync(int? pageNumber = null, int? perPage = null);

    /// <summary>
    /// Creates a DNS Zone using a <see cref="DnsZoneCreate"/> Model.
    /// </summary>
    /// <param name="createModel">The model needed to create a DNS Zone.</param>
    /// <returns>The created DNS Zone as a <see cref="DnsZone"/>.</returns>
    Task<DnsZone> CreateZoneAsync(DnsZoneCreate createModel);

    /// <summary>
    /// Gets the details of a specific DNS Zone.
    /// </summary>
    /// <param name="dnsZoneId">The DNS Zone ID that you want to get.</param>
    /// <returns>A full DNS Zone object.</returns>
    Task<DnsZone> GetZoneAsync(DnsZoneId dnsZoneId);

    /// <summary>
    /// Starts an update patch request.
    /// </summary>
    /// <param name="dnsZoneId">The DnsZoneId to update.</param>
    /// <returns>A PatchClient for fluent updating.</returns>
    IPatchClient<DnsZonePatch, DnsZone> UpdateZone(DnsZoneId dnsZoneId);

    /// <summary>
    /// Delete a DNS Zone and it's associated record. This is irriversable.
    /// </summary>
    /// <param name="dnsZoneId">The DNS Zone ID that you want to update.</param>
    /// <returns>The deleted DNS Zone object.</returns>
    Task<DnsZone> DeleteZoneAsync(DnsZoneId dnsZoneId);

    /// <summary>
    /// Gets a paginated list of DNS records.
    /// </summary>
    /// <param name="dnsZoneId">The DNS Zone ID that you want to get.</param>
    /// <param name="hostname">A partial hostname search value.</param>
    /// <param name="pageNumber">Which page number do you want to return.</param>
    /// <param name="perPage">How many items per page.</param>
    /// <returns>A paginated response model with links to get the previous, next, first and last pages.</returns>
    Task<PaginatedResponseModel<DnsRecordSummary>> GetRecordsAsync(
        DnsZoneId? dnsZoneId = null,
        string? hostname = null,
        int? pageNumber = null,
        int? perPage = null);

    /// <summary>
    /// Create a DNS Record using a <see cref="DnsRecordCreate"/> model.
    /// </summary>
    /// <param name="createModel">The model needed to create a DNS Record.</param>
    /// <returns>The created DNS Record as <see cref="DnsRecordCreate"/>.</returns>
    Task<DnsRecord> CreateRecordAsync(DnsRecordCreate createModel);

    /// <summary>
    /// Delete multiple DNS Records.
    /// </summary>
    /// <param name="records">An Array of the Record Ids to delete.</param>
    /// <returns>The number of records deleted.</returns>
    Task<int> DeleteRecordsAsync(params DnsRecordId[] records);

    /// <summary>
    /// Delete multiple DNS Records.
    /// </summary>
    /// <param name="records">An IEnumerable of the Record Ids to delete.</param>
    /// <returns>The number of records deleted.</returns>
    Task<int> DeleteRecordsAsync(IEnumerable<DnsRecordId> records);

    /// <summary>
    /// Get a detailed DNS Record.
    /// </summary>
    /// <param name="dnsRecordId">The id of the DNS Record you want to get.</param>
    /// <returns>A detailed DNS Record object.</returns>
    Task<DnsRecord> GetRecordAsync(DnsRecordId dnsRecordId);

    /// <summary>
    /// Starts an update patch request.
    /// </summary>
    /// <param name="dnsRecordId">The DnsRecordId to update.</param>
    /// <returns>A PatchClient for fluent updating.</returns>
    IPatchClient<DnsRecordPatch, DnsRecord> UpdateRecord(DnsRecordId dnsRecordId);

    /// <summary>
    /// Delete a single DNS Record.
    /// </summary>
    /// <param name="dnsRecordId">The DNS Record ID that you want to delete.</param>
    /// <returns>The deleted DNS Record object.</returns>
    Task<DnsRecord> DeleteRecordAsync(DnsRecordId dnsRecordId);
}