using Enclave.Api.Modules.SystemManagement.Dns.Models;

namespace Enclave.Sdk.Api.Data.PatchModel;

/// <summary>
/// The patch model for a DNS record. Any values not provided will not be updated.
/// </summary>
public class DnsRecordPatch : DnsRecordPatchModel, IPatchModel
{
}