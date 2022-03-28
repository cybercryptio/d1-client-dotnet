// Copyright 2020-2022 CYBERCRYPT
using System.Security.Cryptography.X509Certificates;
using Grpc.Net.Client;
using Grpc.Core;
using Google.Protobuf;
using Encryptonize.Client.Utils;
using Encryptonize.Client.Response;

namespace Encryptonize.Client;

public class Client : IDisposable, IAsyncDisposable {
    public string? AccessToken { get; private set; }
    public long ExpiryTime { get; private set; }
    private Metadata requestHeaders = new Metadata();
    private GrpcChannel channel;
    private App.Encryptonize.EncryptonizeClient appClient;
    private Authn.Encryptonize.EncryptonizeClient authnClient;
    private Enc.Encryptonize.EncryptonizeClient encClient;
    private Storage.Encryptonize.EncryptonizeClient storageClient;
    private Authz.Encryptonize.EncryptonizeClient authzClient;

    public Client(string endpoint, string certPath =  "") {
        if (string.IsNullOrWhiteSpace(certPath)) {
            channel = GrpcChannel.ForAddress(endpoint);
        } else {
            var cert = new X509Certificate2(File.ReadAllBytes(certPath));

            var handler = new HttpClientHandler();
            handler.ClientCertificates.Add(cert);

            channel = GrpcChannel.ForAddress(endpoint, new GrpcChannelOptions{HttpHandler = handler});
        }
        
        appClient = new App.Encryptonize.EncryptonizeClient(channel);
        authnClient = new Authn.Encryptonize.EncryptonizeClient(channel);
        encClient = new Enc.Encryptonize.EncryptonizeClient(channel);
        storageClient = new Storage.Encryptonize.EncryptonizeClient(channel);
        authzClient = new Authz.Encryptonize.EncryptonizeClient(channel);
    }

    public void Dispose() {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing) {
        if (disposing) {
            channel.Dispose();
        }
    }

    public async ValueTask DisposeAsync() {
        await DisposeAsyncCore().ConfigureAwait(false);

        Dispose(disposing: false);
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
        GC.SuppressFinalize(this);
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
    }

    protected virtual async ValueTask DisposeAsyncCore() {
        await channel.ShutdownAsync().ConfigureAwait(false);
    }

    /////////////////////////////////////////////////////////////////////////
    //                               Utility                               //
    /////////////////////////////////////////////////////////////////////////

    public async Task<VersionResponse> Version() {
        var response = await appClient.VersionAsync(new App.VersionRequest(), requestHeaders).ConfigureAwait(false);
        return new VersionResponse(response.Commit, response.Tag);
    }

    /////////////////////////////////////////////////////////////////////////
    //                           User Management                           //
    /////////////////////////////////////////////////////////////////////////

    public async Task Login(string user, string password) {
        var response = await authnClient.LoginUserAsync(new Authn.LoginUserRequest{ UserId = user, Password = password }).ConfigureAwait(false);

        AccessToken = response.AccessToken;
        ExpiryTime = response.ExpiryTime;

        requestHeaders = new Metadata();
        requestHeaders.Add("Authorization", $"Bearer {AccessToken}");
    }

    public async Task<CreateUserResponse> CreateUser(IList<Scope> scopes) {
        var request = new Authn.CreateUserRequest();
        foreach (Scope scope in scopes) {
            request.Scopes.Add(scope.GetServiceScope());
        }

        var response = await authnClient.CreateUserAsync(request, requestHeaders).ConfigureAwait(false);

        return new CreateUserResponse(response.UserId, response.Password);
    }

    public async Task RemoveUser(string userId) {
        await authnClient.RemoveUserAsync(new Authn.RemoveUserRequest{ UserId = userId }, requestHeaders).ConfigureAwait(false);
    }

    public async Task<CreateGroupResponse> CreateGroup(IList<Scope> scopes) {
        var request = new Authn.CreateGroupRequest();
        foreach (Scope scope in scopes) {
            request.Scopes.Add(scope.GetServiceScope());
        }

        var response = await authnClient.CreateGroupAsync(request, requestHeaders).ConfigureAwait(false);

        return new CreateGroupResponse(response.GroupId);
    }

    public async Task AddUserToGroup(string userId, string groupId) {
        await authnClient.AddUserToGroupAsync(new Authn.AddUserToGroupRequest{ UserId = userId, GroupId = groupId }, requestHeaders)
            .ConfigureAwait(false);
    }

    public async Task RemoveUserFromGroup(string userId, string groupId) {
        await authnClient.RemoveUserFromGroupAsync(new Authn.RemoveUserFromGroupRequest{ UserId = userId, GroupId = groupId },
            requestHeaders).ConfigureAwait(false);
    }

    /////////////////////////////////////////////////////////////////////////
    //                              Encryption                             //
    /////////////////////////////////////////////////////////////////////////

    public async Task<EncryptResponse> Encrypt(byte[] plaintext, byte[] associatedData) {
        var response = await encClient.EncryptAsync(new Enc.EncryptRequest{ Plaintext = ByteString.CopyFrom(plaintext),
            AssociatedData = ByteString.CopyFrom(associatedData) }, requestHeaders).ConfigureAwait(false);
        return new EncryptResponse(response.ObjectId, response.Ciphertext.ToByteArray(), response.AssociatedData.ToByteArray());
    }

    public async Task<DecryptResponse> Decrypt(string objectId, byte[] ciphertext, byte[] associatedData) {
        var response = await encClient.DecryptAsync(new Enc.DecryptRequest{ ObjectId = objectId,
            Ciphertext = ByteString.CopyFrom(ciphertext), AssociatedData = ByteString.CopyFrom(associatedData) }, requestHeaders)
            .ConfigureAwait(false);
        return new DecryptResponse(response.Plaintext.ToByteArray(), response.AssociatedData.ToByteArray());
    }

    /////////////////////////////////////////////////////////////////////////
    //                               Storage                               //
    /////////////////////////////////////////////////////////////////////////

    public async Task<StoreResponse> Store(byte[] plaintext, byte[] associatedData) {
        var response = await storageClient.StoreAsync(new Storage.StoreRequest{ Plaintext = ByteString.CopyFrom(plaintext),
            AssociatedData = ByteString.CopyFrom(associatedData) }, requestHeaders).ConfigureAwait(false);
        return new StoreResponse(response.ObjectId);
    }

    public async Task<RetrieveResponse> Retrieve(string objectId) {
        var response = await storageClient.RetrieveAsync(new Storage.RetrieveRequest{ ObjectId = objectId }, requestHeaders).ConfigureAwait(false);
        return new RetrieveResponse(response.Plaintext.ToByteArray(), response.AssociatedData.ToByteArray());
    }

    public async Task Update(string objectId, byte[] plaintext, byte[] associatedData) {
        await storageClient.UpdateAsync(new Storage.UpdateRequest{ ObjectId = objectId, Plaintext = ByteString.CopyFrom(plaintext),
            AssociatedData = ByteString.CopyFrom(associatedData) }, requestHeaders).ConfigureAwait(false);
    }

    public async Task Delete(string objectId) {
        await storageClient.DeleteAsync(new Storage.DeleteRequest{ ObjectId = objectId }, requestHeaders).ConfigureAwait(false);
    }

    /////////////////////////////////////////////////////////////////////////
    //                             Permissions                             //
    /////////////////////////////////////////////////////////////////////////

    public async Task<GetPermissionsResponse> GetPermissions(string objectId) {
        var response = await authzClient.GetPermissionsAsync(new Authz.GetPermissionsRequest{ ObjectId = objectId }, requestHeaders)
            .ConfigureAwait(false);
        return new GetPermissionsResponse(new List<string>(response.GroupIds));
    }

    public async Task AddPermission(string objectId, string groupId) {
        await authzClient.AddPermissionAsync(new Authz.AddPermissionRequest{ ObjectId = objectId, GroupId = groupId }, requestHeaders)
            .ConfigureAwait(false);
    }

    public async Task RemovePermission(string objectId, string groupId) {
        await authzClient.RemovePermissionAsync(new Authz.RemovePermissionRequest{ ObjectId = objectId, GroupId = groupId }, requestHeaders)
            .ConfigureAwait(false);
    }
}
