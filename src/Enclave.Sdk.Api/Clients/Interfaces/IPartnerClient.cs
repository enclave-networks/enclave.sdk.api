using Enclave.Configuration.Data.Identifiers;

namespace Enclave.Sdk.Api.Clients.Interfaces;

/// <summary>
/// Calls to the Enclave Partner API for one partner. Every call goes to <see cref="EnclaveClientOptions.PartnerApiBaseUrl"/>.
/// </summary>
public interface IPartnerClient
{
    /// <summary>
    /// The ID of the partner every call is made for.
    /// </summary>
    PartnerId PartnerId { get; }

    /// <summary>
    /// The partner's customers, with their admins, admin invites and auto-sync.
    /// </summary>
    ICustomersClient Customers { get; }
}
