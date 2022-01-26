using System.Net.Http.Json;
using Enclave.Sdk.Api.Clients.Interfaces;
using Enclave.Sdk.Api.Data.Authority;

namespace Enclave.Sdk.Api.Clients;

internal class AuthorityClient : ClientBase, IAuthorityClient
{
    private readonly string _orgRoute;

    public AuthorityClient(HttpClient httpClient, string orgRoute)
        : base(httpClient)
    {
        _orgRoute = orgRoute;
    }

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
