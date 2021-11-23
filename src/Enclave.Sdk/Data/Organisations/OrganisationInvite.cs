using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enclave.Sdk.Api.Data.Organisations;

/// <summary>
/// Invite model.
/// </summary>
public class OrganisationInvite
{
    /// <summary>
    /// The email address of the user to invite.
    /// </summary>
    public string? EmailAddress { get; init; }
}
