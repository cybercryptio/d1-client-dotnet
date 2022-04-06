// Copyright 2020-2022 CYBERCRYPT
using System.Security.Cryptography.X509Certificates;
using System.Runtime.CompilerServices;
using Grpc.Net.Client;
using Grpc.Core;
using Google.Protobuf;
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
    private App.Encryptonize.EncryptonizeClient appClient;
    private Authn.Encryptonize.EncryptonizeClient authnClient;
    private Enc.Encryptonize.EncryptonizeClient encClient;
    private Storage.Encryptonize.EncryptonizeClient storageClient;
    private Authz.Encryptonize.EncryptonizeClient authzClient;

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

        appClient = new App.Encryptonize.EncryptonizeClient(channel);
        authnClient = new Authn.Encryptonize.EncryptonizeClient(channel);
        encClient = new Enc.Encryptonize.EncryptonizeClient(channel);
        storageClient = new Storage.Encryptonize.EncryptonizeClient(channel);
        authzClient = new Authz.Encryptonize.EncryptonizeClient(channel);

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

        var response = await appClient.VersionAsync(new App.VersionRequest(), requestHeaders).ConfigureAwait(false);

        return new VersionResponse(response.Commit, response.Tag);
    }

    public VersionResponse Version()
    {
        RefreshToken();

        var response = appClient.Version(new App.VersionRequest(), requestHeaders);

        return new VersionResponse(response.Commit, response.Tag);
    }

    /////////////////////////////////////////////////////////////////////////
    //                           User Management                           //
    /////////////////////////////////////////////////////////////////////////

    public async Task LoginAsync(string user, string password)
    {
        var response = await authnClient.LoginUserAsync(new Authn.LoginUserRequest { UserId = user, Password = password }).ConfigureAwait(false);

        User = user;
        this.password = password;
        accessToken = response.AccessToken;
        ExpiryTime = new DateTime(response.ExpiryTime);

        requestHeaders = new Metadata();
        requestHeaders.Add("Authorization", $"Bearer {accessToken}");
    }

    public void Login(string user, string password)
    {
        var response = authnClient.LoginUser(new Authn.LoginUserRequest { UserId = user, Password = password });

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

        var request = new Authn.CreateUserRequest();
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

        var request = new Authn.CreateUserRequest();
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

        await authnClient.RemoveUserAsync(new Authn.RemoveUserRequest { UserId = userId }, requestHeaders).ConfigureAwait(false);
    }

    public void RemoveUser(string userId)
    {
        RefreshToken();

        authnClient.RemoveUser(new Authn.RemoveUserRequest { UserId = userId }, requestHeaders);
    }

    public async Task<CreateGroupResponse> CreateGroupAsync(IList<Scope> scopes)
    {
        await RefreshTokenAsync().ConfigureAwait(false);

        var request = new Authn.CreateGroupRequest();
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

        var request = new Authn.CreateGroupRequest();
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

        await authnClient.AddUserToGroupAsync(new Authn.AddUserToGroupRequest { UserId = userId, GroupId = groupId }, requestHeaders)
            .ConfigureAwait(false);
    }

    public void AddUserToGroup(string userId, string groupId)
    {
        RefreshToken();

        authnClient.AddUserToGroup(new Authn.AddUserToGroupRequest { UserId = userId, GroupId = groupId }, requestHeaders);
    }

    public async Task RemoveUserFromGroupAsync(string userId, string groupId)
    {
        await RefreshTokenAsync().ConfigureAwait(false);

        await authnClient.RemoveUserFromGroupAsync(new Authn.RemoveUserFromGroupRequest { UserId = userId, GroupId = groupId },
            requestHeaders).ConfigureAwait(false);
    }

    public void RemoveUserFromGroup(string userId, string groupId)
    {
        RefreshToken();

        authnClient.RemoveUserFromGroup(new Authn.RemoveUserFromGroupRequest { UserId = userId, GroupId = groupId },
            requestHeaders);
    }

    /////////////////////////////////////////////////////////////////////////
    //                              Encryption                             //
    /////////////////////////////////////////////////////////////////////////

    public async Task<EncryptResponse> EncryptAsync(byte[] plaintext, byte[] associatedData)
    {
        await RefreshTokenAsync().ConfigureAwait(false);

        var response = await encClient.EncryptAsync(new Enc.EncryptRequest
        {
            Plaintext = ByteString.CopyFrom(plaintext),
            AssociatedData = ByteString.CopyFrom(associatedData)
        }, requestHeaders).ConfigureAwait(false);

        return new EncryptResponse(response.ObjectId, response.Ciphertext.ToByteArray(), response.AssociatedData.ToByteArray());
    }

    public EncryptResponse Encrypt(byte[] plaintext, byte[] associatedData)
    {
        RefreshToken();

        var response = encClient.Encrypt(new Enc.EncryptRequest
        {
            Plaintext = ByteString.CopyFrom(plaintext),
            AssociatedData = ByteString.CopyFrom(associatedData)
        }, requestHeaders);

        return new EncryptResponse(response.ObjectId, response.Ciphertext.ToByteArray(), response.AssociatedData.ToByteArray());
    }

    public async Task<DecryptResponse> DecryptAsync(string objectId, byte[] ciphertext, byte[] associatedData)
    {
        await RefreshTokenAsync().ConfigureAwait(false);

        var response = await encClient.DecryptAsync(new Enc.DecryptRequest
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

        var response = encClient.Decrypt(new Enc.DecryptRequest
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

        var response = await storageClient.StoreAsync(new Storage.StoreRequest
        {
            Plaintext = ByteString.CopyFrom(plaintext),
            AssociatedData = ByteString.CopyFrom(associatedData)
        }, requestHeaders).ConfigureAwait(false);

        return new StoreResponse(response.ObjectId);
    }

    public StoreResponse Store(byte[] plaintext, byte[] associatedData)
    {
        RefreshToken();

        var response = storageClient.Store(new Storage.StoreRequest
        {
            Plaintext = ByteString.CopyFrom(plaintext),
            AssociatedData = ByteString.CopyFrom(associatedData)
        }, requestHeaders);

        return new StoreResponse(response.ObjectId);
    }

    public async Task<RetrieveResponse> RetrieveAsync(string objectId)
    {
        await RefreshTokenAsync().ConfigureAwait(false);

        var response = await storageClient.RetrieveAsync(new Storage.RetrieveRequest { ObjectId = objectId }, requestHeaders).ConfigureAwait(false);

        return new RetrieveResponse(response.Plaintext.ToByteArray(), response.AssociatedData.ToByteArray());
    }

    public RetrieveResponse Retrieve(string objectId)
    {
        RefreshToken();

        var response = storageClient.Retrieve(new Storage.RetrieveRequest { ObjectId = objectId }, requestHeaders);

        return new RetrieveResponse(response.Plaintext.ToByteArray(), response.AssociatedData.ToByteArray());
    }

    public async Task UpdateAsync(string objectId, byte[] plaintext, byte[] associatedData)
    {
        await RefreshTokenAsync().ConfigureAwait(false);

        await storageClient.UpdateAsync(new Storage.UpdateRequest
        {
            ObjectId = objectId,
            Plaintext = ByteString.CopyFrom(plaintext),
            AssociatedData = ByteString.CopyFrom(associatedData)
        }, requestHeaders).ConfigureAwait(false);
    }

    public void Update(string objectId, byte[] plaintext, byte[] associatedData)
    {
        RefreshToken();

        storageClient.Update(new Storage.UpdateRequest
        {
            ObjectId = objectId,
            Plaintext = ByteString.CopyFrom(plaintext),
            AssociatedData = ByteString.CopyFrom(associatedData)
        }, requestHeaders);
    }

    public async Task DeleteAsync(string objectId)
    {
        await RefreshTokenAsync().ConfigureAwait(false);

        await storageClient.DeleteAsync(new Storage.DeleteRequest { ObjectId = objectId }, requestHeaders).ConfigureAwait(false);
    }

    public void Delete(string objectId)
    {
        RefreshToken();

        storageClient.Delete(new Storage.DeleteRequest { ObjectId = objectId }, requestHeaders);
    }

    /////////////////////////////////////////////////////////////////////////
    //                             Permissions                             //
    /////////////////////////////////////////////////////////////////////////

    public async Task<GetPermissionsResponse> GetPermissionsAsync(string objectId)
    {
        await RefreshTokenAsync().ConfigureAwait(false);

        var response = await authzClient.GetPermissionsAsync(new Authz.GetPermissionsRequest { ObjectId = objectId }, requestHeaders)
            .ConfigureAwait(false);

        return new GetPermissionsResponse(new List<string>(response.GroupIds));
    }

    public GetPermissionsResponse GetPermissions(string objectId)
    {
        RefreshToken();

        var response = authzClient.GetPermissions(new Authz.GetPermissionsRequest { ObjectId = objectId }, requestHeaders);

        return new GetPermissionsResponse(new List<string>(response.GroupIds));
    }

    public async Task AddPermissionAsync(string objectId, string groupId)
    {
        await RefreshTokenAsync().ConfigureAwait(false);

        await authzClient.AddPermissionAsync(new Authz.AddPermissionRequest { ObjectId = objectId, GroupId = groupId }, requestHeaders)
            .ConfigureAwait(false);
    }

    public void AddPermission(string objectId, string groupId)
    {
        RefreshToken();

        authzClient.AddPermission(new Authz.AddPermissionRequest { ObjectId = objectId, GroupId = groupId }, requestHeaders);
    }

    public async Task RemovePermissionAsync(string objectId, string groupId)
    {
        await RefreshTokenAsync().ConfigureAwait(false);

        await authzClient.RemovePermissionAsync(new Authz.RemovePermissionRequest { ObjectId = objectId, GroupId = groupId }, requestHeaders)
            .ConfigureAwait(false);
    }

    public void RemovePermission(string objectId, string groupId)
    {
        RefreshToken();

        authzClient.RemovePermission(new Authz.RemovePermissionRequest { ObjectId = objectId, GroupId = groupId }, requestHeaders);
    }
}
