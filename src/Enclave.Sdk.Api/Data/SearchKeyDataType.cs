namespace Enclave.Sdk.Api.Data;

/// <summary>
/// What a <see cref="SearchKey"/>'s values name, for a caller that offers the names to choose from.
/// </summary>
public enum SearchKeyDataType
{
    // The API's enum, with the same members in the same order (portal
    // src/Enclave.Configuration.Data/Modules/Search/SearchKeyDataType.cs), which Enclave.Sdk.Api.Data does
    // not include. The API writes it by name.

    /// <summary>
    /// The values are not the names of other items.
    /// </summary>
    None,

    /// <summary>
    /// The values are tag names.
    /// </summary>
    Tags,

    /// <summary>
    /// The values are system names.
    /// </summary>
    System,

    /// <summary>
    /// The values are enrolment key names.
    /// </summary>
    EnrolmentKey,
}
