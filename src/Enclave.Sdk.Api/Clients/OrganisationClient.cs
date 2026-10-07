using Enclave.Api.Modules.AccountManagement.PublicAccount.Models;
using Enclave.Sdk.Api.Clients.Interfaces;

namespace Enclave.Sdk.Api.Clients;

/// <inheritdoc cref="IOrganisationClient" />
internal class OrganisationClient : OrganisationScopedClient, IOrganisationClient
{
    /// <summary>
    /// This constructor is called by <see cref="EnclaveClient"/> when setting up the <see cref="OrganisationClient"/>.
    /// It also calls the <see cref="OrganisationScopedClient"/> constructor.
    /// </summary>
    /// <param name="httpClient">an instance of httpClient with a baseURL referencing the API.</param>
    /// <param name="currentOrganisation">The current organisaiton used for routing the API calls.</param>
    public OrganisationClient(HttpClient httpClient, AccountOrganisationModel currentOrganisation)
        : base(httpClient, currentOrganisation.OrgId)
    {
        Organisation = currentOrganisation;
    }

    /// <inheritdoc/>
    public AccountOrganisationModel Organisation { get; }
}
