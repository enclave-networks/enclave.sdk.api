namespace Enclave.Sdk.Api.Data;

/// <summary>
/// A key the search term of a list call accepts, such as "tags" in "tags:server", with the help the API gives for it.
/// </summary>
public sealed class SearchKey
{
    // The API's model is SearchKey in portal src/Enclave.Configuration.Data/Modules/Search/SearchKey.cs, which
    // Enclave.Sdk.Api.Data does not include. This copy has the same properties with the same names and types,
    // so it reads and writes the API's JSON with the options every client uses: camelCase names and enums by
    // name, as the API writes them (portal src/Enclave.Api.Scaffolding/CommonWebStartup.cs, AddJsonOptions).
    // The API returns SearchKey<TModel>, whose only other property, DatabaseField, is left out of its JSON.

    /// <summary>
    /// The key's name, which goes before the colon in a search term, such as "tags".
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// The modifiers the key can take, or null if it takes none.
    /// </summary>
    public List<SearchModifier>? Modifiers { get; init; }

    /// <summary>
    /// What the key's values name, such as <see cref="SearchKeyDataType.Tags"/> for tag names.
    /// </summary>
    public SearchKeyDataType DataType { get; init; }

    /// <summary>
    /// A short description of the key.
    /// </summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// Help text for the key, such as the Enclave Portal shows in its search box.
    /// </summary>
    public string HintText { get; init; } = string.Empty;

    /// <summary>
    /// Values the key can take, such as "Enabled" and "Disabled", or null if the API lists none.
    /// </summary>
    public List<string>? HintValues { get; init; }

    /// <summary>
    /// Whether the key can take more than one value, such as several tags.
    /// </summary>
    public bool CanHaveMultiple { get; init; }

    /// <summary>
    /// Whether the API searches with this key for text in a search term that follows no key.
    /// </summary>
    public bool IsDefault { get; init; }

    /// <summary>
    /// Whether a value must match exactly, rather than match any part of the field ignoring case.
    /// </summary>
    public bool UseExactMatch { get; init; }
}
