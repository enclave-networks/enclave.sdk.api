using System.Net.Http.Json;
using System.Web;
using Enclave.Api.Modules.SystemManagement.Common.Models;
using Enclave.Api.Modules.SystemManagement.EnrolmentKeys.Models;
using Enclave.Api.Scaffolding.Pagination.Models;
using Enclave.Configuration.Data.Enums;
using Enclave.Configuration.Data.Identifiers;
using Enclave.Configuration.Data.Modules.EnrolmentKeys.Enums;
using Enclave.Sdk.Api.Clients.Interfaces;
using Enclave.Sdk.Api.Data;

namespace Enclave.Sdk.Api.Clients;

/// <inheritdoc cref="IEnrolmentKeysClient" />
internal class EnrolmentKeysClient : ClientBase, IEnrolmentKeysClient
{
    private readonly string _orgRoute;

    /// <summary>
    /// Constructor which will be called by <see cref="OrganisationClient"/> when it's created.
    /// </summary>
    /// <param name="httpClient">an instance of httpClient with a baseURL referencing the API.</param>
    /// <param name="orgRoute">The organisation API route.</param>
    public EnrolmentKeysClient(HttpClient httpClient, string orgRoute)
    : base(httpClient)
    {
        _orgRoute = orgRoute;
    }

    /// <inheritdoc/>
    public async Task<PaginatedResponseModel<EnrolmentKeySummaryModel>> GetEnrolmentKeysAsync(
        string? searchTerm = null,
        bool? includeDisabled = null,
        EnrolmentKeySortOrder? sortOrder = null,
        int? pageNumber = null,
        int? perPage = null)
    {
        var queryString = BuildQueryString(searchTerm, includeDisabled, sortOrder, pageNumber, perPage);

        var model = await HttpClient.GetFromJsonAsync<PaginatedResponseModel<EnrolmentKeySummaryModel>>($"{_orgRoute}/enrolment-keys?{queryString}", Constants.JsonSerializerOptions);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public async Task<EnrolmentKeyModel> CreateAsync(EnrolmentKeyCreateModel createModel)
    {
        if (createModel is null)
        {
            throw new ArgumentNullException(nameof(createModel));
        }

        var result = await HttpClient.PostAsJsonAsync($"{_orgRoute}/enrolment-keys", createModel, Constants.JsonSerializerOptions);

        var model = await DeserialiseAsync<EnrolmentKeyModel>(result.Content);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public async Task<EnrolmentKeyModel> GetAsync(EnrolmentKeyId enrolmentKeyId)
    {
        var model = await HttpClient.GetFromJsonAsync<EnrolmentKeyModel>($"{_orgRoute}/enrolment-keys/{enrolmentKeyId}", Constants.JsonSerializerOptions);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public IPatchClient<EnrolmentKeyPatchModel, EnrolmentKeyModel> Update(EnrolmentKeyId enrolmentKeyId)
    {
        return new PatchClient<EnrolmentKeyPatchModel, EnrolmentKeyModel>(HttpClient, $"{_orgRoute}/enrolment-keys/{enrolmentKeyId}");
    }

    /// <inheritdoc/>
    public async Task<EnrolmentKeyModel> EnableAsync(EnrolmentKeyId enrolmentKeyId)
    {
        var result = await HttpClient.PutAsync($"{_orgRoute}/enrolment-keys/{enrolmentKeyId}/enable", null);

        var model = await DeserialiseAsync<EnrolmentKeyModel>(result.Content);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public async Task<EnrolmentKeyModel> DisableAsync(EnrolmentKeyId enrolmentKeyId)
    {
        var result = await HttpClient.PutAsync($"{_orgRoute}/enrolment-keys/{enrolmentKeyId}/disable", null);

        var model = await DeserialiseAsync<EnrolmentKeyModel>(result.Content);

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

    /// <inheritdoc/>
    public async Task<EnrolmentKeyModel> EnableUntilAsync(EnrolmentKeyId enrolmentKeyId, DateTimeOffset expiryDateTime, ExpiryAction expiryAction, string? timeZonedId = null)
    {
        var requestModel = new AutoExpireModel(timeZonedId, expiryDateTime.ToString("o"), expiryAction);

        var result = await HttpClient.PutAsJsonAsync($"{_orgRoute}/enrolment-keys/{enrolmentKeyId}/enable-until", requestModel, Constants.JsonSerializerOptions);

        result.EnsureSuccessStatusCode();

        var model = await DeserialiseAsync<EnrolmentKeyModel>(result.Content);

        EnsureNotNull(model);

        return model;
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