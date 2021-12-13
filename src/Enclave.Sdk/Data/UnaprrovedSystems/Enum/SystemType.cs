namespace Enclave.Sdk.Api.Data.UnaprrovedSystems.Enum;

/// <summary>
/// Defines the types of Enrolment Keys.
/// </summary>
public enum SystemType
{
    /// <summary>
    /// For workstations, laptops, servers, and other systems that are relatively long-lived or manually provisioned.
    /// Systems remain in your Enclave Organisation if they stop running.
    /// </summary>
    GeneralPurpose,

    /// <summary>
    /// For containers, kubernetes pods, and other systems that are temporary, short-lived or automatically provisioned.
    /// Systems are automatically removed from your Enclave Organisation when they stop/disconnect.
    /// </summary>
    Ephemeral,
}
