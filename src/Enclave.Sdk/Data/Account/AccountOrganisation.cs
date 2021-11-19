using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Enclave.Sdk.Api.Data.Account;

public class AccountOrganisation
{
    // TODO: Set as Organisation Data Type
    /// <summary>
    /// The organisation ID.
    /// </summary>
    public string OrgId { get; init; }

    /// <summary>
    /// The organisation name.
    /// </summary>
    public string OrgName { get; init; }

    /// <summary>
    /// The user's role within the organisation.
    /// </summary>
    public UserOrganisationRole Role { get; init; }
}

public class AccountOrganisationTopLevel
{
    [JsonPropertyName("orgs")]
    public List<AccountOrganisation> Orgs { get; init; }
}