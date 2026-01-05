using Enclave.Api.Modules.SystemManagement.Authority;
using Enclave.Configuration;
using Enclave.Sdk.Api.Clients;
using FluentAssertions;
using NUnit.Framework;
using System.Text.Json;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Enclave.Sdk.Api.Tests.Clients;

public class AuthorityClientTests
{
    private AuthorityClient _authorityClient;
    private WireMockServer _server;
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    [SetUp]
    public void Setup()
    {
        _server = WireMockServer.Start();

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri(_server.Urls[0]),
        };

        _authorityClient = new AuthorityClient(httpClient);
    }

    [Test]
    public async Task Should_return_a_enrol_result_when_sending_a_valid_request()
    {
        // Arrange
        var responseModel = EnrolResult.Success(null);

        _server
          .Given(Request.Create().WithPath("/authority/enrol").UsingPost())
          .RespondWith(
            Response.Create()
              .WithStatusCode(200)
              .WithBody(await responseModel.ToJsonAsync(_serializerOptions)));

        // Act
        var result = await _authorityClient.EnrolAsync(new EnrolRequestModel
        {
            EnrolmentKey = "key",
            Nonce = "nonce",
            PublicKey = "",
        });

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public async Task Should_throw_an_error_when_sending_a_null_request()
    {
        // Arrange

        // Act
        var result = await _authorityClient.EnrolAsync(new EnrolRequestModel
        {
            EnrolmentKey = "key",
            Nonce = "nonce",
            PublicKey = "",
        });

        // Assert
        Assert.ThrowsAsync<ArgumentNullException>(async () => await _authorityClient.EnrolAsync(null));
    }
}
