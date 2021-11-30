namespace Enclave.Sdk.Api.Data.System;

/// <summary>
/// The possible states of an enrolled system.
/// </summary>
public enum SystemState
{
    /// <summary>
    /// The system has been explicitly disabled.
    /// </summary>
    Disabled,

    /// <summary>
    /// The system is disconnected from the Enclave cloud services.
    /// </summary>
    Disconnected,

    /// <summary>
    /// The system is connected to Enclave cloud services.
    /// </summary>
    Connected,
}