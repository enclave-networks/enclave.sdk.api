using System.Net.Http.Json;
using System.Web;
using Enclave.Sdk.Api.Clients.Interfaces;
using Enclave.Sdk.Api.Data;
using Enclave.Sdk.Api.Data.EnrolmentKeys;
using Enclave.Sdk.Api.Data.EnrolmentKeys.Enum;
using Enclave.Sdk.Api.Data.Pagination;
using Enclave.Sdk.Api.Data.PatchModel;

namespace Enclave.Sdk.Api.Clients;

/// <inheritdoc cref="IEnrolmentKeysClient" />
internal class EnrolmentKeysClient : ClientBase, IEnrolmentKeysClient
{
    private readonly string _orgRoute;

    /// <summary>
    /// Constructor  which will be called by <see cref="OrganisationClient"/> when it's created.
    /// </summary>
    /// <param name="httpClient">an instance of httpClient with a baseURL referencing the API.</param>
    /// <param name="orgRoute">The organisation API route.</param>
    public EnrolmentKeysClient(HttpClient httpClient, string orgRoute)
    : base(httpClient)
    {
        _orgRoute = orgRoute;
    }

    /// <inheritdoc/>
    public async Task<PaginatedResponseModel<EnrolmentKeySummary>> GetEnrolmentKeysAsync(
        string? searchTerm = null,
        bool? includeDisabled = null,
        EnrolmentKeySortOrder? sortOrder = null,
        int? pageNumber = null,
        int? perPage = null)
    {
        var queryString = BuildQueryString(searchTerm, includeDisabled, sortOrder, pageNumber, perPage);

        var model = await HttpClient.GetFromJsonAsync<PaginatedResponseModel<EnrolmentKeySummary>>($"{_orgRoute}/enrolment-keys?{queryString}");

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public async Task<EnrolmentKey> CreateAsync(EnrolmentKeyCreate createModel)
    {
        if (createModel is null)
        {
            throw new ArgumentNullException(nameof(createModel));
        }

        var result = await HttpClient.PostAsJsonAsync($"{_orgRoute}/enrolment-keys", createModel, Constants.JsonSerializerOptions);

        var model = await DeserialiseAsync<EnrolmentKey>(result.Content);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public async Task<EnrolmentKey> GetAsync(EnrolmentKeyId enrolmentKeyId)
    {
        var model = await HttpClient.GetFromJsonAsync<EnrolmentKey>($"{_orgRoute}/enrolment-keys/{enrolmentKeyId}", Constants.JsonSerializerOptions);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public async Task<EnrolmentKey> UpdateAsync(EnrolmentKeyId enrolmentKeyId, PatchBuilder<EnrolmentKeyPatchModel> builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        using var encoded = CreateJsonContent(builder.Send());
        var result = await HttpClient.PatchAsync($"{_orgRoute}/enrolment-keys/{enrolmentKeyId}", encoded);

        result.EnsureSuccessStatusCode();

        var model = await DeserialiseAsync<EnrolmentKey>(result.Content);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public async Task<EnrolmentKey> EnableAsync(EnrolmentKeyId enrolmentKeyId)
    {
        var result = await HttpClient.PutAsync($"{_orgRoute}/enrolment-keys/{enrolmentKeyId}/enable", null);

        var model = await DeserialiseAsync<EnrolmentKey>(result.Content);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public async Task<EnrolmentKey> DisableAsync(EnrolmentKeyId enrolmentKeyId)
    {
        var result = await HttpClient.PutAsync($"{_orgRoute}/enrolment-keys/{enrolmentKeyId}/disable", null);

        var model = await DeserialiseAsync<EnrolmentKey>(result.Content);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public async Task<int> BulkEnableAsync(params EnrolmentKeyId[] enrolmentKeys)
    {
        var requestModel = new
        {
            keyIds = enrolmentKeys,
        };

        var result = await HttpClient.PutAsJsonAsync($"{_orgRoute}/enrolment-keys/enable", requestModel, Constants.JsonSerializerOptions);

        result.EnsureSuccessStatusCode();

        var model = await DeserialiseAsync<BulkKeyActionResult>(result.Content);

        EnsureNotNull(model);

        return model.KeysModified;
    }

    /// <inheritdoc/>
    public async Task<int> BulkEnableAsync(IEnumerable<EnrolmentKeyId> enrolmentKeys)
    {
        return await BulkEnableAsync(enrolmentKeys.ToArray());
    }

    /// <inheritdoc/>
    public async Task<int> BulkDisableAsync(params EnrolmentKeyId[] enrolmentKeys)
    {
        var requestModel = new
        {
            keyIds = enrolmentKeys,
        };

        var result = await HttpClient.PutAsJsonAsync($"{_orgRoute}/enrolment-keys/disable", requestModel, Constants.JsonSerializerOptions);

        result.EnsureSuccessStatusCode();

        var model = await DeserialiseAsync<BulkKeyActionResult>(result.Content);

        EnsureNotNull(model);

        return model.KeysModified;
    }

    /// <inheritdoc/>
    public async Task<int> BulkDisableAsync(IEnumerable<EnrolmentKeyId> enrolmentKeys)
    {
        return await BulkDisableAsync(enrolmentKeys.ToArray());
    }

    private static string? BuildQueryString(string? searchTerm, bool? includeDisabled, EnrolmentKeySortOrder? sortOrder, int? pageNumber, int? perPage)
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