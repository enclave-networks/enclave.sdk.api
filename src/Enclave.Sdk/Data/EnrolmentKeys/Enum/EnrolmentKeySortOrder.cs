namespace Enclave.Sdk.Api.Data.EnrolmentKeys.Enum;

/// <summary>
/// Enrolment Key Sort Order used when making an Enrolment Key request.
/// </summary>
public enum EnrolmentKeySortOrder
{
    /// <summary>
    /// Sort by Description.
    /// </summary>
    Description,

    /// <summary>
    /// Sort by Last Used.
    /// </summary>
    LastUsed,

    /// <summary>
    /// Sort By Approval Mode.
    /// </summary>
    ApprovalMode,

    /// <summary>
    /// Sort by Uses Remaining.
    /// </summary>
    UsesRemaining,
}