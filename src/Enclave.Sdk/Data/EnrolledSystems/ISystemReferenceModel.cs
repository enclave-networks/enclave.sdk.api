using Enclave.Sdk.Api.Data.EnrolledSystems.Enum;

namespace Enclave.Sdk.Api.Data.EnrolledSystems;

/// <summary>
/// Abstraction for a system reference model so we can mix placeholder and "actual" reference models.
/// </summary>
public interface ISystemReferenceModel
{
    /// <summary>
    /// Contains the last connected IP of the system.
    /// </summary>
    string? ConnectedFrom { get; }

    /// <summary>
    /// The System ID.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// The local hostname of the system (if known).
    /// </summary>
    string? MachineName { get; }

    /// <summary>
    /// The set name of the system (if one was provided).
    /// </summary>
    string? Name { get; }

    /// <summary>
    /// The System platform type (if known).
    /// </summary>
    string? PlatformType { get; }

    /// <summary>
    /// The state of the system.
    /// </summary>
    SystemState State { get; }
}
