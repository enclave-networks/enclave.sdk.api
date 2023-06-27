using System.Net.Http.Json;
using System.Web;
using Enclave.Api.Modules.SystemManagement.TrustRequirements.Models;
using Enclave.Api.Scaffolding.Pagination.Models;
using Enclave.Configuration.Data.Identifiers;
using Enclave.Configuration.Data.Modules.TrustRequirements.Enums;
using Enclave.Sdk.Api.Clients.Interfaces;
using Enclave.Sdk.Api.Data;

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
    public async Task<PaginatedResponseModel<TrustRequirementSummaryModel>> GetTrustRequirementsAsync(
        string? searchTerm = null,
        TrustRequirementSortOrder? sortOrder = null,
        int? pageNumber = null,
        int? perPage = null)
    {
        var queryString = BuildQueryString(searchTerm, sortOrder, pageNumber, perPage);

        var model = await HttpClient.GetFromJsonAsync<PaginatedResponseModel<TrustRequirementSummaryModel>>($"{_orgRoute}/trust-requirements?{queryString}", Constants.JsonSerializerOptions);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public async Task<TrustRequirementModel> CreateAsync(TrustRequirementCreateModel createModel)
    {
        if (createModel is null)
        {
            throw new ArgumentNullException(nameof(createModel));
        }

        var result = await HttpClient.PostAsJsonAsync($"{_orgRoute}/trust-requirements", createModel, Constants.JsonSerializerOptions);

        var model = await DeserialiseAsync<TrustRequirementModel>(result.Content);

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
    public async Task<TrustRequirementModel> GetAsync(TrustRequirementId requirementId)
    {
        var model = await HttpClient.GetFromJsonAsync<TrustRequirementModel>($"{_orgRoute}/trust-requirements/{requirementId}", Constants.JsonSerializerOptions);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public IPatchClient<TrustRequirementPatchModel, TrustRequirementModel> Update(TrustRequirementId requirementId)
    {
        return new PatchClient<TrustRequirementPatchModel, TrustRequirementModel>(HttpClient, $"{_orgRoute}/trust-requirements/{requirementId}");
    }

    /// <inheritdoc/>
    public async Task<TrustRequirementModel> DeleteAsync(TrustRequirementId requirementId)
    {
        var result = await HttpClient.DeleteAsync($"{_orgRoute}/trust-requirements/{requirementId}");

        result.EnsureSuccessStatusCode();

        var model = await DeserialiseAsync<TrustRequirementModel>(result.Content);

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
