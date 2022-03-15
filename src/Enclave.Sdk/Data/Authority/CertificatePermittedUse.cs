namespace Enclave.Sdk.Api.Data.Authority;

/// <summary>
/// Defines bit-flags indicating what a certificate can be used for.
/// </summary>
[Flags]
public enum CertificatePermittedUse : byte
{
    /// <summary>
    /// No Permitted Uses.
    /// </summary>
    None = 0x0,

    /// <summary>
    /// For individual endpoints.
    /// </summary>
    /// <remarks>
    /// Certificate commonName is assigned by the root or intermediate, the public key owners identity is not validated.
    /// Certificate may only be signed by an intermediate or root. issued to a primary key.
    /// Certificate may not be used to signed by another. Certificates signed by endpoints are considered invalid.
    /// </remarks>
    Endpoint = 0x2,

    /// <summary>
    /// Special class of endpoint certificate reserved for operational infrastructure, discovery service, relay services etc.
    /// </summary>
    Infrastructure = 0x4,

    /// <summary>
    /// For intermediate level certificates
    /// ===================================
    /// This class may only sign endpoints (class 0), and can only be signed by a root.
    /// </summary>
    Intermediate = 0x8,

    /// <summary>
    /// For root level certificates
    /// ===========================
    /// This class may only be used to sign intermediates and must be signed with own public key.
    /// </summary>
    Root = 0x10,
}
