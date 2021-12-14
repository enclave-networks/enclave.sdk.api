namespace Enclave.Sdk.Api.Data.EnrolmentKeys.Enum;

/// <summary>
/// The status of an enrolment key.
/// </summary>
public enum EnrolmentKeyStatus
{
    /// <summary>
    /// The key is disabled, so cannot be used.
    /// </summary>
    Disabled,

    /// <summary>
    /// The key is enabled, and can be used.
    /// </summary>
    Enabled,

    /// <summary>
    /// The key is enabled, but has no uses left, so cannot be used.
    /// </summary>
    NoUsesRemaining,
}