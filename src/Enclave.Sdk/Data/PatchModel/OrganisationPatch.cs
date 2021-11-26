namespace Enclave.Sdk.Api.Data.PatchModel;

/// <summary>
/// Model defining the modifiable properties for an organisation.
/// </summary>
public class OrganisationPatch : IPatchModel
{
    /// <summary>
    /// The org name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// The org website.
    /// </summary>
    public string? Website { get; set; }

    /// <summary>
    /// A contact number for the organisation.
    /// </summary>
    public string? Phone { get; set; }
}
