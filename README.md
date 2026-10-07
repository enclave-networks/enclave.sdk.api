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

## Update Requests
When updating an item, we make a call to the relevant `Update` method this then returns an instance of `IPatchClient` which has a fluent implementation. So for example;
```csharp
var dnsZoneId = DnsZoneId.FromInt(123);
var result = await _dnsClient.UpdateZone(dnsZoneId).Set(d => d.Name, "New Name").ApplyAsync();
```
This code will update the name of the specified DNS Zone. Once you've called `ApplyAsync` the request will be sent and the returning model will be an updated version of the relevant type in this case the DNS Zone.