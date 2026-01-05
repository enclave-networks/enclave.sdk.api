# CLAUDE.md

## Build and Test Commands

```bash
# Build the solution
dotnet build Enclave.Sdk.Api.sln

# Build in Release mode
dotnet build Enclave.Sdk.Api.sln -c Release

# Run all tests
dotnet test tests/Enclave.Sdk.Api.Tests

# Run a specific test class
dotnet test tests/Enclave.Sdk.Api.Tests --filter "FullyQualifiedName~DnsClientTests"

# Run a specific test
dotnet test tests/Enclave.Sdk.Api.Tests --filter "FullyQualifiedName~DnsClientTests.ShouldReturnListOfZones"
```

## Architecture Overview

This is a .NET 10 SDK for consuming the Enclave Management APIs. The SDK is published as a NuGet package.

### Client Hierarchy

- **EnclaveClient** (`src/Enclave.Sdk.Api/EnclaveClient.cs`) - Main entry point. Authenticates via Personal Access Token and creates organization clients.
- **OrganisationClient** - Created via `EnclaveClient.CreateOrganisationClient()`. Provides access to all organization-scoped API operations through sub-clients:
  - `Dns` - DNS zones and records
  - `EnrolmentKeys` - System enrolment keys
  - `EnrolledSystems` - Enrolled systems management
  - `UnapprovedSystems` - Pending system approvals
  - `Policies` - Network policies
  - `Tags` - System tags
  - `Logs` - Activity logs
  - `TrustRequirements` - Trust requirements

### Key Patterns

**Client Base Class**: All clients extend `ClientBase` which provides shared HTTP functionality and JSON serialization helpers.

**Fluent Patch Updates**: Updates use a fluent `IPatchClient<TModel, TResponse>` pattern:
```csharp
await client.Update().Set(x => x.PropertyName, newValue).ApplyAsync();
```

**Data Models**: The SDK depends on `Enclave.Sdk.Api.Data` NuGet package for all API data models (request/response DTOs). Model types come from `Enclave.Api.Modules.*` namespaces.

**Error Handling**: `ProblemDetailsHttpMessageHandler` intercepts HTTP responses and throws `EnclaveApiException` for API errors.

### Testing

Tests use NUnit with WireMock.Net for HTTP mocking. Each client has a corresponding test file in `tests/Enclave.Sdk.Api.Tests/Clients/`.

## Code Style

- File-scoped namespaces (`csharp_style_namespace_declarations=file_scoped`)
- Private fields prefixed with underscore (`_fieldName`)
- StyleCop analyzers enabled
- Nullable reference types enabled

## Versioning and Releases

Versioning is handled by **GitVersion** (see `GitVersion.yml`). Merging to main triggers a release - no manual tagging required.

- **GitHub Packages**: All builds (alpha/beta/stable) - for internal Enclave consumers
- **nuget.org**: Stable releases only - for external third-party consumers
