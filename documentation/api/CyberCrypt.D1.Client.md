# CyberCrypt.D1.Client assembly

## CyberCrypt.D1.Client namespace

| public type | description |
| --- | --- |
| abstract class [D1BaseClient](./CyberCrypt.D1.Client/D1BaseClient.md) | Client for connection to a D1 server. |
| class [D1GenericClient](./CyberCrypt.D1.Client/D1GenericClient.md) | Client for connection to a D1 Generic server. |
| class [D1StorageClient](./CyberCrypt.D1.Client/D1StorageClient.md) | Client for connection to a D1 Objects server. |
| interface [ID1Base](./CyberCrypt.D1.Client/ID1Base.md) | Interface for Encryption service client |
| interface [ID1Generic](./CyberCrypt.D1.Client/ID1Generic.md) | Interface for Encryption Core service client |
| interface [ID1Storage](./CyberCrypt.D1.Client/ID1Storage.md) | Interface for Encryption Objects service client |

## CyberCrypt.D1.Client.Response namespace

| public type | description |
| --- | --- |
| class [CreateGroupResponse](./CyberCrypt.D1.Client.Response/CreateGroupResponse.md) | Response from [`CreateGroup`](./CyberCrypt.D1.Client/ID1Base/CreateGroup.md) or [`CreateGroupAsync`](./CyberCrypt.D1.Client/ID1Base/CreateGroupAsync.md). |
| class [CreateUserResponse](./CyberCrypt.D1.Client.Response/CreateUserResponse.md) | Response from [`CreateUser`](./CyberCrypt.D1.Client/ID1Base/CreateUser.md) or [`CreateUserAsync`](./CyberCrypt.D1.Client/ID1Base/CreateUserAsync.md). |
| class [DecryptResponse](./CyberCrypt.D1.Client.Response/DecryptResponse.md) | Response from [`Decrypt`](./CyberCrypt.D1.Client/ID1Generic/Decrypt.md) or [`DecryptAsync`](./CyberCrypt.D1.Client/ID1Generic/DecryptAsync.md). |
| class [EncryptResponse](./CyberCrypt.D1.Client.Response/EncryptResponse.md) | Response from [`Encrypt`](./CyberCrypt.D1.Client/ID1Generic/Encrypt.md) or [`EncryptAsync`](./CyberCrypt.D1.Client/ID1Generic/EncryptAsync.md). |
| class [GetPermissionsResponse](./CyberCrypt.D1.Client.Response/GetPermissionsResponse.md) | Response from [`GetPermissions`](./CyberCrypt.D1.Client/ID1Base/GetPermissions.md) or [`GetPermissionsAsync`](./CyberCrypt.D1.Client/ID1Base/GetPermissionsAsync.md). |
| class [RetrieveResponse](./CyberCrypt.D1.Client.Response/RetrieveResponse.md) | Response from [`Retrieve`](./CyberCrypt.D1.Client/ID1Storage/Retrieve.md) or [`RetrieveAsync`](./CyberCrypt.D1.Client/ID1Storage/RetrieveAsync.md). |
| class [StoreResponse](./CyberCrypt.D1.Client.Response/StoreResponse.md) | Response from [`Store`](./CyberCrypt.D1.Client/ID1Storage/Store.md) or [`StoreAsync`](./CyberCrypt.D1.Client/ID1Storage/StoreAsync.md). |
| class [VersionResponse](./CyberCrypt.D1.Client.Response/VersionResponse.md) | Response from [`Version`](./CyberCrypt.D1.Client/ID1Base/Version.md) or [`VersionAsync`](./CyberCrypt.D1.Client/ID1Base/VersionAsync.md). |

## CyberCrypt.D1.Client.Utils namespace

| public type | description |
| --- | --- |
| enum [Scope](./CyberCrypt.D1.Client.Utils/Scope.md) | The possible permission scopes. |

<!-- DO NOT EDIT: generated by xmldocmd for CyberCrypt.D1.Client.dll -->