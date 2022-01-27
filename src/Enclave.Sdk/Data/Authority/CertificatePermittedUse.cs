namespace Enclave.Sdk.Api.Data.Authority;

[Flags]
public enum CertificatePermittedUse : byte
{
    None = 0x0,
    Endpoint = 0x2,
    Infrastructure = 0x4,
    Intermediate = 0x8,
    Root = 0x10
}
