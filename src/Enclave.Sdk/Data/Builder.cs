using System.Linq.Expressions;

namespace Enclave.Sdk.Api.Data;

/// <summary>
/// Class used to construct patch models.
/// </summary>
/// <typeparam name="TModel">The Type we're updating.</typeparam>
public class Builder<TModel>
{
    private Dictionary<string, object> _patchDictionary = new Dictionary<string, object>();

    /// <summary>
    /// Set a value in the global dictionary to the updated value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value you're updating.</typeparam>
    /// <param name="propExpr">Expression tree witht he property you want to update.</param>
    /// <param name="newValue">the new value.</param>
    /// <returns>Builder for fluent building.</returns>
    /// <exception cref="ArgumentNullException">throws if either propExpr or newValue are null.</exception>
    /// <exception cref="ArgumentException">if the selected propExpr body is null.</exception>
    public Builder<TModel> Set<TValue>(Expression<Func<TModel, TValue?>> propExpr, TValue newValue)
    {
        if (newValue is null)
        {
            throw new ArgumentNullException(nameof(newValue), "please specificy a valid new value.");
        }

        if (propExpr is null)
        {
            throw new ArgumentNullException(nameof(propExpr), "please specificy a valid property expression.");
        }

        // Get the property expression.
        var property = propExpr.Body as MemberExpression;

        if (property is null)
        {
            throw new ArgumentException("Specified property expression is not valid.");
        }

        var propName = property.Member.Name;

        _patchDictionary.Add(propName, newValue);

        return this;
    }

    /// <summary>
    /// Build the Dictionary and return it. then reset the dictionary so the builder can be reused.
    /// </summary>
    /// <returns>Dictionary built using the Set model in this class.</returns>
    public Dictionary<string, object> Send()
    {
        try
        {
            return _patchDictionary;
        }
        finally
        {
            _patchDictionary = new Dictionary<string, object>();
        }
    }
}