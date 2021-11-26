namespace Enclave.Sdk.Api.Data.PatchModel;

/// <summary>
/// Defines the modifiable properties of an account.
/// </summary>
public class AccountPatch : IPatchModel
{
    /// <summary>
    /// The user's declared first name.
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// The user's declared last name.
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Whether the user has indicated they would like to recieve marketing emails.
    /// </summary>
    public bool? EmailNotificationsEnabled { get; set; }
}