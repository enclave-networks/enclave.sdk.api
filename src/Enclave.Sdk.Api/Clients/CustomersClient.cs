using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Web;
using Enclave.Api.Scaffolding.Pagination.Models;
using Enclave.Configuration.Data.Identifiers;
using Enclave.Configuration.Data.Modules.Organisation.Enums;
using Enclave.Sdk.Api.Clients.Interfaces;
using Enclave.Sdk.Api.Data;
using Enclave.Sdk.Api.Partner.Models;

namespace Enclave.Sdk.Api.Clients;

/// <inheritdoc cref="ICustomersClient"/>
internal sealed class CustomersClient : ClientBase, ICustomersClient
{
    // The routes, methods, bodies and responses are those of the partner API's CustomersController (portal
    // src/Enclave.Partner.Api/Modules/CustomerManagement/Customers/CustomersController.cs). Customer and
    // account IDs are typed GUIDs that format as 32 hex digits, so they go into paths as they are; an invite
    // ID is string-backed, can hold any text, and goes through PathSegment.
    private readonly string _customersRoute;

    /// <summary>
    /// Constructor which will be called by <see cref="PartnerClient"/> when it's created.
    /// </summary>
    /// <param name="httpClient">an instance of httpClient with a baseURL referencing the partner API.</param>
    /// <param name="partnerRoute">the partner route which specifies the partner ID.</param>
    public CustomersClient(HttpClient httpClient, string partnerRoute)
        : base(httpClient)
    {
        _customersRoute = $"{partnerRoute}/customers";
    }

    /// <inheritdoc/>
    public async Task<PaginatedResponseModel<CustomerModel>> GetCustomersAsync(
        string? searchTerm = null,
        PartnerOrganisationSortOrder? sortOrder = null,
        bool? canAdminOnly = null,
        int? year = null,
        int? month = null,
        int? day = null,
        int? pageNumber = null,
        int? perPage = null)
    {
        var queryString = BuildQueryString(searchTerm, sortOrder, canAdminOnly, year, month, day, pageNumber, perPage);
        var route = queryString.Length == 0 ? _customersRoute : $"{_customersRoute}?{queryString}";

        var model = await HttpClient.GetFromJsonAsync<PaginatedResponseModel<CustomerModel>>(route, Constants.JsonSerializerOptions);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public async Task<CustomerModel> GetAsync(OrganisationGuid customerId)
    {
        var model = await HttpClient.GetFromJsonAsync<CustomerModel>(CustomerRoute(customerId), Constants.JsonSerializerOptions);

        EnsureNotNull(model);

        return model;
    }

    /// <inheritdoc/>
    public async Task<CustomerModel> CreateAsync(CustomerCreateModel createModel)
    {
        ArgumentNullException.ThrowIfNull(createModel);

        using var content = CreateJsonContent(createModel);
        using var response = await HttpClient.PostAsync(_customersRoute, content);

        return await ReadAsync<CustomerModel>(response);
    }

    /// <inheritdoc/>
    public IPatchClient<CustomerPatchModel, CustomerModel> Update(OrganisationGuid customerId)
    {
        return new PatchClient<CustomerPatchModel, CustomerModel>(HttpClient, CustomerRoute(customerId));
    }

    /// <inheritdoc/>
    public async Task<CustomerModel> ConvertAsync(OrganisationGuid customerId, int billingPeriodMonths)
    {
        using var content = CreateJsonContent(new ConvertCustomerModel { BillingPeriodMonths = billingPeriodMonths });
        using var response = await HttpClient.PutAsync($"{CustomerRoute(customerId)}/convert", content);

        return await ReadAsync<CustomerModel>(response);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<CustomerUserModel>> GetAdminsAsync(OrganisationGuid customerId)
    {
        var model = await HttpClient.GetFromJsonAsync<CustomerUsersModel>($"{CustomerRoute(customerId)}/admins", Constants.JsonSerializerOptions);

        EnsureNotNull(model);

        return model.Users ?? Array.Empty<CustomerUserModel>();
    }

    /// <inheritdoc/>
    public async Task<CustomerUserModel> AddAdminAsync(OrganisationGuid customerId, AccountGuid accountId)
    {
        using var response = await HttpClient.PutAsync($"{CustomerRoute(customerId)}/admins/{accountId}", content: null);

        return await ReadAsync<CustomerUserModel>(response);
    }

    /// <inheritdoc/>
    public async Task<CustomerUserModel> RemoveAdminAsync(OrganisationGuid customerId, AccountGuid accountId)
    {
        using var response = await HttpClient.DeleteAsync($"{CustomerRoute(customerId)}/admins/{accountId}");

        return await ReadAsync<CustomerUserModel>(response);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<CustomerAdminInviteModel>> GetPendingInvitesAsync(OrganisationGuid customerId)
    {
        var model = await HttpClient.GetFromJsonAsync<CustomerPendingAdminInvitesModel>($"{CustomerRoute(customerId)}/invites", Constants.JsonSerializerOptions);

        EnsureNotNull(model);

        return model.Invites ?? Array.Empty<CustomerAdminInviteModel>();
    }

    /// <inheritdoc/>
    public async Task<CustomerAdminInviteModel> InviteAdminAsync(OrganisationGuid customerId, string emailAddress)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(emailAddress);

        using var content = CreateJsonContent(new CreateCustomerAdminInviteModel { EmailAddress = emailAddress });
        using var response = await HttpClient.PostAsync($"{CustomerRoute(customerId)}/invites", content);

        return await ReadAsync<CustomerAdminInviteModel>(response);
    }

    /// <inheritdoc/>
    public async Task<CustomerAdminInviteModel> CancelInviteAsync(OrganisationGuid customerId, OrganisationInviteId inviteId)
    {
        // A default OrganisationInviteId formats as null, which PathSegment refuses along with "", "." and "..".
        var route = $"{CustomerRoute(customerId)}/invites/{PathSegment(inviteId.ToString(), nameof(inviteId))}";

        using var response = await HttpClient.DeleteAsync(route);

        var model = await ReadAsync<CustomerAdminInviteModel>(response);

        // The API's response carries the email address and leaves the ID unset, which it writes as null (portal
        // .../Customers/Handlers/DeleteCustomerPendingAdminInviteHandler.cs). A default OrganisationInviteId
        // throws NullReferenceException from Equals, == and GetHashCode (Enclave.Sdk.Api.Data 304.48.0, whose
        // string-backed TypedIds compare the null value), so a model holding one breaks a caller's comparison or
        // dictionary. The API deletes the invite whose ID is the one in the route (portal Enclave.Configuration.Data
        // OrganisationRepository.DeleteOrganisationInviteAsync), so that ID is the cancelled invite's. Its
        // ToString is checked because Equals cannot be called on a default ID.
        if (model.Id.ToString() is null)
        {
            return new CustomerAdminInviteModel { Id = inviteId, EmailAddress = model.EmailAddress };
        }

        return model;
    }

    /// <inheritdoc/>
    public Task<CustomerModel> EnableAutoSyncAsync(OrganisationGuid customerId)
    {
        return SetAutoSyncAsync(customerId, "enable-auto-sync");
    }

    /// <inheritdoc/>
    public Task<CustomerModel> DisableAutoSyncAsync(OrganisationGuid customerId)
    {
        return SetAutoSyncAsync(customerId, "disable-auto-sync");
    }

    private static async Task<TModel> ReadAsync<TModel>(HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();

        var model = await DeserialiseAsync<TModel>(response.Content);

        EnsureNotNull(model);

        return model;
    }

    private static string BuildQueryString(
        string? searchTerm,
        PartnerOrganisationSortOrder? sortOrder,
        bool? canAdminOnly,
        int? year,
        int? month,
        int? day,
        int? pageNumber,
        int? perPage)
    {
        // The names are those the API binds (portal .../Customers/Models/CustomerRequestModel.cs, and
        // Enclave.Api.Scaffolding/Pagination/Models/PaginatedRequestModel.cs for page and per_page).
        var queryString = HttpUtility.ParseQueryString(string.Empty);

        if (searchTerm is not null)
        {
            queryString.Add("search", searchTerm);
        }

        if (sortOrder is not null)
        {
            queryString.Add("sort", sortOrder.Value.ToString());
        }

        if (canAdminOnly is not null)
        {
            queryString.Add("CanAdminOnly", canAdminOnly.Value ? "true" : "false");
        }

        if (year is not null)
        {
            queryString.Add("year", year.Value.ToString(CultureInfo.InvariantCulture));
        }

        if (month is not null)
        {
            queryString.Add("month", month.Value.ToString(CultureInfo.InvariantCulture));
        }

        if (day is not null)
        {
            queryString.Add("day", day.Value.ToString(CultureInfo.InvariantCulture));
        }

        if (pageNumber is not null)
        {
            queryString.Add("page", pageNumber.Value.ToString(CultureInfo.InvariantCulture));
        }

        if (perPage is not null)
        {
            queryString.Add("per_page", perPage.Value.ToString(CultureInfo.InvariantCulture));
        }

        return queryString.ToString() ?? string.Empty;
    }

    private async Task<CustomerModel> SetAutoSyncAsync(OrganisationGuid customerId, string route)
    {
        using var response = await HttpClient.PutAsync($"{CustomerRoute(customerId)}/{route}", content: null);

        response.EnsureSuccessStatusCode();

        // For a customer the partner does not have, these two routes return Ok(null) (portal CustomersController.cs,
        // EnableAutoSync and DisableAutoSync, where ModifyCustomerHandler returns null), and ASP.NET Core sends a
        // null result as 204 No Content with no body
        // (https://learn.microsoft.com/aspnet/core/web-api/advanced/formatting#special-case-formatters). The other
        // routes answer an unknown customer with a problem+json 404, but these give no error, so the status is
        // what says there was no customer.
        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            throw new InvalidOperationException(
                $"The partner API returned no customer for {customerId} (204 No Content), which it does when the partner has no customer with that ID.");
        }

        var model = await DeserialiseAsync<CustomerModel>(response.Content);

        EnsureNotNull(model);

        return model;
    }

    private string CustomerRoute(OrganisationGuid customerId) => $"{_customersRoute}/{customerId}";
}
