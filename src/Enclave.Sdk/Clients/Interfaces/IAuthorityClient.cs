using Enclave.Sdk.Api.Data.Authority;

namespace Enclave.Sdk.Api.Clients.Interfaces;

/// <summary>
/// Provides operations enrol a new system.
/// </summary>
public interface IAuthorityClient
{
    /// <summary>
    /// Enrol a new system.
    /// </summary>
    /// <param name="requestModel">The Request model to enrol the system.</param>
    /// <returns>An EnrolResult model</returns>
    Task<EnrolResult> Enrol(EnrolRequest requestModel);
}