﻿using Enclave.Sdk.Api.Data.Account;
using Enclave.Sdk.Api.Data.Organisations;

namespace Enclave.Sdk.Api.Clients.Interfaces;

public interface IOrganisationClient
{
    AccountOrganisation CurrentOrganisation { get; }

    IAuthorityClient Authority { get; }

    IDnsClient Dns { get; }

    IEnrolmentKeysClient EnrolmentKeys { get; }

    ILogsClient Logs { get; }

    IPoliciesClient Policies { get; }

    ISystemsClient Systems { get; }

    ITagsClient Tags { get; }

    IUnapprovedSystemsClient UnapprovedSystems { get; }

    Task CancelInviteAync(string emailAddress);

    Task<Organisation?> GetAsync();

    Task<OrganisationPricing> GetOrganisationPricing();

    Task<IReadOnlyList<OrganisationUser>?> GetOrganisationUsersAsync();

    Task<OrganisationPendingInvites> GetPendingInvitesAsync();

    Task InviteUserAsync(string emailAddress);

    Task RemoveUserAsync(string accountId);

    Task<Organisation> UpdateAsync(Dictionary<string, object> updatedModel);
}
