// Copyright 2020-2022 CYBERCRYPT
using System.Security.Cryptography.X509Certificates;
using System.Runtime.CompilerServices;
using Grpc.Net.Client;
using Grpc.Core;
using Google.Protobuf;
using Encryptonize.Client;
using Encryptonize.Client.Utils;
using Encryptonize.Client.Response;

[assembly: InternalsVisibleTo("Encryptonize.Client.Tests")]

namespace Encryptonize.Client;

/// <summary>
/// Interface for Encryption service client
/// </summary>
public interface IEncryptonizeClient
{
    /// <summary>
    /// Gets or sets the username used to authenticate with the Encryptonize server.
    /// </summary>
    string User { get; }

    /// <summary>
    /// Gets the expiration time of the access token.
    /// </summary>
    /// <remarks>
    /// When no access token is cached, the value is <c>DateTime.MinValue.AddMinutes(1)</c>
    /// </remarks>
    DateTime ExpiryTime { get; }

    /// <summary>
    /// Give a group permission to access an object.
    /// </summary>
    /// <param name="objectId">The ID of the object.</param>
    /// <param name="groupId">The ID of the group.</param>
    void AddPermission(string objectId, string groupId);

    /// <inheritdoc cref="AddPermission"/>
    Task AddPermissionAsync(string objectId, string groupId);

    /// <summary>
    /// Add a user to a group.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="groupId">The ID of group.</param>
    void AddUserToGroup(string userId, string groupId);

    /// <inheritdoc cref="AddUserToGroup"/>
    Task AddUserToGroupAsync(string userId, string groupId);

    /// <summary>
    /// Create a new group.
    /// </summary>
    /// <param name="scopes">List of scopes assigned to the new group.</param>
    /// <returns>An instance of <see cref="CreateGroupResponse"/>.</returns>
    CreateGroupResponse CreateGroup(IList<Scope> scopes);

    /// <inheritdoc cref="CreateGroup"/>
    Task<CreateGroupResponse> CreateGroupAsync(IList<Scope> scopes);

    /// <summary>
    /// Create a new user.
    /// </summary>
    /// <param name="scopes">List of scopes assigned to the new user.</param>
    /// <returns>An instance of <see cref="CreateUserResponse"/>.</returns>
    CreateUserResponse CreateUser(IList<Scope> scopes);

    /// <inheritdoc cref="CreateUser"/>
    Task<CreateUserResponse> CreateUserAsync(IList<Scope> scopes);

    /// <summary>
    /// Decrypt an encrypted object.
    /// </summary>
    /// <param name="objectId">The ID of the object.</param>
    /// <param name="ciphertext">The ciphertext.</param>
    /// <param name="associatedData">The associated data attached to the data.</param>
    /// <returns>An instance of <see cref="DecryptResponse" />.</returns>
    DecryptResponse Decrypt(string objectId, byte[] ciphertext, byte[] associatedData);

    /// <inheritdoc cref="Decrypt"/>
    Task<DecryptResponse> DecryptAsync(string objectId, byte[] ciphertext, byte[] associatedData);

    /// <summary>
    /// Delete data encrypted in the storage attached to Encryptonize.
    /// </summary>
    /// <param name="objectId">The ID of the object.</param>
    void Delete(string objectId);

    /// <inheritdoc cref="Delete"/>
    Task DeleteAsync(string objectId);

    /// <summary>
    /// Encrypt an object with associated data.
    /// </summary>
    /// <param name="plaintext">The plaintext to encrypt.</param>
    /// <param name="associatedData">The attached associated data.</param>
    /// <returns>An instance of <see cref="EncryptResponse" />.</returns>
    EncryptResponse Encrypt(byte[] plaintext, byte[] associatedData);

    /// <inheritdoc cref="Encrypt"/>
    Task<EncryptResponse> EncryptAsync(byte[] plaintext, byte[] associatedData);

    /// <summary>
    /// Get the permissions applied to an object.
    /// </summary>
    /// <param name="objectId">The ID of the object.</param>
    /// <returns>An instance of <see cref="GetPermissionsResponse" />.</returns>
    GetPermissionsResponse GetPermissions(string objectId);

    /// <inheritdoc cref="GetPermissions"/>
    Task<GetPermissionsResponse> GetPermissionsAsync(string objectId);

    /// <summary>
    /// Login to the Encryptonize service.
    /// </summary>
    void Login(string user, string password);

    /// <inheritdoc cref="Login"/>
    Task LoginAsync(string user, string password);

    /// <summary>
    /// Revoke a groups permission to access an object.
    /// </summary>
    /// <param name="objectId">The ID of the object.</param>
    /// <param name="groupId">The ID of the group.</param>
    void RemovePermission(string objectId, string groupId);

    /// <inheritdoc cref="RemovePermission"/>
    Task RemovePermissionAsync(string objectId, string groupId);

    /// <summary>
    /// Delete a user.
    /// </summary>
    /// <param name="userId">ID of the user to delete.</param>
    void RemoveUser(string userId);

    /// <inheritdoc cref="RemoveUser"/>
    Task RemoveUserAsync(string userId);

    /// <summary>
    /// Remove a user from a group.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="groupId">The ID of group.</param>
    void RemoveUserFromGroup(string userId, string groupId);

    /// <inheritdoc cref="RemoveUserFromGroup"/>
    Task RemoveUserFromGroupAsync(string userId, string groupId);

    /// <summary>
    /// Retreive some data encrypted in the storage attached to Encryptonize.
    /// </summary>
    /// <param name="objectId">The ID of the object.</param>
    /// <returns>An instance of <see cref="RetrieveResponse" />.</returns>
    RetrieveResponse Retrieve(string objectId);

    /// <inheritdoc cref="Retrieve"/>
    Task<RetrieveResponse> RetrieveAsync(string objectId);

    /// <summary>
    /// Store some data encrypted in the storage attached to Encryptonize.
    /// </summary>
    /// <param name="plaintext">The plaintext to store.</param>
    /// <param name="associatedData">The attached associated data.</param>
    /// <returns>An instance of <see cref="StoreResponse" />.</returns>
    StoreResponse Store(byte[] plaintext, byte[] associatedData);

    /// <inheritdoc cref="Store"/>
    Task<StoreResponse> StoreAsync(byte[] plaintext, byte[] associatedData);

    /// <summary>
    /// Update some data stored in the storage attached to Encryptonize.
    /// </summary>
    /// <param name="objectId">The ID of the object.</param>
    /// <param name="plaintext">The plaintext to store.</param>
    /// <param name="associatedData">The attached associated data.</param>
    void Update(string objectId, byte[] plaintext, byte[] associatedData);

    /// <inheritdoc cref="Update"/>
    Task UpdateAsync(string objectId, byte[] plaintext, byte[] associatedData);

    /// <summary>
    /// Get the version of the Encryptonize server.
    /// </summary>
    /// <returns>An instance of <see cref="VersionResponse"/>.</returns>
    VersionResponse Version();

    /// <inheritdoc cref="Version"/>
    Task<VersionResponse> VersionAsync();
}

/// <summary>
/// Client for connection to an Encryptonize server.
/// </summary>
/// <remarks>
/// Login is done on-demand and the access token is automatically refreshed when it expires.
/// </remarks>
public class EncryptonizeClient : IDisposable, IAsyncDisposable, IEncryptonizeClient
{
    private string password = string.Empty;
    internal string accessToken = string.Empty;

    /// <inheritdoc />
    public string User { get; private set; }

    /// <inheritdoc />
    public DateTime ExpiryTime { get; internal set; } = DateTime.MinValue.AddMinutes(1); // Have to add one minute to avoid exception because of underflow when calculating if the token is expired.

    private Metadata requestHeaders = new Metadata();
    private GrpcChannel channel;
    private Protobuf.Version.VersionClient versionClient;
    private Protobuf.Authn.AuthnClient authnClient;
    private Protobuf.Authz.AuthzClient authzClient;
    private Protobuf.Core.CoreClient coreClient;
    private Protobuf.Objects.ObjectsClient objectsClient;

    /// <summary>
    /// Initialize a new instance of the <see cref="EncryptonizeClient"/> class.
    /// </summary>
    /// <param name="endpoint">The endpoint of the Encryptonize server.</param>
    /// <param name="username">The username used to authenticate with the Encryptonize server.</param>
    /// <param name="password">The password used to authenticate with the Encryptonize server.</param>
    /// <param name="certPath">The optional path to the certificate used to authenticate with the Encryptonize server when mTLS is enabled.</param>
    /// <returns>A new instance of the <see cref="EncryptonizeClient"/> class.</returns>
    public EncryptonizeClient(string endpoint, string username, string password, string certPath = "")
    {
        if (string.IsNullOrWhiteSpace(certPath))
        {
            channel = GrpcChannel.ForAddress(endpoint);
        }
        else
        {
            var cert = new X509Certificate2(File.ReadAllBytes(certPath));

            var handler = new HttpClientHandler();
            handler.ClientCertificates.Add(cert);

            channel = GrpcChannel.ForAddress(endpoint, new GrpcChannelOptions { HttpHandler = handler });
        }

        versionClient = new(channel);
        authnClient = new(channel);
        authzClient = new(channel);
        coreClient = new(channel);
        objectsClient = new(channel);

        User = username;
        this.password = password;
    }

    /// <summary>
    /// Dispose the client.
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Dispose the client.
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            channel.Dispose();
        }
    }

    /// <summary>
    /// Asynchronously dispose the client.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);

        Dispose(disposing: false);
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
        GC.SuppressFinalize(this);
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
    }

    /// <summary>
    /// Asynchronously dispose the client.
    /// </summary>
    protected virtual async ValueTask DisposeAsyncCore()
    {
        await channel.ShutdownAsync().ConfigureAwait(false);
    }

    private async Task RefreshTokenAsync()
    {
        if (DateTime.Now > ExpiryTime.AddMinutes(-1))
        {
            await LoginAsync(User, password).ConfigureAwait(false);
        }
    }

    private void RefreshToken()
    {
        if (DateTime.Now > ExpiryTime.AddMinutes(-1))
        {
            Login(User, password);
        }
    }

    /////////////////////////////////////////////////////////////////////////
    //                               Utility                               //
    /////////////////////////////////////////////////////////////////////////

    /// <inheritdoc />
    public async Task<VersionResponse> VersionAsync()
    {
        await RefreshTokenAsync().ConfigureAwait(false);

        var response = await versionClient.VersionAsync(new Protobuf.VersionRequest(), requestHeaders).ConfigureAwait(false);

        return new VersionResponse(response.Commit, response.Tag);
    }

    /// <inheritdoc />
    public VersionResponse Version()
    {
        RefreshToken();

        var response = versionClient.Version(new Protobuf.VersionRequest(), requestHeaders);

        return new VersionResponse(response.Commit, response.Tag);
    }

    /////////////////////////////////////////////////////////////////////////
    //                           User Management                           //
    /////////////////////////////////////////////////////////////////////////

    /// <inheritdoc />
    public async Task LoginAsync(string user, string password)
    {
        var response = await authnClient.LoginUserAsync(new Protobuf.LoginUserRequest { UserId = user, Password = password }).ConfigureAwait(false);

        User = user;
        this.password = password;
        accessToken = response.AccessToken;
        ExpiryTime = new DateTime(response.ExpiryTime);

        requestHeaders = new Metadata();
        requestHeaders.Add("Authorization", $"Bearer {accessToken}");
    }

    /// <inheritdoc />
    public void Login(string user, string password)
    {
        var response = authnClient.LoginUser(new Protobuf.LoginUserRequest { UserId = user, Password = password });

        User = user;
        this.password = password;
        accessToken = response.AccessToken;
        ExpiryTime = new DateTime(response.ExpiryTime);

        requestHeaders = new Metadata();
        requestHeaders.Add("Authorization", $"Bearer {accessToken}");
    }

    /// <inheritdoc />
    public async Task<CreateUserResponse> CreateUserAsync(IList<Scope> scopes)
    {
        await RefreshTokenAsync().ConfigureAwait(false);

        var request = new Protobuf.CreateUserRequest();
        foreach (Scope scope in scopes)
        {
            request.Scopes.Add(scope.GetServiceScope());
        }

        var response = await authnClient.CreateUserAsync(request, requestHeaders).ConfigureAwait(false);

        return new CreateUserResponse(response.UserId, response.Password);
    }

    /// <inheritdoc />
    public CreateUserResponse CreateUser(IList<Scope> scopes)
    {
        RefreshToken();

        var request = new Protobuf.CreateUserRequest();
        foreach (Scope scope in scopes)
        {
            request.Scopes.Add(scope.GetServiceScope());
        }

        var response = authnClient.CreateUser(request, requestHeaders);

        return new CreateUserResponse(response.UserId, response.Password);
    }

    /// <inheritdoc />
    public async Task RemoveUserAsync(string userId)
    {
        await RefreshTokenAsync().ConfigureAwait(false);

        await authnClient.RemoveUserAsync(new Protobuf.RemoveUserRequest { UserId = userId }, requestHeaders).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public void RemoveUser(string userId)
    {
        RefreshToken();

        authnClient.RemoveUser(new Protobuf.RemoveUserRequest { UserId = userId }, requestHeaders);
    }

    /// <inheritdoc />
    public async Task<CreateGroupResponse> CreateGroupAsync(IList<Scope> scopes)
    {
        await RefreshTokenAsync().ConfigureAwait(false);

        var request = new Protobuf.CreateGroupRequest();
        foreach (Scope scope in scopes)
        {
            request.Scopes.Add(scope.GetServiceScope());
        }

        var response = await authnClient.CreateGroupAsync(request, requestHeaders).ConfigureAwait(false);

        return new CreateGroupResponse(response.GroupId);
    }

    /// <inheritdoc />
    public CreateGroupResponse CreateGroup(IList<Scope> scopes)
    {
        RefreshToken();

        var request = new Protobuf.CreateGroupRequest();
        foreach (Scope scope in scopes)
        {
            request.Scopes.Add(scope.GetServiceScope());
        }

        var response = authnClient.CreateGroup(request, requestHeaders);

        return new CreateGroupResponse(response.GroupId);
    }

    /// <inheritdoc />
    public async Task AddUserToGroupAsync(string userId, string groupId)
    {
        await RefreshTokenAsync().ConfigureAwait(false);

        await authnClient.AddUserToGroupAsync(new Protobuf.AddUserToGroupRequest { UserId = userId, GroupId = groupId }, requestHeaders)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public void AddUserToGroup(string userId, string groupId)
    {
        RefreshToken();

        authnClient.AddUserToGroup(new Protobuf.AddUserToGroupRequest { UserId = userId, GroupId = groupId }, requestHeaders);
    }

    /// <inheritdoc />
    public async Task RemoveUserFromGroupAsync(string userId, string groupId)
    {
        await RefreshTokenAsync().ConfigureAwait(false);

        await authnClient.RemoveUserFromGroupAsync(new Protobuf.RemoveUserFromGroupRequest { UserId = userId, GroupId = groupId },
            requestHeaders).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public void RemoveUserFromGroup(string userId, string groupId)
    {
        RefreshToken();

        authnClient.RemoveUserFromGroup(new Protobuf.RemoveUserFromGroupRequest { UserId = userId, GroupId = groupId },
            requestHeaders);
    }

    /////////////////////////////////////////////////////////////////////////
    //                              Encryption                             //
    /////////////////////////////////////////////////////////////////////////

    /// <inheritdoc />
    public async Task<EncryptResponse> EncryptAsync(byte[] plaintext, byte[] associatedData)
    {
        await RefreshTokenAsync().ConfigureAwait(false);

        var response = await coreClient.EncryptAsync(new Protobuf.EncryptRequest
        {
            Plaintext = ByteString.CopyFrom(plaintext),
            AssociatedData = ByteString.CopyFrom(associatedData)
        }, requestHeaders).ConfigureAwait(false);

        return new EncryptResponse(response.ObjectId, response.Ciphertext.ToByteArray(), response.AssociatedData.ToByteArray());
    }

    /// <inheritdoc />
    public EncryptResponse Encrypt(byte[] plaintext, byte[] associatedData)
    {
        RefreshToken();

        var response = coreClient.Encrypt(new Protobuf.EncryptRequest
        {
            Plaintext = ByteString.CopyFrom(plaintext),
            AssociatedData = ByteString.CopyFrom(associatedData)
        }, requestHeaders);

        return new EncryptResponse(response.ObjectId, response.Ciphertext.ToByteArray(), response.AssociatedData.ToByteArray());
    }

    /// <inheritdoc />
    public async Task<DecryptResponse> DecryptAsync(string objectId, byte[] ciphertext, byte[] associatedData)
    {
        await RefreshTokenAsync().ConfigureAwait(false);

        var response = await coreClient.DecryptAsync(new Protobuf.DecryptRequest
        {
            ObjectId = objectId,
            Ciphertext = ByteString.CopyFrom(ciphertext),
            AssociatedData = ByteString.CopyFrom(associatedData)
        }, requestHeaders)
            .ConfigureAwait(false);

        return new DecryptResponse(response.Plaintext.ToByteArray(), response.AssociatedData.ToByteArray());
    }

    /// <inheritdoc />
    public DecryptResponse Decrypt(string objectId, byte[] ciphertext, byte[] associatedData)
    {
        RefreshToken();

        var response = coreClient.Decrypt(new Protobuf.DecryptRequest
        {
            ObjectId = objectId,
            Ciphertext = ByteString.CopyFrom(ciphertext),
            AssociatedData = ByteString.CopyFrom(associatedData)
        }, requestHeaders);

        return new DecryptResponse(response.Plaintext.ToByteArray(), response.AssociatedData.ToByteArray());
    }

    /////////////////////////////////////////////////////////////////////////
    //                               Storage                               //
    /////////////////////////////////////////////////////////////////////////

    /// <inheritdoc />
    public async Task<StoreResponse> StoreAsync(byte[] plaintext, byte[] associatedData)
    {
        await RefreshTokenAsync().ConfigureAwait(false);

        var response = await objectsClient.StoreAsync(new Protobuf.StoreRequest
        {
            Plaintext = ByteString.CopyFrom(plaintext),
            AssociatedData = ByteString.CopyFrom(associatedData)
        }, requestHeaders).ConfigureAwait(false);

        return new StoreResponse(response.ObjectId);
    }

    /// <inheritdoc />
    public StoreResponse Store(byte[] plaintext, byte[] associatedData)
    {
        RefreshToken();

        var response = objectsClient.Store(new Protobuf.StoreRequest
        {
            Plaintext = ByteString.CopyFrom(plaintext),
            AssociatedData = ByteString.CopyFrom(associatedData)
        }, requestHeaders);

        return new StoreResponse(response.ObjectId);
    }

    /// <inheritdoc />
    public async Task<RetrieveResponse> RetrieveAsync(string objectId)
    {
        await RefreshTokenAsync().ConfigureAwait(false);

        var response = await objectsClient.RetrieveAsync(new Protobuf.RetrieveRequest { ObjectId = objectId }, requestHeaders).ConfigureAwait(false);

        return new RetrieveResponse(response.Plaintext.ToByteArray(), response.AssociatedData.ToByteArray());
    }

    /// <inheritdoc />
    public RetrieveResponse Retrieve(string objectId)
    {
        RefreshToken();

        var response = objectsClient.Retrieve(new Protobuf.RetrieveRequest { ObjectId = objectId }, requestHeaders);

        return new RetrieveResponse(response.Plaintext.ToByteArray(), response.AssociatedData.ToByteArray());
    }

    /// <inheritdoc />
    public async Task UpdateAsync(string objectId, byte[] plaintext, byte[] associatedData)
    {
        await RefreshTokenAsync().ConfigureAwait(false);

        await objectsClient.UpdateAsync(new Protobuf.UpdateRequest
        {
            ObjectId = objectId,
            Plaintext = ByteString.CopyFrom(plaintext),
            AssociatedData = ByteString.CopyFrom(associatedData)
        }, requestHeaders).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public void Update(string objectId, byte[] plaintext, byte[] associatedData)
    {
        RefreshToken();

        objectsClient.Update(new Protobuf.UpdateRequest
        {
            ObjectId = objectId,
            Plaintext = ByteString.CopyFrom(plaintext),
            AssociatedData = ByteString.CopyFrom(associatedData)
        }, requestHeaders);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(string objectId)
    {
        await RefreshTokenAsync().ConfigureAwait(false);

        await objectsClient.DeleteAsync(new Protobuf.DeleteRequest { ObjectId = objectId }, requestHeaders).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public void Delete(string objectId)
    {
        RefreshToken();

        objectsClient.Delete(new Protobuf.DeleteRequest { ObjectId = objectId }, requestHeaders);
    }

    /////////////////////////////////////////////////////////////////////////
    //                             Permissions                             //
    /////////////////////////////////////////////////////////////////////////

    /// <inheritdoc />
    public async Task<GetPermissionsResponse> GetPermissionsAsync(string objectId)
    {
        await RefreshTokenAsync().ConfigureAwait(false);

        var response = await authzClient.GetPermissionsAsync(new Protobuf.GetPermissionsRequest { ObjectId = objectId }, requestHeaders)
            .ConfigureAwait(false);

        return new GetPermissionsResponse(new List<string>(response.GroupIds));
    }

    /// <inheritdoc />
    public GetPermissionsResponse GetPermissions(string objectId)
    {
        RefreshToken();

        var response = authzClient.GetPermissions(new Protobuf.GetPermissionsRequest { ObjectId = objectId }, requestHeaders);

        return new GetPermissionsResponse(new List<string>(response.GroupIds));
    }

    /// <inheritdoc />
    public async Task AddPermissionAsync(string objectId, string groupId)
    {
        await RefreshTokenAsync().ConfigureAwait(false);

        await authzClient.AddPermissionAsync(new Protobuf.AddPermissionRequest { ObjectId = objectId, GroupId = groupId }, requestHeaders)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public void AddPermission(string objectId, string groupId)
    {
        RefreshToken();

        authzClient.AddPermission(new Protobuf.AddPermissionRequest { ObjectId = objectId, GroupId = groupId }, requestHeaders);
    }

    /// <inheritdoc />
    public async Task RemovePermissionAsync(string objectId, string groupId)
    {
        await RefreshTokenAsync().ConfigureAwait(false);

        await authzClient.RemovePermissionAsync(new Protobuf.RemovePermissionRequest { ObjectId = objectId, GroupId = groupId }, requestHeaders)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public void RemovePermission(string objectId, string groupId)
    {
        RefreshToken();

        authzClient.RemovePermission(new Protobuf.RemovePermissionRequest { ObjectId = objectId, GroupId = groupId }, requestHeaders);
    }
}
