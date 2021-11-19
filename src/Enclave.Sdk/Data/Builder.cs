using System.Linq.Expressions;

namespace Enclave.Sdk.Api.Data;

public class Builder<TModel>
    where TModel : IDataModel
{
    private Dictionary<string, object> _patchDictionary = new Dictionary<string, object>();

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