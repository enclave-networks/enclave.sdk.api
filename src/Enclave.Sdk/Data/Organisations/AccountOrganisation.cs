using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Enclave.Sdk.Api.Data.Organisations;

public class AccountOrganisation
{
    // TODO: Set as Organisation Data Type
    [JsonPropertyName("orgId")]
    public string OrgId { get; init; }

    [JsonPropertyName("orgName")]
    public string OrgName { get; init; }

    [JsonPropertyName("role")]
    public string Role { get; init; }
}

public class AccountOrganisationTopLevel
{
    [JsonPropertyName("orgs")]
    public List<AccountOrganisation> Orgs { get; init; }
}