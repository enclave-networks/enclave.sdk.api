using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enclave.Sdk.Api.Data.Dns;

/// <summary>
/// Summary of DNS properties for the organisation.
/// </summary>
public class DnsSummary
{
    /// <summary>
    /// The number of zones in the organisation.
    /// </summary>
    public int ZoneCount { get; init; }

    /// <summary>
    /// The total number of DNS records in the organisation, across all zones.
    /// </summary>
    public int TotalRecordCount { get; init; }
}
