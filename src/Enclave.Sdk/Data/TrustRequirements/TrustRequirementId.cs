using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypedIds;

namespace Enclave.Sdk.Api.Data.TrustRequirements;

/// <summary>
/// Defines a typed ID for a trust requirement.
/// </summary>
[TypedId(IdBackingType.Int)]
public readonly partial struct TrustRequirementId
{
}