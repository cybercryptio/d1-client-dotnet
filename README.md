# .NET Client Library for CYBERCRYPT D1

## Prerequisites

- The [D1 Generic](https://github.com/cybercryptio/d1-service-generic) or the [D1 Storage](https://github.com/cybercryptio/d1-service-storage) service must be deployed and accessible.

## Installation

The client library is available through nuget.org. The latest version can be installed using the following command:

```bash
dotnet add package CyberCrypt.D1.Client
```

## Configuring the client

### Username and password

Username and password is only available then using the Standalone ID provider in the D1 service, you can refer to the [D1 Generic Getting Started](https://github.com/cybercryptio/d1-service-generic/blob/master/documentation/getting_started.md) or the [D1 Storage Getting Started](https://github.com/cybercryptio/d1-service-storage/blob/master/documentation/getting_started.md) guides for details on obtain these.

When using username and password the access token is automatically refreshed when it expires.

```csharp
using CyberCrypt.D1.Client;
using CyberCrypt.D1.Client.Credentials;

var d1Url = "https://localhost:9000";
var d1Username = "bd778920-f130-4a5c-b577-79d71bedae67";
var d1Password = "Iy7ZH89rUj4H8dqagKUSqmkVOFULxghtgJR-rSreeVk";
var credentials = new UsernamePasswordCredentials(d1Url, d1Username, d1Password);
var client = new D1GenericClient(d1Url, credentials);
// OR
var client = new D1StorageClient(d1Url, credentials);
```

### OIDC

When using an OIDC provider you will need to obtain an ID Token the usual way, and then provided it to the client.

```csharp
using CyberCrypt.D1.Client;
using CyberCrypt.D1.Client.Credentials;

var d1Url = "https://localhost:9000";
var idToken = "eyJ ... zcifQ.ewo ... NzAKfQ.ggW8h ... Mzqg";
var credentials = new TokenCredentials(idToken);
var client = new D1GenericClient(d1Url, credentials);
// OR
var client = new D1StorageClient(d1Url, credentials);
```

## API reference

[API reference](documentation/api/CyberCrypt.D1.Client.md)
