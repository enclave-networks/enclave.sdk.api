using Enclave.Api.Modules.ActivityLogs.Logs.Models;
using Enclave.Api.Scaffolding.Pagination.Models;

namespace Enclave.Sdk.Api.Clients.Interfaces;

/// <summary>
/// Access and search for logs.
/// </summary>
public interface ILogsClient
{
    /// <summary>
    /// Gets a paginated list of logs which can be searched and interated upon.
    /// </summary>
    /// <param name="pageNumber">Which page number do you want to return.</param>
    /// <param name="perPage">How many per page.</param>
    /// <returns>A paginated response model with links to get the previous, next, first and last pages.</returns>
    Task<PaginatedResponseModel<LogEntryModel>> GetLogsAsync(int? pageNumber = null, int? perPage = null);
}