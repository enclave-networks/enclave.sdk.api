using System.Net.Http.Json;
using System.Web;
using Enclave.Api.Modules.ActivityLogs.Logs.Models;
using Enclave.Api.Scaffolding.Pagination.Models;
using Enclave.Sdk.Api.Clients.Interfaces;

namespace Enclave.Sdk.Api.Clients;

/// <inheritdoc cref="ILogsClient"/>
internal class LogsClient : ClientBase, ILogsClient
{
    private readonly string _orgRoute;

    /// <summary>
    /// Constructor which will be called by <see cref="OrganisationClient"/> when it's created.
    /// </summary>
    /// <param name="httpClient">an instance of httpClient with a baseURL referencing the API.</param>
    /// <param name="orgRoute">the orgRoute which specifies the orgId.</param>
    public LogsClient(HttpClient httpClient, string orgRoute)
    : base(httpClient)
    {
        _orgRoute = orgRoute;
    }

    /// <inheritdoc/>
    public async Task<PaginatedResponseModel<LogEntryModel>> GetLogsAsync(int? pageNumber = null, int? perPage = null)
    {
        var queryString = BuildQueryString(pageNumber, perPage);

        var model = await HttpClient.GetFromJsonAsync<PaginatedResponseModel<LogEntryModel>>($"{_orgRoute}/logs?{queryString}", Constants.JsonSerializerOptions);

        EnsureNotNull(model);

        return model;
    }

    private static string? BuildQueryString(int? pageNumber, int? perPage)
    {
        var queryString = HttpUtility.ParseQueryString(string.Empty);
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
