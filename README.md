# Enclave.Sdk.Api
Provides a NuGet package that makes it easier to consume the Enclave Management APIs


# Getting Started
Starting to use the API is simple first you'll want to create a personal access token which can be done on the [account](https://portal.enclave.io/account) page.

## Creating an Enclave Client
When creating an enclave clien there are 3 ways to start. The first is to use a `credentials.json` file found in the `.enclave` folder in your user directory with the below structure.

```json
{
    "personalAccessToken": "PERSONAL ACCESS TOKEN"
}
```

You then create a new the `EnclaveClient` as below
```csharp
var enclaveClient = new EnclaveClient();
```

Alternatively you can pass the `EnclaveClient` the personal access token this will also then use the default api endpoint which is `https://api.enclave.io`

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


## Selecting Your Org
Once you're authenticated you can then retrieve a list of orgs by calling the below block of code
```csharp
// Retrieve all orgs associated to the authenticated User
var organisations = await enclaveClient.GetOrganisationsAsync();

// Select the org you want here we're just getting the first org
var organisation = organisations.FirstOrDefault();

// Create and organisation client with the specified Org
var organisationClient = enclaveClient.CreateOrganisationClient(organisation);
```

## Making an API Call
Making a call is really easy from the `OrganisationClient` here you have access to all the api calls listed on the [Enclave Api Docs](https://api.enclave.io/)

To make an organisation call
```csharp
var currentOrganisaiton = await organisationClient.GetAsync();
```

All other areas are properties on `OrganisationClient` so for example
```csharp
var enrolledSystems = await organisationClient.EnrolledSystems.GetSystemsAsync();

var enrolmentKey = await organisationClient.EnrolmentKeys.GetEnrolmentKeysAsync();
```

## Update Requests