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
- **OrganisationClient** - Created via `EnclaveClient.CreateOrganisationClient(AccountOrganisationModel)`, returning `IOrganisationClient`. `CreateOrganisationClient(OrganisationGuid)` returns `IOrganisationScopedClient` (implemented by `OrganisationScopedClient`), which has every call and an `OrgId` but no `Organisation`; `IOrganisationClient` extends it. Provides access to all organization-scoped API operations through sub-clients:
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

A field set to `null` is sent as JSON `null`, which the API applies as "clear this field"; fields not set are left out of the body.

**Data Models**: The SDK depends on `Enclave.Sdk.Api.Data` NuGet package for all API data models (request/response DTOs). Model types come from `Enclave.Api.Modules.*` namespaces. A model `Enclave.Sdk.Api.Data` has wrong is corrected in this package: `GatewayPriorityTypeJsonConverter` (`Data/`) writes `GatewayPriorityType.Prioritised` as the API's `Ordered` and reads both names. A model it lacks is defined in this package, with the portal model's property names and types so it reads and writes the same JSON, and cites the portal source: `SearchKey`, `SearchModifier` and `SearchKeyDataType` (`Data/`, namespace `Enclave.Sdk.Api.Data`) are the API's search key model, which `GetSearchKeysAsync` returns on the systems, unapproved systems, enrolment keys, policies and tags clients.

**Serialization**: `Constants.JsonSerializerOptions` is used for every request and response. `GatewayPriorityTypeJsonConverter` is registered before `JsonStringEnumConverter`, because System.Text.Json uses the first converter that can convert a type.

**IDs in URL paths**: Every caller-supplied string that goes into a URL path goes through `ClientBase.PathSegment`, which escapes it with `Uri.EscapeDataString` and throws `ArgumentException` for null, `""`, `.` and `..`. Typed integer IDs (`PolicyId`, `DnsZoneId` and the like) format as digits and are not escaped.

**Error Handling**: `ProblemDetailsHttpMessageHandler` intercepts HTTP responses and throws `EnclaveApiException` for `application/problem+json` responses. Every client call also checks the status (`EnsureSuccessStatusCode`, or `GetFromJsonAsync`, which does), so any other failure throws `HttpRequestException` with `StatusCode` set.

**HTTP Handler**: `EnclaveClientOptions.HttpMessageHandler` (JSON-ignored), when set, becomes the inner handler beneath `ProblemDetailsHttpMessageHandler` in place of `HttpClientHandler`. The package never disposes it; the caller owns it.

### Partner API Clients

- `EnclaveClient.CreatePartnerClient(PartnerId)` returns `IPartnerClient` (`PartnerClient`), whose `Customers` (`ICustomersClient`, `CustomersClient`) covers the routes of the partner API's `CustomersController` (portal `src/Enclave.Partner.Api/Modules/CustomerManagement/Customers/CustomersController.cs`), all under `partner/{partnerId}/customers`. That controller is the authority for routes, bodies and responses.
- The partner API runs on its own host: `https://partner-api.enclave.io` in production and `https://staging-partner-api.enclave.io` in staging, beside `https://api.enclave.io` and `https://staging-api.enclave.io`. `EnclaveClientOptions.PartnerApiBaseUrl` defaults to production (`Constants.PartnerApiUrl`), as `BaseUrl` does. `EnclaveClient` builds a second `HttpClient` for it in its constructor from the same options (token, User-Agent, `ProblemDetailsHttpMessageHandler` over the caller's `HttpMessageHandler`); when a caller sets the URL to null or blank, `CreatePartnerClient` throws `InvalidOperationException`.
- Only the customer routes are covered, because personal access tokens carry only the `ReadCustomers` and `WriteCustomers` partner scopes (portal `Enclave.Accounts/Config/EnclaveIdentityClientConfig.cs`, `AddPersonalAccessTokenClient`). Left out: `oldest-version`, `countries`, `referlink`, the Gradient endpoints, and the partner's own properties, users and invites.
- Models: `Enclave.Sdk.Api.Data` has none for the partner API, so they are in `Partner/Models/` (namespace `Enclave.Sdk.Api.Partner.Models`), each with a portal model's property names, types and nullability and citing it in its summary. The bodies the client builds or unwraps (`ConvertCustomerModel`, `CreateCustomerAdminInviteModel`, `CustomerUsersModel`, `CustomerPendingAdminInvitesModel`) are internal.
- `OrganisationGuid`, `AccountGuid` and `PartnerId` format as 32 hex digits and go into paths unescaped. `OrganisationInviteId` is string-backed, so it goes through `PathSegment`.
- A default `OrganisationInviteId` throws `NullReferenceException` from `Equals`, `==` and `GetHashCode`; check `ToString() is null` instead. The cancel-invite response writes the ID as null, so `CancelInviteAsync` returns the ID it was given. The auto-sync routes answer an unknown customer with 204 and no body, which `CustomersClient` turns into `InvalidOperationException`.
- Tests: `PartnerClientTests` (option, hosts, handler), `Clients/CustomersClientTests` (every call through `EnclaveClient` against a WireMock partner host, with response JSON written as the API writes it), and `PartnerErrorResponseTests`, which lists every partner call. Add a new partner call to `PartnerErrorResponseTests`, not `ErrorResponseTests`.

### Testing

Tests use NUnit with WireMock.Net for HTTP mocking. Each client has a corresponding test file in `tests/Enclave.Sdk.Api.Tests/Clients/`.

- `ErrorResponseTests` lists every API call on every client and checks each throws `HttpRequestException` for a plain 502 and 404, and `EnclaveApiException` for problem+json. Add a new call to its list.
- `RecordingHttpMessageHandler` records the requests a client sends (URI with its escaping, Authorization header, body). It either passes them on to an inner handler or answers them itself.

## Code Style

- File-scoped namespaces (`csharp_style_namespace_declarations=file_scoped`)
- Private fields prefixed with underscore (`_fieldName`)
- StyleCop analyzers enabled
- Nullable reference types enabled

## Versioning and Releases

Versioning is handled by **GitVersion** (see `GitVersion.yml`). Merging to main triggers a release - no manual tagging required.

- **GitHub Packages**: All builds (alpha/beta/stable) - for internal Enclave consumers
- **nuget.org**: Stable releases only - for external third-party consumers
