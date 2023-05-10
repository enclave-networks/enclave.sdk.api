using System.Net.Http.Json;
using System.Web;
using Enclave.Sdk.Api.Clients.Interfaces;
using Enclave.Sdk.Api.Data;
using Enclave.Sdk.Api.Data.AutoExpiry;
using Enclave.Sdk.Api.Data.EnrolmentKeys;
using Enclave.Sdk.Api.Data.Pagination;
using Enclave.Sdk.Api.Data.PatchModel;
using Enclave.Sdk.Api.Data.Policies;
using Enclave.Sdk.Api.Data.Policies.Enum;

namespace Enclave.Sdk.Api.Clients;

/// <inheritdoc cref="IPoliciesClient" />
internal class PoliciesClient : ClientBase, IPoliciesClient
{
    private readonly string _orgRoute;

    /// <summary>
    /// Constructor which will be called by <see cref="OrganisationClient"/> when it's created.
    /// It also calls the <see cref="ClientBase"/> constructor.
    /// </summary>
    /// <param name="httpClient">an instance of httpClient with a baseURL referencing the API.</param>
    /// <param name="orgRoute">The organisation API route.</param>
    public PoliciesClient(HttpClient httpClient, string orgRoute)
    : base(httpClient)
    {
        _orgRoute = orgRoute;
    }

    /// <inheritdoc/>
    public async Task<PaginatedResponseModel<Policy>> GetPoliciesAsync(
        string? searchTerm = null,
        bool? includeDisabled = null,
        PolicySortOrder? sortOrder = null,
        int? pageNumber = null,
        int? perPage = null)
    {
        var queryString = BuildQueryString(searchTerm, includeDisabled, sortOrder, pageNumber, perPage);

        var model = await HttpClient.GetFromJsonAsync<PaginatedResponseModel<Policy>>($"{_orgRoute}/policies?{queryString}", Constants.JsonSerializerOptions);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public async Task<Policy> CreateAsync(PolicyCreate createModel)
    {
        if (createModel is null)
        {
            throw new ArgumentNullException(nameof(createModel));
        }

        var result = await HttpClient.PostAsJsonAsync($"{_orgRoute}/policies", createModel, Constants.JsonSerializerOptions);

        var model = await DeserialiseAsync<Policy>(result.Content);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public async Task<int> DeletePoliciesAsync(params PolicyId[] policyIds)
    {
        using var content = CreateJsonContent(new
        {
            policyIds,
        });

        using var request = new HttpRequestMessage
        {
            Content = content,
            Method = HttpMethod.Delete,
            RequestUri = new Uri($"{HttpClient.BaseAddress}{_orgRoute}/policies"),
        };

        var result = await HttpClient.SendAsync(request);

        result.EnsureSuccessStatusCode();

        var model = await DeserialiseAsync<BulkPolicyDeleteResult>(result.Content);

        EnsureNotNull(model);

        return model.PoliciesDeleted;
    }

    /// <inheritdoc/>
    public async Task<int> DeletePoliciesAsync(IEnumerable<PolicyId> policyIds)
    {
        return await DeletePoliciesAsync(policyIds.ToArray());
    }

    /// <inheritdoc/>
    public async Task<Policy> GetAsync(PolicyId policyId)
    {
        var model = await HttpClient.GetFromJsonAsync<Policy>($"{_orgRoute}/policies/{policyId}", Constants.JsonSerializerOptions);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public IPatchClient<PolicyPatch, Policy> Update(PolicyId policyId)
    {
        return new PatchClient<PolicyPatch, Policy>(HttpClient, $"{_orgRoute}/policies/{policyId}");
    }

    /// <inheritdoc/>
    public async Task<Policy> DeleteAsync(PolicyId policyId)
    {
        var result = await HttpClient.DeleteAsync($"{_orgRoute}/policies/{policyId}");

        result.EnsureSuccessStatusCode();

        var model = await DeserialiseAsync<Policy>(result.Content);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public async Task<Policy> EnableAsync(PolicyId policyId)
    {
        var result = await HttpClient.PutAsync($"{_orgRoute}/policies/{policyId}/enable", null);

        var model = await DeserialiseAsync<Policy>(result.Content);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public async Task<Policy> DisableAsync(PolicyId policyId)
    {
        var result = await HttpClient.PutAsync($"{_orgRoute}/policies/{policyId}/disable", null);

        var model = await DeserialiseAsync<Policy>(result.Content);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public async Task<int> EnablePoliciesAsync(params PolicyId[] policyIds)
    {
        var requestModel = new
        {
            policyIds,
        };

        var result = await HttpClient.PutAsJsonAsync($"{_orgRoute}/policies/enable", requestModel, Constants.JsonSerializerOptions);

        result.EnsureSuccessStatusCode();

        var model = await DeserialiseAsync<BulkPolicyUpdateResult>(result.Content);

        EnsureNotNull(model);

        return model.PoliciesUpdated;
    }

    /// <inheritdoc/>
    public async Task<int> EnablePoliciesAsync(IEnumerable<PolicyId> policyIds)
    {
        return await EnablePoliciesAsync(policyIds.ToArray());
    }

    /// <inheritdoc/>
    public async Task<int> DisablePoliciesAsync(params PolicyId[] policyIds)
    {
        var requestModel = new
        {
            policyIds,
        };

        var result = await HttpClient.PutAsJsonAsync($"{_orgRoute}/policies/disable", requestModel, Constants.JsonSerializerOptions);

        result.EnsureSuccessStatusCode();

        var model = await DeserialiseAsync<BulkPolicyUpdateResult>(result.Content);

        EnsureNotNull(model);

        return model.PoliciesUpdated;
    }

    /// <inheritdoc/>
    public async Task<int> DisablePoliciesAsync(IEnumerable<PolicyId> policyIds)
    {
        return await DisablePoliciesAsync(policyIds.ToArray());
    }

    /// <inheritdoc/>
    public async Task<Policy> EnableUntilAsync(PolicyId policyId, DateTimeOffset expiryDateTime, ExpiryAction expiryAction, string? timeZonedId = null)
    {
        var requestModel = new AutoExpire
        {
            ExpiryDateTime = expiryDateTime.ToString("o"),
            ExpiryAction = expiryAction,
            TimeZoneId = timeZonedId,
        };

        var result = await HttpClient.PutAsJsonAsync($"{_orgRoute}/policies/{policyId}/enable-until", requestModel, Constants.JsonSerializerOptions);

        result.EnsureSuccessStatusCode();

        var model = await DeserialiseAsync<Policy>(result.Content);

        EnsureNotNull(model);

        return model;
    }

    private static string? BuildQueryString(string? searchTerm, bool? includeDisabled, PolicySortOrder? sortOrder, int? pageNumber, int? perPage)
    {
        var queryString = HttpUtility.ParseQueryString(string.Empty);
        if (searchTerm is not null)
        {
            queryString.Add("search", searchTerm);
        }

        if (includeDisabled is not null)
        {
            queryString.Add("include_disabled", includeDisabled.ToString());
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