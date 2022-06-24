﻿using Enclave.Sdk.Api.Data.PatchModel;

namespace Enclave.Sdk.Api.Data.TrustRequirements;

/// <summary>
/// Defines the modifiable properties of a trust requirement.
/// </summary>
public class TrustRequirementPatch : IPatchModel
{
    /// <summary>
    /// The trust requirement name/description.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Notes for the trust requirement.
    /// </summary>
    public string? Notes { get; init; }

    /// <summary>
    /// The trust requirement settings.
    /// </summary>
    public TrustRequirementSettingsModel? Settings { get; init; }
}