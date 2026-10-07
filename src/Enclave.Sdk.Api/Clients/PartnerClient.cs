using Enclave.Configuration.Data.Identifiers;
using Enclave.Sdk.Api.Clients.Interfaces;

namespace Enclave.Sdk.Api.Clients;

/// <inheritdoc cref="IPartnerClient" />
internal sealed class PartnerClient : IPartnerClient
{
    /// <summary>
    /// This constructor is called by <see cref="EnclaveClient"/> when setting up a client for a partner.
    /// </summary>
    /// <param name="httpClient">an instance of httpClient with a baseURL referencing the partner API.</param>
    /// <param name="partnerId">The ID of the partner used for routing the API calls.</param>
    public PartnerClient(HttpClient httpClient, PartnerId partnerId)
    {
        PartnerId = partnerId;

        // Every partner API route is under /partner/{partnerId}/ (portal Enclave.Partner.Api/Scaffolding/PartnerRouteAttribute.cs).
        // A PartnerId formats as 32 hex digits, so it needs no escaping.
        Customers = new CustomersClient(httpClient, $"partner/{partnerId}");
    }

    /// <inheritdoc/>
    public PartnerId PartnerId { get; }

    /// <inheritdoc/>
    public ICustomersClient Customers { get; }
}
