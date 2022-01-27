using System.Net.Http.Json;
using Enclave.Sdk.Api.Clients.Interfaces;
using Enclave.Sdk.Api.Data.Authority;

namespace Enclave.Sdk.Api.Clients;

/// <inheritdoc cref="IAuthorityClient" />
internal class AuthorityClient : ClientBase, IAuthorityClient
{
    private readonly string _orgRoute;

    /// <summary>
    /// Constructor  which will be called by <see cref="OrganisationClient"/> when it's created.
    /// </summary>
    /// <param name="httpClient">an instance of httpClient with a baseURL referencing the API.</param>
    /// <param name="orgRoute">The organisation API route.</param>
    public AuthorityClient(HttpClient httpClient, string orgRoute)
        : base(httpClient)
    {
        _orgRoute = orgRoute;
    }

    /// <inheritdoc/>
    public async Task<EnrolResult> Enrol(EnrolRequest requestModel)
    {
        if (requestModel is null)
        {
            throw new ArgumentNullException(nameof(requestModel));
        }

        var result = await HttpClient.PostAsJsonAsync($"{_orgRoute}/authority/enrol", requestModel, Constants.JsonSerializerOptions);

        var model = await DeserialiseAsync<EnrolResult>(result.Content);

        EnsureNotNull(model);

        return model;
    }
}
