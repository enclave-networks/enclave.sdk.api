using System.Text.Json.Serialization;

namespace Enclave.Sdk.Api.Data.Account;

/// <summary>
/// Contains the role an account has within an organisation.
/// </summary>
public class AccountOrganisation
{
    /// <summary>
    /// The organisation ID.
    /// </summary>
    public OrganisationId OrgId { get; init; }

    /// <summary>
    /// The organisation name.
    /// </summary>
    public string OrgName { get; init; } = default!;

    /// <summary>
    /// The user's role within the organisation.
    /// </summary>
    public UserOrganisationRole Role { get; init; }
}

/// <summary>
/// Account orgs response model.
/// </summary>
public class AccountOrganisationTopLevel
{
    /// <summary>
    /// The set of organisations.
    /// </summary>
    public IReadOnlyList<AccountOrganisation> Orgs { get; init; } = default!;
}