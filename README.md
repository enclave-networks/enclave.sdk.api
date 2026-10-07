# Enclave.Sdk.Api
Provides a NuGet package that makes it easier to consume the Enclave Management APIs


# Getting Started
Starting to use the API is simple; first you'll want to create a personal access token which can be done on the [account](https://portal.enclave.io/account) page.

## Creating an Enclave Client
When creating an Enclave client there are 3 ways to start. The first is to use a `credentials.json` file found in the `.enclave` folder in your user directory with the below structure.

```json
{
    "personalAccessToken": "PERSONAL ACCESS TOKEN"
}
```

You then create a new `EnclaveClient` as below
```csharp
var enclaveClient = new EnclaveClient();
```

If the file doesn't exist, this throws a `FileNotFoundException` that names the file and the ways to supply a token. A file that isn't valid JSON, or holds no credentials, throws an `InvalidOperationException` naming the file.

Alternatively you can pass the `EnclaveClient` the personal access token directly, which overrides any value in your credentials file.

```csharp
var token = "YOUR TOKEN";
var enclaveClient = new EnclaveClient(token);
```

Finally you can pass in an `EnclaveClientOptions` object

```csharp
var enclaveClient = new EnclaveClient(new EnclaveClientOptions
{
    PersonalAccessToken = "YOUR TOKEN",
});
```


## Selecting Your Organisation
Once you're authenticated, you create a client for the organisation you want to work in. There are two ways to do this.

### From your list of organisations
Retrieve the organisations your token can see, pick one, and create a client for it.
```csharp
// Retrieve all orgs associated to the authenticated User
var organisations = await enclaveClient.GetOrganisationsAsync();

// Select the org you want here we're just getting the first org
var organisation = organisations.FirstOrDefault();

// Create a client for the specified organisation.
var organisationClient = enclaveClient.CreateOrganisationClient(organisation);
```

### From an organisation ID
If you already know the organisation's ID (an `OrganisationGuid`, for example the `OrgId` of an organisation you saved earlier), create the client from the ID alone. This skips the call that retrieves your organisations.
```csharp
// Parse an organisation ID you have as text, for example from configuration
if (!OrganisationGuid.TryParse("YOUR ORGANISATION ID", out var organisationId))
{
    throw new InvalidOperationException("Not a valid organisation ID.");
}

var organisationClient = enclaveClient.CreateOrganisationClient(organisationId);
```

The two clients make the same API calls. They differ in what they know about the organisation:

| | From your list of organisations | From an organisation ID |
|---|---|---|
| Returns | `IOrganisationClient` | `IOrganisationScopedClient` |
| API calls to create it | 1 (`GetOrganisationsAsync`) | none |
| Every API call below | yes | yes |
| `OrgId` | yes | yes |
| `Organisation` (the organisation's name and your role in it) | yes | no |

`IOrganisationClient` extends `IOrganisationScopedClient`, so code written against `IOrganisationScopedClient` works with either client.

## Making an API Call
Making a call is really easy from the organisation client; here you have access to all the API calls listed on the [Enclave API Docs](https://api.enclave.io/)

To make an organisation call
```csharp
var currentOrganisation = await organisationClient.GetAsync();
```

All other areas are properties on the organisation client, so for example
```csharp
var enrolledSystems = await organisationClient.EnrolledSystems.GetSystemsAsync();

var enrolmentKey = await organisationClient.EnrolmentKeys.GetEnrolmentKeysAsync();
```

IDs you pass as text, such as system IDs and tag names, are always sent as a single ID, whatever characters they contain. An empty ID, `.` or `..` throws an `ArgumentException` before anything is sent.

## Errors
A call that fails throws:

| What went wrong | Exception |
|---|---|
| The API refused the request and said why | `EnclaveApiException`, with the API's explanation in `ProblemDetails` |
| Any other failure, such as a proxy error or a missing route | `HttpRequestException`, with the status in `StatusCode` |

## Update Requests
When updating an item, we make a call to the relevant `Update` method this then returns an instance of `IPatchClient` which has a fluent implementation. So for example;
```csharp
var dnsZoneId = DnsZoneId.FromInt(123);
var result = await _dnsClient.UpdateZone(dnsZoneId).Set(d => d.Name, "New Name").ApplyAsync();
```
This code will update the name of the specified DNS Zone. Once you've called `ApplyAsync` the request will be sent and the returning model will be an updated version of the relevant type in this case the DNS Zone.

Only the fields you `Set` are sent, and the rest keep their values. Set a field to `null` to clear it:
```csharp
await organisationClient.Policies.Update(policyId).Set(p => p.ActiveHours, null).ApplyAsync();
```

## Search Keys
The list calls for systems, unapproved systems, enrolment keys, policies and tags take a search term, which can use search keys such as `tags:server`. `GetSearchKeysAsync` lists the keys each one accepts, with a description and example values for each.
```csharp
var searchKeys = await organisationClient.EnrolledSystems.GetSearchKeysAsync();

foreach (var key in searchKeys)
{
    Console.WriteLine($"{key.Name}: {key.Description}");
}
```

## Gateway Priority
For a gateway policy that uses its gateways in the order listed (the portal's "Ordered"), use `GatewayPriorityType.Prioritised`.

If you write the models to JSON yourself, add `GatewayPriorityTypeJsonConverter` before any `JsonStringEnumConverter`, so that value is written as `Ordered`, the name the API uses:
```csharp
var options = new JsonSerializerOptions
{
    Converters = { new GatewayPriorityTypeJsonConverter(), new JsonStringEnumConverter() },
};
```

## Logging or Capturing Requests
To log the requests the clients send, or to capture them without sending them, for example for a dry run, give `EnclaveClientOptions` an `HttpMessageHandler`. Every client created from those options sends its requests through it.
```csharp
// LoggingHandler is your own DelegatingHandler that logs each request and passes it on
var enclaveClient = new EnclaveClient(new EnclaveClientOptions
{
    PersonalAccessToken = "YOUR TOKEN",
    HttpMessageHandler = new LoggingHandler { InnerHandler = new HttpClientHandler() },
});
```

- A handler that answers a request itself sends nothing. One that passes requests on must be a `DelegatingHandler` with its `InnerHandler` set.
- Each request carries your token in its `Authorization` header, so leave that header out of anything you log.
- You own the handler: dispose it once you have finished with the clients.

## Partner API
A partner manages its customers through the Enclave Partner API. It uses production by default; to use staging, set both URLs:

| | `BaseUrl` | `PartnerApiBaseUrl` |
|---|---|---|
| Production (default) | `https://api.enclave.io` | `https://partner-api.enclave.io` |
| Staging | `https://staging-api.enclave.io` | `https://staging-partner-api.enclave.io` |

A credentials file can set it too, as `partnerApiBaseUrl`.

```csharp
var enclaveClient = new EnclaveClient(new EnclaveClientOptions
{
    PersonalAccessToken = "YOUR TOKEN",
});

// The partner portal shows your partner ID
if (!PartnerId.TryParse("YOUR PARTNER ID", out var partnerId))
{
    throw new InvalidOperationException("Not a valid partner ID.");
}

var partnerClient = enclaveClient.CreatePartnerClient(partnerId);

var customers = await partnerClient.Customers.GetCustomersAsync(searchTerm: "Globex");
```

`Customers` lists, reads, creates, updates and converts customers, manages their admins and admin invites, and turns admin auto-sync on and off. Its models are in the `Enclave.Sdk.Api.Partner.Models` namespace.

A customer is an Enclave organisation, and every customer call takes its organisation ID. To work with a customer's systems, policies and the rest, create an organisation client from the customer's ID, as long as you have access to it (`UserHasAccess`):
```csharp
await foreach (var customer in customers.Items)
{
    var systems = await enclaveClient.CreateOrganisationClient(customer.Id).EnrolledSystems.GetSystemsAsync();
}
```

Your personal access token needs the `ReadCustomers` scope to read customers. Making changes, and listing invites, need `WriteCustomers` and the Owner or Admin role in the partner.