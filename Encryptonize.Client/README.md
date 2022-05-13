# .NET client library for Encryptonize-as-a-Service

**TODO**: Generally update links

## Getting started

**TODO**: Vanja any good ideas? :)
For a description of Encryptonize please take a look at [TODO](insert link).
The client is used to communicate with the Encryptonize service, to manage encrypted data.

### Prerequisites

- The [Encryptonize Core](https://github.com/cyber-crypt-com/encryptonize-core) service must be deployed and accessible. **TODO**: Link to information on how to deploy?

### Installation

The client library is available through nuget.org. The latest version can be installed using the following command:

```bash
dotnet add package Encryptonize.Client
```

### How to use

#### Configuring the client

Creating a new client is easy, it only requires you to pass the URL of the Encryptonize service, the username and the password into the constructor:

```csharp
using Encryptonize.Client;

var encryptonizeUrl = "https://localhost:9000";
var encryptonizeUsername = "bd778920-f130-4a5c-b577-79d71bedae67";
var encryptonizePassword = "Iy7ZH89rUj4H8dqagKUSqmkVOFULxghtgJR-rSreeVk";
var client = new EncryptonizeCoreClient(encryptonizeUrl, encryptonizeUsername, encryptonizePassword);
// OR
var client = new EncryptonizeObjectsClient(encryptonizeUrl, encryptonizeUsername, encryptonizePassword);
```

Afterwards the client is ready to be used, and the different methods can be called. See the [API reference](#api-reference) for a description of all the available methods.

## API reference

**TODO**: Insert link to API reference.

## Development

If you want to modify the generated client, then you would need to ensure that you have the correct project structure as well as the tooling needed to build the client.

### Build

To be able to locate the necessary protobuf files, you can either define the environment variable `PROTOBUF_PATH` to point to the directory containing the protobuf files, or checkout the [encryptonize-core repository](https://github.com/cyber-crypt-com/encryptonize-core) on the relative path `../encryptonize-core/` and the [encryptonize-objects repository](https://github.com/cyber-crypt-com/encryptonize-objects) on the relative path `../encryptonize-objects`.
Build the library:

```bash
dotnet build src/Encryptonize.Client.csproj
```

### Run tests

The tests require having the Encryptonize Service running. You can configure the connection to the service through environment variables:

```text
E2E_TEST_URL  = <Encryptonize Service endpoint>
E2E_TEST_CERT = <Client certificate>
E2E_TEST_UID  = <Encryptonize User ID>
E2E_TEST_PASS = <Encryptonize User Password>
```
