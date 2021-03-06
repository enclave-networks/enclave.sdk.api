namespace Enclave.Sdk.Api.Data.Organisations;

/// <summary>
/// Model for the pending list of org invites.
/// </summary>
public class OrganisationPendingInvites
{
    /// <summary>
    /// The set of outstanding invites that have been sent for this organisation.
    /// </summary>
    public IReadOnlyList<OrganisationInvite>? Invites { get; init; }
}