namespace Enclave.Sdk.Api.Data.EnrolledSystems.Enum;

/// <summary>
/// System Query Sort order used when making a System request.
/// </summary>
public enum SystemQuerySortMode
{
    /// <summary>
    /// Sort by recently enrolled.
    /// </summary>
    RecentlyEnrolled,

    /// <summary>
    /// Sort by recently connected.
    /// </summary>
    RecentlyConnected,

    /// <summary>
    /// Sort by description.
    /// </summary>
    Description,

    /// <summary>
    /// Sort by enrolment key.
    /// </summary>
    EnrolmentKeyUsed,
}