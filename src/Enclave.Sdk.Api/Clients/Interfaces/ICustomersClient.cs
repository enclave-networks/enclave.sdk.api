using Enclave.Api.Scaffolding.Pagination.Models;
using Enclave.Configuration.Data.Identifiers;
using Enclave.Configuration.Data.Modules.Organisation.Enums;
using Enclave.Sdk.Api.Data;
using Enclave.Sdk.Api.Partner.Models;

namespace Enclave.Sdk.Api.Clients.Interfaces;

/// <summary>
/// A partner's customers, with their admins, admin invites and auto-sync. A customer is an Enclave organisation, and is
/// identified by its organisation ID. Reading needs a token with the ReadCustomers scope. Changes, and listing invites,
/// need the WriteCustomers scope and the Owner or Admin role in the partner.
/// </summary>
public interface ICustomersClient
{
    /// <summary>
    /// Gets a page of the partner's customers. Setting any of <paramref name="year"/>, <paramref name="month"/> and
    /// <paramref name="day"/> returns the customers as the partner's usage report for that date recorded them, in place
    /// of their current state.
    /// </summary>
    /// <param name="searchTerm">A partial match on the customer's name.</param>
    /// <param name="sortOrder">The order of the customers. The API sorts by name when it is not given.</param>
    /// <param name="canAdminOnly">True to return only customers the user is an admin of.</param>
    /// <param name="year">The year of the usage report. The API takes the current year when it is not given.</param>
    /// <param name="month">The month of the usage report. The API takes the current month when it is not given.</param>
    /// <param name="day">The day of the usage report. The API takes the partner's monthly reporting day when it is not given.</param>
    /// <param name="pageNumber">Which page to return.</param>
    /// <param name="perPage">How many customers per page.</param>
    /// <returns>A paginated response model with links to get the previous, next, first and last pages.</returns>
    Task<PaginatedResponseModel<CustomerModel>> GetCustomersAsync(
        string? searchTerm = null,
        PartnerOrganisationSortOrder? sortOrder = null,
        bool? canAdminOnly = null,
        int? year = null,
        int? month = null,
        int? day = null,
        int? pageNumber = null,
        int? perPage = null);

    /// <summary>
    /// Gets a customer.
    /// </summary>
    /// <param name="customerId">The ID of the customer's organisation.</param>
    /// <returns>The <see cref="CustomerModel"/>.</returns>
    Task<CustomerModel> GetAsync(OrganisationGuid customerId);

    /// <summary>
    /// Creates a customer, with a new organisation.
    /// </summary>
    /// <param name="createModel">The new customer. Every field is sent.</param>
    /// <returns>The created <see cref="CustomerModel"/>.</returns>
    /// <exception cref="ArgumentNullException">Throws if <paramref name="createModel"/> is null.</exception>
    Task<CustomerModel> CreateAsync(CustomerCreateModel createModel);

    /// <summary>
    /// Starts an update patch request.
    /// </summary>
    /// <param name="customerId">The ID of the customer's organisation.</param>
    /// <returns>A PatchClient for fluent updating.</returns>
    IPatchClient<CustomerPatchModel, CustomerModel> Update(OrganisationGuid customerId);

    /// <summary>
    /// Converts a customer to paid.
    /// </summary>
    /// <param name="customerId">The ID of the customer's organisation.</param>
    /// <param name="billingPeriodMonths">The billing period in months: 1, 12, 24 or 36.</param>
    /// <returns>The converted <see cref="CustomerModel"/>.</returns>
    Task<CustomerModel> ConvertAsync(OrganisationGuid customerId, int billingPeriodMonths);

    /// <summary>
    /// Gets the owners and admins of a customer's organisation.
    /// </summary>
    /// <param name="customerId">The ID of the customer's organisation.</param>
    /// <returns>The owners and admins.</returns>
    Task<IReadOnlyList<CustomerUserModel>> GetAdminsAsync(OrganisationGuid customerId);

    /// <summary>
    /// Makes one of the partner's users an admin of a customer's organisation. To add someone who is not one of the
    /// partner's users, invite them with <see cref="InviteAdminAsync"/>.
    /// </summary>
    /// <param name="customerId">The ID of the customer's organisation.</param>
    /// <param name="accountId">The account ID of one of the partner's users.</param>
    /// <returns>The added admin. The API gives the role as Admin, and the account's creation date as the join date.</returns>
    Task<CustomerUserModel> AddAdminAsync(OrganisationGuid customerId, AccountGuid accountId);

    /// <summary>
    /// Removes a user from a customer's organisation.
    /// </summary>
    /// <param name="customerId">The ID of the customer's organisation.</param>
    /// <param name="accountId">The account ID of the user to remove.</param>
    /// <returns>The removed user. The API gives the role as Admin, and the account's creation date as the join date.</returns>
    Task<CustomerUserModel> RemoveAdminAsync(OrganisationGuid customerId, AccountGuid accountId);

    /// <summary>
    /// Gets the pending admin invites of a customer's organisation. This needs the WriteCustomers scope.
    /// </summary>
    /// <param name="customerId">The ID of the customer's organisation.</param>
    /// <returns>The pending invites.</returns>
    Task<IReadOnlyList<CustomerAdminInviteModel>> GetPendingInvitesAsync(OrganisationGuid customerId);

    /// <summary>
    /// Invites a user to be an admin of a customer's organisation, by email. Inviting an address that already has a
    /// pending invite sends the invite again.
    /// </summary>
    /// <param name="customerId">The ID of the customer's organisation.</param>
    /// <param name="emailAddress">The email address of the user to invite.</param>
    /// <returns>The invite.</returns>
    /// <exception cref="ArgumentException">Throws if <paramref name="emailAddress"/> is null, empty or white space.</exception>
    Task<CustomerAdminInviteModel> InviteAdminAsync(OrganisationGuid customerId, string emailAddress);

    /// <summary>
    /// Cancels a pending admin invite.
    /// </summary>
    /// <param name="customerId">The ID of the customer's organisation.</param>
    /// <param name="inviteId">The ID of the invite, from <see cref="GetPendingInvitesAsync"/>.</param>
    /// <returns>The cancelled invite. The API's response has no ID, so its <see cref="CustomerAdminInviteModel.Id"/> is <paramref name="inviteId"/>.</returns>
    /// <exception cref="ArgumentException">Throws if <paramref name="inviteId"/> is empty, "." or "..".</exception>
    Task<CustomerAdminInviteModel> CancelInviteAsync(OrganisationGuid customerId, OrganisationInviteId inviteId);

    /// <summary>
    /// Enables admin auto-sync for a customer.
    /// </summary>
    /// <param name="customerId">The ID of the customer's organisation.</param>
    /// <returns>The updated <see cref="CustomerModel"/>.</returns>
    /// <exception cref="InvalidOperationException">Throws if the API returns no customer, which it does when the partner has no customer with that ID.</exception>
    Task<CustomerModel> EnableAutoSyncAsync(OrganisationGuid customerId);

    /// <summary>
    /// Disables admin auto-sync for a customer.
    /// </summary>
    /// <param name="customerId">The ID of the customer's organisation.</param>
    /// <returns>The updated <see cref="CustomerModel"/>.</returns>
    /// <exception cref="InvalidOperationException">Throws if the API returns no customer, which it does when the partner has no customer with that ID.</exception>
    Task<CustomerModel> DisableAutoSyncAsync(OrganisationGuid customerId);
}
