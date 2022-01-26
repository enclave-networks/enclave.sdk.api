namespace Enclave.Sdk.Api.Data.Authority;

/// <summary>
/// Defines the response model for an issued certificate.
/// </summary>
public class EnrolResult
{
    /// <summary>
    /// The version number field denotes the version of the certificate.
    /// </summary>
    public int Version { get; init; }

    /// <summary>
    /// The unique serial number of the certificate assigned by the certification authority; the size of the serial number field is a 16 byte (128 bit) guid.
    /// </summary>
    /// <remarks>
    ///  <para>The value is unique to every certificate issued by a certification authority so as to allow identification of individual certificates.</para>
    ///  <para>The value may be randomly selected, or incremented by the certificate authority.</para>
    /// </remarks>
    public byte[] SerialNumber { get; init; } = default!;

    /// <summary>
    /// Permitted uses of the certificate.
    /// </summary>
    public CertificatePermittedUse PermittedUse { get; init; }

    /// <summary>
    /// Unique system name assigned to the generated certificate. Used to identify the system in the rest of Enclave.
    /// </summary>
    public string SubjectDistinguishedName { get; init; } = default!;

    /// <summary>
    /// Contains the provided public key.
    /// </summary>
    public byte[] SubjectPublicKey { get; init; } = default!;

    /// <summary>
    /// Unix epoch timestamp indicating the point from which this certificate is considered valid.
    /// </summary>
    public long NotBefore { get; init; }

    /// <summary>
    /// Unix epoch timestamp indicating the point after which this certificate is no longer considered valid.
    /// </summary>
    public long NotAfter { get; init; }

    /// <summary>
    /// The name of the issuing authority.
    /// </summary>
    public string IssuerDistinguishedName { get; init; } = default!;

    /// <summary>
    /// The public key of the issuing authority.
    /// </summary>
    public byte[] IssuerPublicKey { get; init; } = default!;

    /// <summary>
    /// Signature of this certificate.
    /// </summary>
    public byte[] Signature { get; init; } = default!;
}
