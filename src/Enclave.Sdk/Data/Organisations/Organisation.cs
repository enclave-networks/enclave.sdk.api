using Enclave.Sdk.Api.Data.Account;
using Enclave.Sdk.Api.Data.Organisations.Enum;
using System.Text.Json.Serialization;

namespace Enclave.Sdk.Api.Data.Organisations;

/// <summary>
/// Organisation properties model.
/// </summary>
public class Organisation
{
    /// <summary>
    /// The organisation ID.
    /// </summary>
    public OrganisationId Id { get; init; }

    /// <summary>
    /// The UTC timestamp at which the organisation was created.
    /// </summary>
    public DateTime Created { get; init; }

    /// <summary>
    /// The name of the organisation.
    /// </summary>
    public string Name { get; init; } = default!;

    /// <summary>
    /// The current plan the organisation is on.
    /// </summary>
    public OrganisationPlan Plan { get; init; }

    /// <summary>
    /// The provided website info for the org (if known).
    /// </summary>
    public string? Website { get; init; }

    /// <summary>
    /// The provided phone info for the org (if known).
    /// </summary>
    public string? Phone { get; init; }

    /// <summary>
    /// The maximum number of systems this organisation can have enrolled.
    /// </summary>
    public int MaxSystems { get; init; }

    /// <summary>
    /// The number of enrolled systems.
    /// </summary>
    public long EnrolledSystems { get; init; }

    /// <summary>
    /// The number of systems waiting to be approved.
    /// </summary>
    public long UnapprovedSystems { get; init; }
}