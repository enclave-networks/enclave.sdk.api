using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enclave.Sdk.Api.Data.Organisations;

public class OrganisationUser
{
    // TODO Do we want to use account GUID?
    public string Id { get; init; }

    public string EmailAddress { get; init; }

    public string FirstName { get; init; }

    public string LastName { get; init; }

    public DateTime JoinDate { get; init; }

    // TODO Impliment ENUM?
    public string Role { get; init; }
}

public class OrganisationUsersTopLevel
{
    public IReadOnlyList<OrganisationUser> Users { get; init; }
}