namespace Enclave.Sdk.Api.Data.TrustRequirements.Enum;

/// <summary>
/// Defines a type of trust requirement, that generally indicates how that requirement
/// is evaluated.
/// </summary>
public enum TrustRequirementType
{
    /// <summary>
    /// User authentication, usually via a token.
    /// </summary>
    UserAuthentication,

    /// <summary>
    /// The public IP address of the peer.
    /// </summary>
    PublicIp,
}