# CyberCrypt.D1.Client assembly

## CyberCrypt.D1.Client namespace

| public type | description |
| --- | --- |
| abstract class [D1BaseClient](./CyberCrypt.D1.Client/D1BaseClient.md) | Client for connection to a D1 server. |
| class [D1ClientOptions](./CyberCrypt.D1.Client/D1ClientOptions.md) | Options for the [`D1BaseClient`](./CyberCrypt.D1.Client/D1BaseClient.md). |
| class [D1GenericClient](./CyberCrypt.D1.Client/D1GenericClient.md) | Client for connection to a D1 Generic server. |
| class [D1StorageClient](./CyberCrypt.D1.Client/D1StorageClient.md) | Client for connection to a D1 Storage server. |
| interface [ID1Base](./CyberCrypt.D1.Client/ID1Base.md) | Interface for Encryption service client |
| interface [ID1Generic](./CyberCrypt.D1.Client/ID1Generic.md) | Interface for D1 Generic service client |
| interface [ID1Storage](./CyberCrypt.D1.Client/ID1Storage.md) | Interface for D1 Storage service client |

## CyberCrypt.D1.Client.Credentials namespace

| public type | description |
| --- | --- |
| interface [ID1Credentials](./CyberCrypt.D1.Client.Credentials/ID1Credentials.md) | Interface for credential implementations. |
| class [TokenCredentials](./CyberCrypt.D1.Client.Credentials/TokenCredentials.md) | Credentials based off a user provide token. |
| class [UsernamePasswordCredentials](./CyberCrypt.D1.Client.Credentials/UsernamePasswordCredentials.md) | Credentials from username and password. |

## CyberCrypt.D1.Client.Response namespace

| public type | description |
| --- | --- |
| class [CreateGroupResponse](./CyberCrypt.D1.Client.Response/CreateGroupResponse.md) | Response from [`CreateGroup`](./CyberCrypt.D1.Client.ServiceClients/ID1Authn/CreateGroup.md) or [`CreateGroupAsync`](./CyberCrypt.D1.Client.ServiceClients/ID1Authn/CreateGroupAsync.md). |
| class [CreateUserResponse](./CyberCrypt.D1.Client.Response/CreateUserResponse.md) | Response from [`CreateUser`](./CyberCrypt.D1.Client.ServiceClients/ID1Authn/CreateUser.md) or [`CreateUserAsync`](./CyberCrypt.D1.Client.ServiceClients/ID1Authn/CreateUserAsync.md). |
| class [DecryptResponse](./CyberCrypt.D1.Client.Response/DecryptResponse.md) | Response from [`Decrypt`](./CyberCrypt.D1.Client.ServiceClients/ID1Encrypt/Decrypt.md) or [`DecryptAsync`](./CyberCrypt.D1.Client.ServiceClients/ID1Encrypt/DecryptAsync.md). |
| class [EncryptResponse](./CyberCrypt.D1.Client.Response/EncryptResponse.md) | Response from [`Encrypt`](./CyberCrypt.D1.Client.ServiceClients/ID1Encrypt/Encrypt.md) or [`EncryptAsync`](./CyberCrypt.D1.Client.ServiceClients/ID1Encrypt/EncryptAsync.md). |
| class [GetPermissionsResponse](./CyberCrypt.D1.Client.Response/GetPermissionsResponse.md) | Response from [`GetPermissions`](./CyberCrypt.D1.Client.ServiceClients/ID1Authz/GetPermissions.md) or [`GetPermissionsAsync`](./CyberCrypt.D1.Client.ServiceClients/ID1Authz/GetPermissionsAsync.md). |
| class [RetrieveResponse](./CyberCrypt.D1.Client.Response/RetrieveResponse.md) | Response from [`Retrieve`](./CyberCrypt.D1.Client.ServiceClients/ID1Store/Retrieve.md) or [`RetrieveAsync`](./CyberCrypt.D1.Client.ServiceClients/ID1Store/RetrieveAsync.md). |
| class [SearchResponse](./CyberCrypt.D1.Client.Response/SearchResponse.md) | Response from [`Search`](./CyberCrypt.D1.Client.ServiceClients/ID1Searchable/Search.md) or [`SearchAsync`](./CyberCrypt.D1.Client.ServiceClients/ID1Searchable/SearchAsync.md). |
| class [StoreResponse](./CyberCrypt.D1.Client.Response/StoreResponse.md) | Response from [`Store`](./CyberCrypt.D1.Client.ServiceClients/ID1Store/Store.md) or [`StoreAsync`](./CyberCrypt.D1.Client.ServiceClients/ID1Store/StoreAsync.md). |
| class [VersionResponse](./CyberCrypt.D1.Client.Response/VersionResponse.md) | Response from [`Version`](./CyberCrypt.D1.Client.ServiceClients/ID1Version/Version.md) or [`VersionAsync`](./CyberCrypt.D1.Client.ServiceClients/ID1Version/VersionAsync.md). |

## CyberCrypt.D1.Client.ServiceClients namespace

| public type | description |
| --- | --- |
| class [D1AuthnClient](./CyberCrypt.D1.Client.ServiceClients/D1AuthnClient.md) | Authn client for connection to a D1 server. |
| class [D1AuthzClient](./CyberCrypt.D1.Client.ServiceClients/D1AuthzClient.md) | Authz client for connection to a D1 server. |
| class [D1EncryptClient](./CyberCrypt.D1.Client.ServiceClients/D1EncryptClient.md) | Encrypt client for connection to a D1 server. |
| class [D1SearchableClient](./CyberCrypt.D1.Client.ServiceClients/D1SearchableClient.md) | Searchable client for connection to a D1 server. |
| class [D1StoreClient](./CyberCrypt.D1.Client.ServiceClients/D1StoreClient.md) | Store client for connection to a D1 server. |
| class [D1VersionClient](./CyberCrypt.D1.Client.ServiceClients/D1VersionClient.md) | Version client for connection to a D1 server. |
| interface [ID1Authn](./CyberCrypt.D1.Client.ServiceClients/ID1Authn.md) | Interface for Authn client |
| interface [ID1Authz](./CyberCrypt.D1.Client.ServiceClients/ID1Authz.md) | Interface for Authz client |
| interface [ID1Encrypt](./CyberCrypt.D1.Client.ServiceClients/ID1Encrypt.md) | Interface for Encrypt client |
| interface [ID1Searchable](./CyberCrypt.D1.Client.ServiceClients/ID1Searchable.md) | Interface for Searchable client |
| interface [ID1Store](./CyberCrypt.D1.Client.ServiceClients/ID1Store.md) | Interface for Store client |
| interface [ID1Version](./CyberCrypt.D1.Client.ServiceClients/ID1Version.md) | Interface for Version client |

## CyberCrypt.D1.Client.Utils namespace

| public type | description |
| --- | --- |
| enum [Scope](./CyberCrypt.D1.Client.Utils/Scope.md) | The possible permission scopes. |

<!-- DO NOT EDIT: generated by xmldocmd for CyberCrypt.D1.Client.dll -->
