using Enclave.Sdk.Api.Data.EnrolledSystems.Enum;

namespace Enclave.Sdk.Api.Data.SystemManagement;

/// <summary>
/// Defines a system reference model.
/// </summary>
public class SystemReference
{
    /// <summary>
    /// Contains the last connected IP of the system.
    /// </summary>
    public string? ConnectedFrom { get; init; }

    /// <summary>
    /// The System ID.
    /// </summary>
    public string Id { get; init; }

    /// <summary>
    /// The local hostname of the system (if known).
    /// </summary>
    public string? MachineName { get; init; }

    /// <summary>
    /// The set name of the system (if one was provided).
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// The System platform type (if known).
    /// </summary>
    public string? PlatformType { get; init; }

    /// <summary>
    /// The state of the system.
    /// </summary>
    public SystemState State { get; init; }
}
