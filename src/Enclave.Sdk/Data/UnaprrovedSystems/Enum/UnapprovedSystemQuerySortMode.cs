namespace Enclave.Sdk.Api.Data.UnaprrovedSystems.Enum;

/// <summary>
/// Sort modes for unapproved systems.
/// </summary>
public enum UnapprovedSystemQuerySortMode
{
    /// <summary>
    /// Sort by the when the system was enrolled (most recent first).
    /// </summary>
    RecentlyEnrolled,

    /// <summary>
    /// Sort by the system description.
    /// </summary>
    Description,

    /// <summary>
    /// Sort by the name of enrolment key used.
    /// </summary>
    EnrolmentKeyUsed,
}