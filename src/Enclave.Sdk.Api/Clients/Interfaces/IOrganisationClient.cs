﻿using Enclave.Api.Modules.AccountManagement.PublicAccount.Models;
using Enclave.Api.Modules.OrganisationManagement;
using Enclave.Api.Modules.OrganisationManagement.Organisation.Models;
using Enclave.Sdk.Api.Data;

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
    AccountOrganisationModel Organisation { get; }

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
    /// Starts an update patch request.
    /// </summary>
    /// <returns>A PatchClient for fluent updating.</returns>
    IPatchClient<OrganisationPatchModel, OrganisationPropertiesModel> Update();
}
