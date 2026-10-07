namespace Enclave.Sdk.Api.Data;

/// <summary>
/// A modifier a <see cref="SearchKey"/> can take, which changes how the API compares the key's value.
/// </summary>
public enum SearchModifier
{
    // The API's enum, with the same members in the same order (portal
    // src/Enclave.Configuration.Data/Modules/Search/SearchModifier.cs), which Enclave.Sdk.Api.Data does not
    // include. The API writes it by name. The characters are the ones the API reads in a search term (portal
    // src/Enclave.Configuration.Data/Modules/Search/BaseSearchKeyService.cs, CharToSearchModifier).

    /// <summary>
    /// Less than the value, written "&lt;" in a search term.
    /// </summary>
    LessThan,

    /// <summary>
    /// Greater than the value, written "&gt;" in a search term.
    /// </summary>
    GreaterThan,

    /// <summary>
    /// Not equal to the value, written "!" in a search term.
    /// </summary>
    Not,

    /// <summary>
    /// Any one of several values, written "|" in a search term.
    /// </summary>
    Or,
}
