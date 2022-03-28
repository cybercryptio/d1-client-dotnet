# .NET client library for Encryptonize-as-a-Service v3.1.0
*Currently the library does not support any of the premium features.*

## Build
Required directory structure:
```
.
├── encryptonize-core
└── encryptonize-premium/
    └── clients/
        └── encryption-service/
            └── dotnet/
                └── Encryptonize.Client/
                    ├── src
                    └── tests/
                        └── Encryptonize.Client.Tests
```
Build the library:
```
dotnet build src/Encryptonize.Client.csproj
```

## Run tests
The tests require having the Encryptonize Core service running locally.

You can configure the connection to the Encryptonize Service through environment variables:
```
E2E_TEST_URL  = <Encryptonize Service endpoint>
E2E_TEST_CERT = <Client certificate>
E2E_TEST_UID  = <Encryptonize User ID>
E2E_TEST_PASS = <Encryptonize User Password>
```

To run the tests:
```
dotnet test tests/Encryptonize.Client.Tests/Encryptonize.Client.Tests.csproj
```

To build and run a Dockerized instance of the Encryptonize Core service and run the tests against it you can use the make target from [`encryptonize-premium`](https://github.com/cyber-crypt-com/encryptonize-premium/blob/master/encryption-service/makefile):
```
make dotnet-tests
```

## Usage
The library is intended for internal use only. It can be imported by adding a project reference to `Encryptonize.Client.csproj`.
