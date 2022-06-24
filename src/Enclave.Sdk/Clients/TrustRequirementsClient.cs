using Enclave.Sdk.Api.Clients.Interfaces;
using Enclave.Sdk.Api.Data;
using Enclave.Sdk.Api.Data.Pagination;
using Enclave.Sdk.Api.Data.TrustRequirements;
using Enclave.Sdk.Api.Data.TrustRequirements.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Enclave.Sdk.Api.Clients;

internal class TrustRequirementsClient : ClientBase, ITrustRequirementsClient
{
    private readonly string _orgRoute;

    /// <summary>
    /// Constructor which will be called by <see cref="OrganisationClient"/> when it's created.
    /// </summary>
    /// <param name="httpClient">an instance of httpClient with a baseURL referencing the API.</param>
    /// <param name="orgRoute">The organisation API route.</param>
    public TrustRequirementsClient(HttpClient httpClient, string orgRoute)
    : base(httpClient)
    {
        _orgRoute = orgRoute;
    }

    /// <inheritdoc/>
    public async Task<PaginatedResponseModel<TrustRequirement>> GetTrustRequirementsAsync(
        string? searchTerm = null,
        TrustRequirementSortOrder? sortOrder = null,
        int? pageNumber = null,
        int? perPage = null)
    {
        var queryString = BuildQueryString(searchTerm, sortOrder, pageNumber, perPage);

        var model = await HttpClient.GetFromJsonAsync<PaginatedResponseModel<TrustRequirement>>($"{_orgRoute}/trust-requirements?{queryString}", Constants.JsonSerializerOptions);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public async Task<TrustRequirement> CreateAsync(TrustRequirementCreate createModel)
    {
        if (createModel is null)
        {
            throw new ArgumentNullException(nameof(createModel));
        }

        var result = await HttpClient.PostAsJsonAsync($"{_orgRoute}/trust-requirements", createModel, Constants.JsonSerializerOptions);

        var model = await DeserialiseAsync<TrustRequirement>(result.Content);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public async Task<int> DeleteTrustRequirementsAsync(params TrustRequirementId[] requirementIds)
    {
        using var content = CreateJsonContent(new
        {
            requirementIds,
        });

        using var request = new HttpRequestMessage
        {
            Content = content,
            Method = HttpMethod.Delete,
            RequestUri = new Uri($"{HttpClient.BaseAddress}{_orgRoute}/trust-requirements"),
        };

        var result = await HttpClient.SendAsync(request);

        result.EnsureSuccessStatusCode();

        var model = await DeserialiseAsync<BulkTrustRequirementDeleteResult>(result.Content);

        EnsureNotNull(model);

        return model.RequirementsDeleted;
    }

    /// <inheritdoc/>
    public async Task<TrustRequirement> GetAsync(TrustRequirementId requirementId)
    {
        var model = await HttpClient.GetFromJsonAsync<TrustRequirement>($"{_orgRoute}/trust-requirements/{requirementId}", Constants.JsonSerializerOptions);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public IPatchClient<TrustRequirementPatch, TrustRequirement> Update(TrustRequirementId requirementId)
    {
        return new PatchClient<TrustRequirementPatch, TrustRequirement>(HttpClient, $"{_orgRoute}/trust-requirements/{requirementId}");
    }

    /// <inheritdoc/>
    public async Task<TrustRequirement> DeleteAsync(TrustRequirementId requirementId)
    {
        var result = await HttpClient.DeleteAsync($"{_orgRoute}/trust-requirements/{requirementId}");

        result.EnsureSuccessStatusCode();

        var model = await DeserialiseAsync<TrustRequirement>(result.Content);

        EnsureNotNull(model);

        return model;
    }

    private static string? BuildQueryString(string? searchTerm, TrustRequirementSortOrder? sortOrder, int? pageNumber, int? perPage)
    {
        var queryString = HttpUtility.ParseQueryString(string.Empty);
        if (searchTerm is not null)
        {
            queryString.Add("search", searchTerm);
        }

        if (sortOrder is not null)
        {
            queryString.Add("sort", sortOrder.ToString());
        }

        if (pageNumber is not null)
        {
            queryString.Add("page", pageNumber.ToString());
        }

        if (perPage is not null)
        {
            queryString.Add("per_page", perPage.ToString());
        }

        return queryString.ToString();
    }
}
