using System.Globalization;
using TypedIds;

namespace Enclave.Sdk.Api.Data.Tags;

/// <summary>
/// A strongly-typed tag reference ID value.
/// </summary>
[TypedId(IdBackingType.String)]
public readonly partial struct TagRefId
{
    // We put 'tag/' at the start of the ref ID so there is zero chance of a reference being a tag as well.
    public static TagRefId New() => FromString(string.Format(CultureInfo.InvariantCulture, "ref:{0:N}", Guid.NewGuid()));
}
