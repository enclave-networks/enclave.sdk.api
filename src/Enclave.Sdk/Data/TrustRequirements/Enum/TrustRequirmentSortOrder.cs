using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enclave.Sdk.Api.Data.TrustRequirements.Enum;

/// <summary>
/// The sort order when making a Trust Requirements Request.
/// </summary>
public enum TrustRequirementSortOrder
{
    /// <summary>
    /// Sort by description.
    /// </summary>
    Description,

    /// <summary>
    /// Sort by recently created.
    /// </summary>
    RecentlyCreated,
}
