using Enclave.Sdk.Api.Data.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enclave.Sdk.Api.Data.Organisations;

/// <summary>
/// Defines the properties of a user's membership of an organisation.
/// </summary>
public class OrganisationUser
{
    /// <summary>
    /// The account ID.
    /// </summary>
    public string Id { get; init; }

    /// <summary>
    /// The user email address.
    /// </summary>
    public string EmailAddress { get; init; }

    /// <summary>
    /// The user first name.
    /// </summary>
    public string FirstName { get; init; }

    /// <summary>
    /// The user last name.
    /// </summary>
    public string LastName { get; init; }

    /// <summary>
    /// The UTC timestamp for when the user joined the organisation.
    /// </summary>
    public DateTime JoinDate { get; init; }

    /// <summary>
    /// The user's role in the organisation.
    /// </summary>
    public UserOrganisationRole Role { get; init; }
}

public class OrganisationUsersTopLevel
{
    public IReadOnlyList<OrganisationUser> Users { get; init; }
}