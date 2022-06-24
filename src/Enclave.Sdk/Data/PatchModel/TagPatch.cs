using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enclave.Sdk.Api.Data.PatchModel;

/// <summary>
/// Defines the modifiable properties of a tag.
/// </summary>
public class TagPatch : IPatchModel
{
    /// <summary>
    /// The tag name.
    /// </summary>
    public string? Tag { get; set; }

    /// <summary>
    /// An optional custom tag colour.
    /// </summary>
    public string? Colour { get; set; }

    /// <summary>
    /// Any notes or additional info for this tag.
    /// </summary>
    public string? Notes { get; set; }

    //public TrustRequirementId[]? TrustRequirements { get; set; }
}
