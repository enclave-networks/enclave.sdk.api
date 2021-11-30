namespace Enclave.Sdk.Api.Data.Dns;

/// <summary>
/// Defines the result of a DNS Record bulk delete.
/// </summary>
public class BulkDnsRecordDeleteResult
{
    /// <summary>
    /// Specifies the number of DNS Records that were successfully deleted.
    /// </summary>
    public int DnsRecordsDeleted { get; }
}
