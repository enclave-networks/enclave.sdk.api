using Enclave.Sdk.Api.Data;
using Enclave.Sdk.Api.Data.Account;
using Enclave.Sdk.Api.Data.Organisations;
using Enclave.Sdk.Api.Data.PatchModel;

namespace Enclave.Sdk.Api.Clients.Interfaces;

/// <summary>
/// Provides access to organisation level API calls and organisation related clients.
/// For more information please refer to our API docs at https://api.enclave.io/.
/// </summary>
public interface IOrganisationClient
{
    /// <summary>
    /// The organisation selected and the one used to create this client.
    /// </summary>
    AccountOrganisation Organisation { get; }

    /// <summary>
    /// An instance of <see cref="AuthorityClient"/> associated with the current organisaiton.
    /// </summary>
    IAuthorityClient Authority { get; }

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
    ISystemsClient Systems { get; }

    /// <summary>
    /// An instance of <see cref="TagsClient"/> associated with the current organisaiton.
    /// </summary>
    ITagsClient Tags { get; }

    /// <summary>
    /// An instance of <see cref="UnapprovedSystemsClient"/> associated with the current organisaiton.
    /// </summary>
    IUnapprovedSystemsClient UnapprovedSystems { get; }

    /// <summary>
    /// Get more detail on your current organisaiton.
    /// </summary>
    /// <returns>A detailed organisation model.</returns>
    Task<Organisation?> GetAsync();

    /// <summary>
    /// Gets the users that have access to the current organisaiton.
    /// </summary>
    /// <returns>List of users associated with the current organisation.</returns>
    Task<IReadOnlyList<OrganisationUser>?> GetOrganisationUsersAsync();

    /// <summary>
    /// Get a list of invites that haven't been accepted.
    /// </summary>
    /// <returns>ReadOnlyList of pending invites.</returns>
    Task<IReadOnlyList<OrganisationInvite>> GetPendingInvitesAsync();

    /// <summary>
    /// Invite a user provided they haven't already been invited.
    /// </summary>
    /// <param name="emailAddress">Email address of the user you want to invite.</param>
    Task InviteUserAsync(string emailAddress);

    /// <summary>
    /// Cancel and invite before it's accepted.
    /// </summary>
    /// <param name="emailAddress">Email address of the user who's invite you want to revoke.</param>
    Task CancelInviteAync(string emailAddress);

    /// <summary>
    /// Removes a user from the organisation.
    /// </summary>
    /// <param name="accountId">The id of the users you want to remove.</param>
    Task RemoveUserAsync(string accountId);

    /// <summary>
    /// Patch request to update the organisation.
    /// Use Builder.cs to help you generate the dictionary.
    /// </summary>
    /// <param ref="Builder" name="builder">An instance of <see cref="PatchBuilder{TModel}"/> used to setup our patch request.</param>
    /// <returns>The updated organisation.</returns>
    /// <exception cref="ArgumentNullException">Throws if builder is null.</exception>
    Task<Organisation> UpdateAsync(PatchBuilder<OrganisationPatch> builder);
}
