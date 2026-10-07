using System.Text.Json;
using FluentAssertions;
using NUnit.Framework;

namespace Enclave.Sdk.Api.Tests;

// The parameterless EnclaveClient constructor reads ~/.enclave/credentials.json. These tests give the
// reader a path in a fresh temporary directory, so the real user profile is never read, and check the
// errors a person sees when the file is missing or unreadable name the file and say how to fix it.
public class EnclaveClientCredentialsFileTests
{
    private string _directory;

    private string CredentialsPath => Path.Combine(_directory, ".enclave", "credentials.json");

    [SetUp]
    public void Setup()
    {
        _directory = Path.Combine(Path.GetTempPath(), "enclave-sdk-api-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_directory);
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(_directory))
        {
            Directory.Delete(_directory, recursive: true);
        }
    }

    [Test]
    public void Should_read_the_token_from_the_credentials_file()
    {
        // Arrange
        WriteCredentials("{\"personalAccessToken\": \"TOKEN\"}");

        // Act
        var options = EnclaveClient.ReadCredentialsFile(CredentialsPath);

        // Assert
        options.PersonalAccessToken.Should().Be("TOKEN");
    }

    // A machine where nothing has been saved has no .enclave directory at all, which .NET reports as
    // DirectoryNotFoundException, and one with the directory but no file reports FileNotFoundException
    // (File.ReadAllText, https://learn.microsoft.com/dotnet/api/system.io.file.readalltext). Both mean
    // there are no credentials, so both give the same error naming the file and the ways to supply a token.
    [TestCase(false)]
    [TestCase(true)]
    public void Should_throw_a_file_not_found_exception_naming_the_file_and_the_fixes_when_there_is_no_credentials_file(bool directoryExists)
    {
        // Arrange
        if (directoryExists)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(CredentialsPath));
        }

        // Act
        var act = () => EnclaveClient.ReadCredentialsFile(CredentialsPath);

        // Assert
        var exception = act.Should().Throw<FileNotFoundException>().Which;
        exception.FileName.Should().Be(CredentialsPath);
        exception.Message.Should().Contain(CredentialsPath)
            .And.Contain("personalAccessToken")
            .And.Contain("EnclaveClient(");
    }

    [Test]
    public void Should_throw_an_invalid_operation_exception_naming_the_file_when_it_is_not_valid_json()
    {
        // Arrange
        WriteCredentials("{ personalAccessToken: ");

        // Act
        var act = () => EnclaveClient.ReadCredentialsFile(CredentialsPath);

        // Assert
        var exception = act.Should().Throw<InvalidOperationException>().Which;
        exception.Message.Should().Contain(CredentialsPath);
        exception.InnerException.Should().BeAssignableTo<JsonException>();
    }

    // "null" is valid JSON that deserialises to no options at all, which the constructor would
    // otherwise pass on as a null argument.
    [Test]
    public void Should_throw_an_invalid_operation_exception_naming_the_file_when_it_holds_no_credentials()
    {
        // Arrange
        WriteCredentials("null");

        // Act
        var act = () => EnclaveClient.ReadCredentialsFile(CredentialsPath);

        // Assert
        act.Should().Throw<InvalidOperationException>().WithMessage($"*{CredentialsPath}*");
    }

    // A handler is an object in the running process, not a setting, so it is neither read from nor
    // written to a credentials file. A file with an httpMessageHandler entry still gives its token, and
    // options written out for a credentials file hold no handler.
    [Test]
    public void Should_leave_the_http_message_handler_out_of_the_credentials_file()
    {
        // Arrange
        WriteCredentials("{\"personalAccessToken\": \"TOKEN\", \"httpMessageHandler\": {}}");
        var options = new EnclaveClientOptions
        {
            PersonalAccessToken = "TOKEN",
            HttpMessageHandler = new HttpClientHandler(),
        };

        // Act
        var read = EnclaveClient.ReadCredentialsFile(CredentialsPath);
        var written = JsonDocument.Parse(JsonSerializer.Serialize(options));

        // Assert
        read.PersonalAccessToken.Should().Be("TOKEN");
        read.HttpMessageHandler.Should().BeNull();
        written.RootElement.EnumerateObject().Select(p => p.Name).Should().BeEquivalentTo("PersonalAccessToken", "BaseUrl", "PartnerApiBaseUrl");
    }

    private void WriteCredentials(string json)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(CredentialsPath));
        File.WriteAllText(CredentialsPath, json);
    }
}
