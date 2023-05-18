using System.Linq.Expressions;
using Enclave.Api.Scaffolding.Models;
using Enclave.Sdk.Api.Clients;

namespace Enclave.Sdk.Api.Data;

/// <summary>
/// Class used to construct and send patch requests.
/// </summary>
/// <typeparam name="TModel">The Type we're updating.</typeparam>
/// <typeparam name="TResponse">The Type we're returning.</typeparam>
internal class PatchClient<TModel, TResponse> : ClientBase, IPatchClient<TModel, TResponse>
    where TModel : PatchModel
{
    private readonly string _patchUrl;

    private Dictionary<string, object> _patchDictionary = new Dictionary<string, object>();

    /// <summary>
    /// This client handles the patch requests for all models.
    /// </summary>
    /// <param name="httpClient">The shared httpClient instance.</param>
    /// <param name="patchUrl">The Url for the patch request.</param>
    public PatchClient(HttpClient httpClient, string patchUrl)
        : base(httpClient)
    {
        _patchUrl = patchUrl;
    }

    public IPatchClient<TModel, TResponse> Set<TValue>(Expression<Func<TModel, TValue?>> propExpr, TValue newValue)
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
    /// Send the request that has been setup prior.
    /// </summary>
    /// <returns>An object of type TResponse.</returns>
    public async Task<TResponse> ApplyAsync()
    {
        try
        {
            using var encoded = CreateJsonContent(_patchDictionary);
            var result = await HttpClient.PatchAsync(_patchUrl, encoded);

            result.EnsureSuccessStatusCode();

            var model = await DeserialiseAsync<TResponse>(result.Content);

            EnsureNotNull(model);

            return model;
        }
        finally
        {
            _patchDictionary = new Dictionary<string, object>();
        }
    }
}