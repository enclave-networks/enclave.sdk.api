using System.Linq.Expressions;
using Enclave.Sdk.Api.Data.PatchModel;

namespace Enclave.Sdk.Api.Data;

/// <summary>
/// Construct and send patch requests.
/// </summary>
/// <typeparam name="TModel">The Type we're updating.</typeparam>
/// <typeparam name="TResponse">The Type we're returning.</typeparam>
public interface IPatchClient<TModel, TResponse>
    where TModel : IPatchModel
{
    /// <summary>
    /// Set a value in the global dictionary to the updated value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value you're updating.</typeparam>
    /// <param name="propExpr">Expression tree witht he property you want to update.</param>
    /// <param name="newValue">The new value.</param>
    /// <returns>Builder for fluent building.</returns>
    /// <exception cref="ArgumentNullException">Throws if either propExpr or newValue are null.</exception>
    /// <exception cref="ArgumentException">If the selected propExpr body is null.</exception>
    IPatchClient<TModel, TResponse> Set<TValue>(Expression<Func<TModel, TValue?>> propExpr, TValue newValue);

    /// <summary>
    /// Send the request that has been setup prior.
    /// </summary>
    /// <returns>An object of type TResponse.</returns>
    Task<TResponse> SendAsync();
}