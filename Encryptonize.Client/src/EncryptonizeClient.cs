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

public interface IEncryptonizeClient
{
    string User { get; }
    DateTime ExpiryTime { get; }

    void AddPermission(string objectId, string groupId);
    Task AddPermissionAsync(string objectId, string groupId);
    void AddUserToGroup(string userId, string groupId);
    Task AddUserToGroupAsync(string userId, string groupId);
    CreateGroupResponse CreateGroup(IList<Scope> scopes);
    Task<CreateGroupResponse> CreateGroupAsync(IList<Scope> scopes);
    CreateUserResponse CreateUser(IList<Scope> scopes);
    Task<CreateUserResponse> CreateUserAsync(IList<Scope> scopes);
    DecryptResponse Decrypt(string objectId, byte[] ciphertext, byte[] associatedData);
    Task<DecryptResponse> DecryptAsync(string objectId, byte[] ciphertext, byte[] associatedData);
    void Delete(string objectId);
    Task DeleteAsync(string objectId);
    void Dispose();
    ValueTask DisposeAsync();
    EncryptResponse Encrypt(byte[] plaintext, byte[] associatedData);
    Task<EncryptResponse> EncryptAsync(byte[] plaintext, byte[] associatedData);
    GetPermissionsResponse GetPermissions(string objectId);
    Task<GetPermissionsResponse> GetPermissionsAsync(string objectId);
    void Login(string user, string password);
    Task LoginAsync(string user, string password);
    void RemovePermission(string objectId, string groupId);
    Task RemovePermissionAsync(string objectId, string groupId);
    void RemoveUser(string userId);
    Task RemoveUserAsync(string userId);
    void RemoveUserFromGroup(string userId, string groupId);
    Task RemoveUserFromGroupAsync(string userId, string groupId);
    RetrieveResponse Retrieve(string objectId);
    Task<RetrieveResponse> RetrieveAsync(string objectId);
    StoreResponse Store(byte[] plaintext, byte[] associatedData);
    Task<StoreResponse> StoreAsync(byte[] plaintext, byte[] associatedData);
    void Update(string objectId, byte[] plaintext, byte[] associatedData);
    Task UpdateAsync(string objectId, byte[] plaintext, byte[] associatedData);
    VersionResponse Version();
    Task<VersionResponse> VersionAsync();
}

public class EncryptonizeClient : IDisposable, IAsyncDisposable, IEncryptonizeClient
{
    private string password = string.Empty;
    internal string accessToken = string.Empty;
    public string User { get; private set; }
    public DateTime ExpiryTime { get; internal set; } = DateTime.MinValue.AddMinutes(1); // Have to add one minute to avoid exception because of underflow when calculating if the token is expired.
    private Metadata requestHeaders = new Metadata();
    private GrpcChannel channel;
    private Protobuf.Version.VersionClient versionClient;
    private Protobuf.Authn.AuthnClient authnClient;
    private Protobuf.Authz.AuthzClient authzClient;
    private Protobuf.EAAS.EAASClient eaasClient;
    private Protobuf.Objects.ObjectsClient objectsClient;

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
        eaasClient = new(channel);
        objectsClient = new(channel);

        User = username;
        this.password = password;
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            channel.Dispose();
        }
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);

        Dispose(disposing: false);
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
        GC.SuppressFinalize(this);
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
    }

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

    public async Task<VersionResponse> VersionAsync()
    {
        await RefreshTokenAsync().ConfigureAwait(false);

        var response = await versionClient.VersionAsync(new Protobuf.VersionRequest(), requestHeaders).ConfigureAwait(false);

        return new VersionResponse(response.Commit, response.Tag);
    }

    public VersionResponse Version()
    {
        RefreshToken();

        var response = versionClient.Version(new Protobuf.VersionRequest(), requestHeaders);

        return new VersionResponse(response.Commit, response.Tag);
    }

    /////////////////////////////////////////////////////////////////////////
    //                           User Management                           //
    /////////////////////////////////////////////////////////////////////////

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

    public async Task RemoveUserAsync(string userId)
    {
        await RefreshTokenAsync().ConfigureAwait(false);

        await authnClient.RemoveUserAsync(new Protobuf.RemoveUserRequest { UserId = userId }, requestHeaders).ConfigureAwait(false);
    }

    public void RemoveUser(string userId)
    {
        RefreshToken();

        authnClient.RemoveUser(new Protobuf.RemoveUserRequest { UserId = userId }, requestHeaders);
    }

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

    public async Task AddUserToGroupAsync(string userId, string groupId)
    {
        await RefreshTokenAsync().ConfigureAwait(false);

        await authnClient.AddUserToGroupAsync(new Protobuf.AddUserToGroupRequest { UserId = userId, GroupId = groupId }, requestHeaders)
            .ConfigureAwait(false);
    }

    public void AddUserToGroup(string userId, string groupId)
    {
        RefreshToken();

        authnClient.AddUserToGroup(new Protobuf.AddUserToGroupRequest { UserId = userId, GroupId = groupId }, requestHeaders);
    }

    public async Task RemoveUserFromGroupAsync(string userId, string groupId)
    {
        await RefreshTokenAsync().ConfigureAwait(false);

        await authnClient.RemoveUserFromGroupAsync(new Protobuf.RemoveUserFromGroupRequest { UserId = userId, GroupId = groupId },
            requestHeaders).ConfigureAwait(false);
    }

    public void RemoveUserFromGroup(string userId, string groupId)
    {
        RefreshToken();

        authnClient.RemoveUserFromGroup(new Protobuf.RemoveUserFromGroupRequest { UserId = userId, GroupId = groupId },
            requestHeaders);
    }

    /////////////////////////////////////////////////////////////////////////
    //                              Encryption                             //
    /////////////////////////////////////////////////////////////////////////

    public async Task<EncryptResponse> EncryptAsync(byte[] plaintext, byte[] associatedData)
    {
        await RefreshTokenAsync().ConfigureAwait(false);

        var response = await eaasClient.EncryptAsync(new Protobuf.EncryptRequest
        {
            Plaintext = ByteString.CopyFrom(plaintext),
            AssociatedData = ByteString.CopyFrom(associatedData)
        }, requestHeaders).ConfigureAwait(false);

        return new EncryptResponse(response.ObjectId, response.Ciphertext.ToByteArray(), response.AssociatedData.ToByteArray());
    }

    public EncryptResponse Encrypt(byte[] plaintext, byte[] associatedData)
    {
        RefreshToken();

        var response = eaasClient.Encrypt(new Protobuf.EncryptRequest
        {
            Plaintext = ByteString.CopyFrom(plaintext),
            AssociatedData = ByteString.CopyFrom(associatedData)
        }, requestHeaders);

        return new EncryptResponse(response.ObjectId, response.Ciphertext.ToByteArray(), response.AssociatedData.ToByteArray());
    }

    public async Task<DecryptResponse> DecryptAsync(string objectId, byte[] ciphertext, byte[] associatedData)
    {
        await RefreshTokenAsync().ConfigureAwait(false);

        var response = await eaasClient.DecryptAsync(new Protobuf.DecryptRequest
        {
            ObjectId = objectId,
            Ciphertext = ByteString.CopyFrom(ciphertext),
            AssociatedData = ByteString.CopyFrom(associatedData)
        }, requestHeaders)
            .ConfigureAwait(false);

        return new DecryptResponse(response.Plaintext.ToByteArray(), response.AssociatedData.ToByteArray());
    }

    public DecryptResponse Decrypt(string objectId, byte[] ciphertext, byte[] associatedData)
    {
        RefreshToken();

        var response = eaasClient.Decrypt(new Protobuf.DecryptRequest
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

    public async Task<RetrieveResponse> RetrieveAsync(string objectId)
    {
        await RefreshTokenAsync().ConfigureAwait(false);

        var response = await objectsClient.RetrieveAsync(new Protobuf.RetrieveRequest { ObjectId = objectId }, requestHeaders).ConfigureAwait(false);

        return new RetrieveResponse(response.Plaintext.ToByteArray(), response.AssociatedData.ToByteArray());
    }

    public RetrieveResponse Retrieve(string objectId)
    {
        RefreshToken();

        var response = objectsClient.Retrieve(new Protobuf.RetrieveRequest { ObjectId = objectId }, requestHeaders);

        return new RetrieveResponse(response.Plaintext.ToByteArray(), response.AssociatedData.ToByteArray());
    }

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

    public async Task DeleteAsync(string objectId)
    {
        await RefreshTokenAsync().ConfigureAwait(false);

        await objectsClient.DeleteAsync(new Protobuf.DeleteRequest { ObjectId = objectId }, requestHeaders).ConfigureAwait(false);
    }

    public void Delete(string objectId)
    {
        RefreshToken();

        objectsClient.Delete(new Protobuf.DeleteRequest { ObjectId = objectId }, requestHeaders);
    }

    /////////////////////////////////////////////////////////////////////////
    //                             Permissions                             //
    /////////////////////////////////////////////////////////////////////////

    public async Task<GetPermissionsResponse> GetPermissionsAsync(string objectId)
    {
        await RefreshTokenAsync().ConfigureAwait(false);

        var response = await authzClient.GetPermissionsAsync(new Protobuf.GetPermissionsRequest { ObjectId = objectId }, requestHeaders)
            .ConfigureAwait(false);

        return new GetPermissionsResponse(new List<string>(response.GroupIds));
    }

    public GetPermissionsResponse GetPermissions(string objectId)
    {
        RefreshToken();

        var response = authzClient.GetPermissions(new Protobuf.GetPermissionsRequest { ObjectId = objectId }, requestHeaders);

        return new GetPermissionsResponse(new List<string>(response.GroupIds));
    }

    public async Task AddPermissionAsync(string objectId, string groupId)
    {
        await RefreshTokenAsync().ConfigureAwait(false);

        await authzClient.AddPermissionAsync(new Protobuf.AddPermissionRequest { ObjectId = objectId, GroupId = groupId }, requestHeaders)
            .ConfigureAwait(false);
    }

    public void AddPermission(string objectId, string groupId)
    {
        RefreshToken();

        authzClient.AddPermission(new Protobuf.AddPermissionRequest { ObjectId = objectId, GroupId = groupId }, requestHeaders);
    }

    public async Task RemovePermissionAsync(string objectId, string groupId)
    {
        await RefreshTokenAsync().ConfigureAwait(false);

        await authzClient.RemovePermissionAsync(new Protobuf.RemovePermissionRequest { ObjectId = objectId, GroupId = groupId }, requestHeaders)
            .ConfigureAwait(false);
    }

    public void RemovePermission(string objectId, string groupId)
    {
        RefreshToken();

        authzClient.RemovePermission(new Protobuf.RemovePermissionRequest { ObjectId = objectId, GroupId = groupId }, requestHeaders);
    }
}
