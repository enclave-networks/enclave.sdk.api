namespace Enclave.Sdk.Api.Data.Authority;

/// <summary>
/// Model for an enrolment request.
/// </summary>
public class EnrolRequest
{
    /// <summary>
    /// A 256-bit (32 byte) public key for signing, in base-64.
    /// </summary>
    public string PublicKey { get; set; } = default!;

    /// <summary>
    /// An Enclave Enrolment Key.
    /// </summary>
    public string EnrolmentKey { get; set; } = default!;

    /// <summary>
    /// A 256-bit (32 byte) nonce for the request, in base-64.
    /// </summary>
    public string Nonce { get; set; } = default!;

    /// <summary>
    /// Unix epoch timestamp from which the certificate becomes valid.
    /// If none provided, defaults to now.
    /// </summary>
    public long? NotBefore { get; set; }

    /// <summary>
    /// Unix epoch timestamp until which the certificate is valid. If none provided, defaults to a permanent certificate.
    /// </summary>
    public long? NotAfter { get; set; }
}
