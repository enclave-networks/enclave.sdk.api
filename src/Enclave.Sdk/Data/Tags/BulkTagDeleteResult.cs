namespace Enclave.Sdk.Api.Data.Policies;

/// <summary>
/// Model for the result of a tag bulk delete operation.
/// </summary>
public class BulkTagDeleteResult
{
    /// <summary>
    /// The number of tags successfully deleted.
    /// </summary>
    public int TagsDeleted { get; init; }
}
