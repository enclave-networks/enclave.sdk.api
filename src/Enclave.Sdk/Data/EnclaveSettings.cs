using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enclave.Sdk.Api.Data;

/// <summary>
/// A representation of the enclave settings json file.
/// </summary>
public class EnclaveSettings
{
    /// <summary>
    /// the bearer token from encalve.
    /// </summary>
    public string? BearerToken { get; set; }

    /// <summary>
    /// The Api base url.
    /// </summary>
    public string? BaseUrl { get; set; }
}
