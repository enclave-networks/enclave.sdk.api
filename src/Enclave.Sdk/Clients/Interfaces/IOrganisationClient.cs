using Enclave.Sdk.Api.Data.Account;
using Enclave.Sdk.Api.Data.Organisations;

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
    AccountOrganisation CurrentOrganisation { get; }

    /// <summary>
    /// An instance of AuthorityClient associated with the current organisaiton.
    /// </summary>
    IAuthorityClient Authority { get; }

    /// <summary>
    /// An instance of DnsClient associated with the current organisaiton.
    /// </summary>
    IDnsClient Dns { get; }

    /// <summary>
    /// An instance of EnrolmentKeysClient associated with the current organisaiton.
    /// </summary>
    IEnrolmentKeysClient EnrolmentKeys { get; }

    /// <summary>
    /// An instance of LogsClient associated with the current organisaiton.
    /// </summary>
    ILogsClient Logs { get; }

    /// <summary>
    /// An instance of PoliciesClient associated with the current organisaiton.
    /// </summary>
    IPoliciesClient Policies { get; }

    /// <summary>
    /// An instance of SystemsClient associated with the current organisaiton.
    /// </summary>
    ISystemsClient Systems { get; }

    /// <summary>
    /// An instance of TagsClient associated with the current organisaiton.
    /// </summary>
    ITagsClient Tags { get; }

    /// <summary>
    /// An instance of UnapprovedSystemsClient associated with the current organisaiton.
    /// </summary>
    IUnapprovedSystemsClient UnapprovedSystems { get; }

    /// <summary>
    /// Get more detail on your current organisaiton.
    /// </summary>
    /// <returns>A more detailed version of CurrentOrganisaiton.</returns>
    Task<Organisation?> GetAsync();

    /// <summary>
    /// Gets pricing options for the current organisaiton.
    /// </summary>
    /// <returns>A representation of the pricing options.</returns>
    Task<OrganisationPricing> GetOrganisationPricing();

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
    /// <param ref="Builder" name="updatedModel">Use Builder.cs to properly generate.</param>
    /// <returns>The updated organisation.</returns>
    Task<Organisation> UpdateAsync(Dictionary<string, object> updatedModel);
}
