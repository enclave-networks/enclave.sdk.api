using Enclave.Api.Modules.OrganisationManagement;
using Enclave.Api.Modules.OrganisationManagement.Organisation.Models;
using Enclave.Configuration.Data.Identifiers;
using Enclave.Sdk.Api.Data;

namespace Enclave.Sdk.Api.Clients.Interfaces;

/// <summary>
/// Provides access to organisation level API calls and organisation related clients for one organisation, known by its ID.
/// For more information please refer to our API docs at https://api.enclave.io/.
/// </summary>
public interface IOrganisationScopedClient
{
    /// <summary>
    /// The ID of the organisation this client sends its calls to.
    /// </summary>
    OrganisationGuid OrgId { get; }

    /// <summary>
    /// An instance of <see cref="DnsClient"/> associated with the current organisaiton.
    /// </summary>
    IDnsClient Dns { get; }

    /// <summary>
    /// An instance of <see cref="EnrolmentKeysClient"/> associated with the current organisaiton.
    /// </summary>
    IEnrolmentKeysClient EnrolmentKeys { get; }

    /// <summary>
    /// An instance of <see cref="LogsClient"/> associated with the current organisaiton.
    /// </summary>
    ILogsClient Logs { get; }

    /// <summary>
    /// An instance of <see cref="PoliciesClient"/> associated with the current organisaiton.
    /// </summary>
    IPoliciesClient Policies { get; }

    /// <summary>
    /// An instance of <see cref="SystemsClient"/> associated with the current organisaiton.
    /// </summary>
    ISystemsClient EnrolledSystems { get; }

    /// <summary>
    /// An instance of <see cref="TagsClient"/> associated with the current organisaiton.
    /// </summary>
    ITagsClient Tags { get; }

    /// <summary>
    /// An instance of <see cref="UnapprovedSystemsClient"/> associated with the current organisaiton.
    /// </summary>
    IUnapprovedSystemsClient UnapprovedSystems { get; }

    /// <summary>
    /// An instance of <see cref="TrustRequirementsClient"/> associated with the current organisation.
    /// </summary>
    ITrustRequirementsClient TrustRequirements { get; }

    /// <summary>
    /// Get more detail on your current organisaiton.
    /// </summary>
    /// <returns>A detailed organisation model.</returns>
    Task<OrganisationPropertiesModel?> GetAsync();

    /// <summary>
    /// Gets the users that have access to the current organisaiton.
    /// </summary>
    /// <returns>List of users associated with the current organisation.</returns>
    Task<IReadOnlyList<OrganisationUser>> GetOrganisationUsersAsync();

    /// <summary>
    /// Get a list of invites that haven't been accepted.
    /// </summary>
    /// <returns>ReadOnlyList of pending invites.</returns>
    Task<IReadOnlyList<OrganisationInviteModel>> GetPendingInvitesAsync();

    /// <summary>
    /// Invites a user to the organisation, by email. Inviting an address that already has a pending invite sends the
    /// invite again.
    /// </summary>
    /// <param name="emailAddress">Email address of the user you want to invite.</param>
    /// <returns>
    /// The invite. Its email address is the one the invite was first sent to, which can differ in case from
    /// <paramref name="emailAddress"/>.
    /// </returns>
    /// <exception cref="InvalidOperationException">Throws if the API's successful response has no invite in its body.</exception>
    Task<OrganisationInviteModel> InviteUserAsync(string emailAddress);

    /// <summary>
    /// Cancels an invite before it's accepted.
    /// </summary>
    /// <param name="emailAddress">Email address of the user whose invite you want to revoke.</param>
    /// <returns>
    /// The cancelled invite. Its email address is the one the invite was sent to, which can differ in case from
    /// <paramref name="emailAddress"/>.
    /// </returns>
    /// <exception cref="InvalidOperationException">Throws if the API's successful response has no invite in its body.</exception>
    Task<OrganisationInviteModel> CancelInviteAync(string emailAddress);

    /// <summary>
    /// Removes a user from the organisation.
    /// </summary>
    /// <param name="accountId">The account ID of the user you want to remove.</param>
    /// <returns>The removed user, with the role they held and the date they joined.</returns>
    /// <exception cref="ArgumentException">Throws if <paramref name="accountId"/> is null, empty, "." or "..".</exception>
    /// <exception cref="InvalidOperationException">Throws if the API's successful response has no user in its body.</exception>
    Task<OrganisationUser> RemoveUserAsync(string accountId);

    /// <summary>
    /// Starts an update patch request.
    /// </summary>
    /// <returns>A PatchClient for fluent updating.</returns>
    IPatchClient<OrganisationPatchModel, OrganisationPropertiesModel> Update();
}
