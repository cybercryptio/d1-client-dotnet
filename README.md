# .NET Client Library for Encryptonize&reg;

.NET client libraries for
* [Encryptonize&reg; Core](https://github.com/cyber-crypt-com/encryptonize-core)
* [Encryptonize&reg; Objects](https://github.com/cyber-crypt-com/encryptonize-objects)

## Usage
In order to use the client you will need credentials for the Encryptonize Core server.
When setting up the server the first time, you need to bootstrap an initial user with credentials
either through the executable as described
[here](https://github.com/cyber-crypt-com/encryptonize-core/blob/master/documentation/user_manual.md#bootstrapping-users).
Subsequent users can be created through the API as described
[here](https://github.com/cyber-crypt-com/encryptonize-core/blob/master/documentation/user_manual.md#creating-users-through-the-api).

A new Encryptonize&reg; Core client can be created by creating an instance of the
`EncryptonizeCoreClient` class as shown below.

```csharp
// Create a new Encryptonize Core client, providing the hostname, a root CA certificate, and user
// credentials.
var client = new EncryptonizeCoreClient("localhost:9000", "user id", "password", "./ca.crt");

// Encrypt sensitive data.
var plaintext = System.Text.Encoding.ASCII.GetBytes("secret data");
var associatedData = System.Text.Encoding.ASCII.GetBytes("metadata");
var encryptResponse = client.Encrypt(plaintext, associatedData);

// Decrypt the response.
var decryptResponse = client.Decrypt(encryptResponse.ObjectId, encryptResponse.Ciphertext, encryptResponse.AssociatedData);
```

The process is similar for Encryptonize&reg; Objects:

```csharp
// Create a new Encryptonize Core client, providing the hostname, a root CA certificate, and user
// credentials.
var client = new EncryptonizeObjectsClient("localhost:9000", "user id", "password", "./ca.crt");

// Store sensitive data in encrypted form.
var plaintext = System.Text.Encoding.ASCII.GetBytes("secret data");
var associatedData = System.Text.Encoding.ASCII.GetBytes("metadata");
var storeResponse = client.Store(plaintext, associatedData);

// Retrieve the stored data.
var retrieveResponse = client.Retrieve(storeResponse.ObjectId);
```

For more information, see the [documentation](./documentation/api/Encryptonize.Client.md).

## Development

This document describes the process for building the library on your local computer.

### Getting started

You will need to install .NET 6 to build the project. To install .NET 6 follow the instructions on the [Microsoft website](https://dotnet.microsoft.com/download/dotnet-core/).

### Building the project

The project can be built using the standard `dotnet build` command.

But it is recommended to use the makefile, `make build`, as it ensures API documentation is updated as needed.

To generate the API documentation, XmlDocMarkdown needs to be installed. To install XmlDocMarkdown run `dotnet tool install xmldocmd -g`.
