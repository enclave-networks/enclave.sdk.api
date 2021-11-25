using Enclave.Sdk.Api.Clients.Interfaces;
using Enclave.Sdk.Api.Data.EnrolmentKeys;
using Enclave.Sdk.Api.Data.EnrolmentKeys.Enum;
using Enclave.Sdk.Api.Data.Pagination;

namespace Enclave.Sdk.Api.Clients;

/// <inheritdoc cref="IEnrolmentKeysClient" />
public class EnrolmentKeysClient : ClientBase, IEnrolmentKeysClient
{
    private string _orgRoute;

    /// <summary>
    /// This constructor is called by <see cref="EnclaveClient"/> when setting up the <see cref="EnrolmentKeysClient"/>.
    /// It also calls the <see cref="ClientBase"/> constructor.
    /// </summary>
    /// <param name="httpClient">an instance of httpClient with a baseURL referencing the API.</param>
    /// <param name="orgRoute">The API</param>
    public EnrolmentKeysClient(HttpClient httpClient, string orgRoute)
    : base(httpClient)
    {
        _orgRoute = orgRoute;
    }

    public async Task<PaginatedResponseModel<EnrolmentKey>> GetEnrolmentKeys(
        string? searchTerm = null,
        bool includeDisabled = false,
        EnrolmentKeySortOrder? sortOrder = null,
        int? page = null,
        int? perPage = null)
    {

    }
}
