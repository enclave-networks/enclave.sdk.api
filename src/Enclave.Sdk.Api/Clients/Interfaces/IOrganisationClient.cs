using Enclave.Api.Modules.AccountManagement.PublicAccount.Models;

namespace Enclave.Sdk.Api.Clients.Interfaces;

/// <summary>
/// Provides access to organisation level API calls and organisation related clients.
/// For more information please refer to our API docs at https://api.enclave.io/.
/// </summary>
public interface IOrganisationClient : IOrganisationScopedClient
{
    /// <summary>
    /// The organisation selected and the one used to create this client.
    /// </summary>
    AccountOrganisationModel Organisation { get; }
}
