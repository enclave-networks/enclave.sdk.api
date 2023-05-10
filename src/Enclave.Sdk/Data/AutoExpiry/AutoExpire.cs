using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enclave.Sdk.Api.Data.AutoExpiry;

/// <summary>
/// Contains the information required to automatically expire an item (e.g an enrolment key), including
/// how long to wait before expiry and what to do upon expiry.
/// </summary>
public class AutoExpire
{
    /// <summary>
    /// An IANA or Windows time zone ID. If this isn't null,  <see cref="ExpiryDateTime"/> will be updated if the specified time zone's rules change.
    /// If not, it won't be.
    /// </summary>
    public string? TimeZoneId { get; set; }

    /// <summary>
    /// The ISO 8601 date and time at which the item will expire.
    /// </summary>
    public string ExpiryDateTime { get; set; } = default!;

    /// <summary>
    /// The action to take upon expiry.
    /// </summary>
    public ExpiryAction ExpiryAction { get; set; }
}
