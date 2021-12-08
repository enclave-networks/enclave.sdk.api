namespace Enclave.Sdk.Api.Data.PatchModel;

/// <summary>
/// Patch model containing properties that can be updated.
/// </summary>
public class UnapprovedSystemPatch : IPatchModel
{
    /// <summary>
    /// The system description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The set of tags applied to the system.
    /// </summary>
    public IReadOnlyList<string>? Tags { get; set; }

    /// <summary>
    /// Any notes or additional info for this system.
    /// </summary>
    public string? Notes { get; set; }
}
