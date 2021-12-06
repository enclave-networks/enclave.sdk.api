namespace Enclave.Sdk.Api.Data.Policies.Enum;

/// <summary>
/// Defines the possible states of policies.
/// </summary>
public enum PolicyState
{
    /// <summary>
    /// Policy is disabled.
    /// </summary>
    Disabled,

    /// <summary>
    /// Policy is active.
    /// </summary>
    Active,

    /// <summary>
    /// Policy is enabled, but no traffic will flow, due to the lack of ACLs.
    /// </summary>
    InactiveNoAcls,
}