﻿using System.Net.Http.Json;
using Enclave.Api.Modules.SystemManagement.Authority;
using Enclave.Configuration;
using Enclave.Sdk.Api.Clients.Interfaces;

namespace Enclave.Sdk.Api.Clients;

/// <inheritdoc cref="IAuthorityClient" />
internal class AuthorityClient : ClientBase, IAuthorityClient
{
    /// <summary>
    /// Constructor which will be called by <see cref="EnclaveClient"/> when it's created.
    /// </summary>
    /// <param name="httpClient">an instance of httpClient with a baseURL referencing the API.</param>
    public AuthorityClient(HttpClient httpClient)
        : base(httpClient)
    {
    }

    /// <inheritdoc/>
    public async Task<EnrolResult> EnrolAsync(EnrolRequestModel requestModel)
    {
        if (requestModel is null)
        {
            throw new ArgumentNullException(nameof(requestModel));
        }

        var result = await HttpClient.PostAsJsonAsync($"authority/enrol", requestModel, Constants.JsonSerializerOptions);

        var model = await DeserialiseAsync<EnrolResult>(result.Content);

        EnsureNotNull(model);

        return model;
    }
}
