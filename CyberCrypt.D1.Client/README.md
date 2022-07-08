# .NET client library for D1-as-a-Service

**TODO**: Generally update links

## Getting started

**TODO**: Vanja any good ideas? :)
For a description of CyberCrypt D1 please take a look at [TODO](insert link).
The client is used to communicate with the D1 service, to manage encrypted data.

### Prerequisites

- The [D1 Generic](https://github.com/cybercryptio/d1-service-generic) service must be deployed and accessible. **TODO**: Link to information on how to deploy?

### Installation

The client library is available through nuget.org. The latest version can be installed using the following command:

```bash
dotnet add package CyberCrypt.D1.Client
```

### How to use

#### Configuring the client

Creating a new client is easy, it only requires you to pass the URL of the D1 service, the username and the password into the constructor:

```csharp
using CyberCrypt.D1.Client;

var d1Url = "https://localhost:9000";
var d1Username = "bd778920-f130-4a5c-b577-79d71bedae67";
var d1Password = "Iy7ZH89rUj4H8dqagKUSqmkVOFULxghtgJR-rSreeVk";
var client = new D1GenericClient(d1Url, new UsernamePasswordCredentials(d1Username, d1Password));
// OR
var client = new D1StorageClient(d1Url, new UsernamePasswordCredentials(d1Username, d1Password));
```

Afterwards the client is ready to be used, and the different methods can be called. See the [API reference](#api-reference) for a description of all the available methods.

## API reference

**TODO**: Insert link to API reference.

## Development

If you want to modify the generated client, then you would need to ensure that you have the correct project structure as well as the tooling needed to build the client.

### Build

To be able to locate the necessary protobuf files, you can either define the environment variable `PROTOBUF_PATH` to point to the directory containing the protobuf files, or checkout the [d1-service-generic repository](https://github.com/cybercryptio/d1-service-generic) on the relative path `../d1-service-generic/` and the [d1-service-storage repository](https://github.com/cybercryptio/d1-service-storage) on the relative path `../d1-service-storage`.
Build the library:

```bash
dotnet build src/CyberCrypt.D1.Client.csproj
```

### Run tests

The tests require having the D1 Service running. You can configure the connection to the service through environment variables:

```text
E2E_TEST_URL  = <D1 Service endpoint>
E2E_TEST_CERT = <Client certificate>
E2E_TEST_UID  = <D1 User ID>
E2E_TEST_PASS = <D1 User Password>
```
