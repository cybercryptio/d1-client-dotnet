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

public class Client : IDisposable, IAsyncDisposable {
    public string User { get; private set; }
    public string Password { get; private set; }
    public string? AccessToken { get; private set; }
    public DateTime ExpiryTime { get; internal set; }
    private Metadata requestHeaders = new Metadata();
    private GrpcChannel channel;
    private Protobuf.Version.VersionClient versionClient;
    private Protobuf.Authn.AuthnClient authnClient;
    private Protobuf.Authz.AuthzClient authzClient;
    private Protobuf.EAAS.EAASClient eaasClient;
    private Protobuf.Objects.ObjectsClient objectsClient;

    private Client(string endpoint, string certPath =  "") {
        if (string.IsNullOrWhiteSpace(certPath)) {
            channel = GrpcChannel.ForAddress(endpoint);
        } else {
            var cert = new X509Certificate2(File.ReadAllBytes(certPath));

            var handler = new HttpClientHandler();
            handler.ClientCertificates.Add(cert);

            channel = GrpcChannel.ForAddress(endpoint, new GrpcChannelOptions{HttpHandler = handler});
        }
        
        versionClient = new Protobuf.Version.VersionClient(channel);
        authnClient = new Protobuf.Authn.AuthnClient(channel);
        authzClient = new Protobuf.Authz.AuthzClient(channel);
        eaasClient = new Protobuf.EAAS.EAASClient(channel);
        objectsClient = new Protobuf.Objects.ObjectsClient(channel);

        User = "";
        Password = "";
    }

    public static async Task<Client> New(string endpoint, string user, string password, string certPath =  "") {
        var client = new Client(endpoint, certPath);
        await client.Login(user, password).ConfigureAwait(false);
        return client;
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

    private async Task RefreshToken() {
        if (DateTime.Now > ExpiryTime.AddMinutes(-1)) {
            await Login(User, Password).ConfigureAwait(false);
        }
    }

    /////////////////////////////////////////////////////////////////////////
    //                               Utility                               //
    /////////////////////////////////////////////////////////////////////////

    public async Task<VersionResponse> Version() {
        await RefreshToken().ConfigureAwait(false);

        var response = await versionClient.VersionAsync(new Protobuf.VersionRequest(), requestHeaders).ConfigureAwait(false);

        return new VersionResponse(response.Commit, response.Tag);
    }

    /////////////////////////////////////////////////////////////////////////
    //                           User Management                           //
    /////////////////////////////////////////////////////////////////////////

    public async Task Login(string user, string password) {
        var response = await authnClient.LoginUserAsync(new Protobuf.LoginUserRequest{ UserId = user, Password = password }).ConfigureAwait(false);

        User = user;
        Password = password;
        AccessToken = response.AccessToken;
        ExpiryTime = new DateTime(response.ExpiryTime);

        requestHeaders = new Metadata();
        requestHeaders.Add("Authorization", $"Bearer {AccessToken}");
    }

    public async Task<CreateUserResponse> CreateUser(IList<Scope> scopes) {
        await RefreshToken().ConfigureAwait(false);

        var request = new Protobuf.CreateUserRequest();
        foreach (Scope scope in scopes) {
            request.Scopes.Add(scope.GetServiceScope());
        }

        var response = await authnClient.CreateUserAsync(request, requestHeaders).ConfigureAwait(false);

        return new CreateUserResponse(response.UserId, response.Password);
    }

    public async Task RemoveUser(string userId) {
        await RefreshToken().ConfigureAwait(false);

        await authnClient.RemoveUserAsync(new Protobuf.RemoveUserRequest{ UserId = userId }, requestHeaders).ConfigureAwait(false);
    }

    public async Task<CreateGroupResponse> CreateGroup(IList<Scope> scopes) {
        await RefreshToken().ConfigureAwait(false);

        var request = new Protobuf.CreateGroupRequest();
        foreach (Scope scope in scopes) {
            request.Scopes.Add(scope.GetServiceScope());
        }

        var response = await authnClient.CreateGroupAsync(request, requestHeaders).ConfigureAwait(false);

        return new CreateGroupResponse(response.GroupId);
    }

    public async Task AddUserToGroup(string userId, string groupId) {
        await RefreshToken().ConfigureAwait(false);

        await authnClient.AddUserToGroupAsync(new Protobuf.AddUserToGroupRequest{ UserId = userId, GroupId = groupId }, requestHeaders)
            .ConfigureAwait(false);
    }

    public async Task RemoveUserFromGroup(string userId, string groupId) {
        await RefreshToken().ConfigureAwait(false);

        await authnClient.RemoveUserFromGroupAsync(new Protobuf.RemoveUserFromGroupRequest{ UserId = userId, GroupId = groupId },
            requestHeaders).ConfigureAwait(false);
    }

    /////////////////////////////////////////////////////////////////////////
    //                              Encryption                             //
    /////////////////////////////////////////////////////////////////////////

    public async Task<EncryptResponse> Encrypt(byte[] plaintext, byte[] associatedData) {
        await RefreshToken().ConfigureAwait(false);

        var response = await eaasClient.EncryptAsync(new Protobuf.EncryptRequest{ Plaintext = ByteString.CopyFrom(plaintext),
            AssociatedData = ByteString.CopyFrom(associatedData) }, requestHeaders).ConfigureAwait(false);
        
        return new EncryptResponse(response.ObjectId, response.Ciphertext.ToByteArray(), response.AssociatedData.ToByteArray());
    }

    public async Task<DecryptResponse> Decrypt(string objectId, byte[] ciphertext, byte[] associatedData) {
        await RefreshToken().ConfigureAwait(false);

        var response = await eaasClient.DecryptAsync(new Protobuf.DecryptRequest{ ObjectId = objectId,
            Ciphertext = ByteString.CopyFrom(ciphertext), AssociatedData = ByteString.CopyFrom(associatedData) }, requestHeaders)
            .ConfigureAwait(false);
        
        return new DecryptResponse(response.Plaintext.ToByteArray(), response.AssociatedData.ToByteArray());
    }

    /////////////////////////////////////////////////////////////////////////
    //                               Storage                               //
    /////////////////////////////////////////////////////////////////////////

    public async Task<StoreResponse> Store(byte[] plaintext, byte[] associatedData) {
        await RefreshToken().ConfigureAwait(false);

        var response = await objectsClient.StoreAsync(new Protobuf.StoreRequest{ Plaintext = ByteString.CopyFrom(plaintext),
            AssociatedData = ByteString.CopyFrom(associatedData) }, requestHeaders).ConfigureAwait(false);
        
        return new StoreResponse(response.ObjectId);
    }

    public async Task<RetrieveResponse> Retrieve(string objectId) {
        await RefreshToken().ConfigureAwait(false);

        var response = await objectsClient.RetrieveAsync(new Protobuf.RetrieveRequest{ ObjectId = objectId }, requestHeaders).ConfigureAwait(false);

        return new RetrieveResponse(response.Plaintext.ToByteArray(), response.AssociatedData.ToByteArray());
    }

    public async Task Update(string objectId, byte[] plaintext, byte[] associatedData) {
        await RefreshToken().ConfigureAwait(false);

        await objectsClient.UpdateAsync(new Protobuf.UpdateRequest{ ObjectId = objectId, Plaintext = ByteString.CopyFrom(plaintext),
            AssociatedData = ByteString.CopyFrom(associatedData) }, requestHeaders).ConfigureAwait(false);
    }

    public async Task Delete(string objectId) {
        await RefreshToken().ConfigureAwait(false);

        await objectsClient.DeleteAsync(new Protobuf.DeleteRequest{ ObjectId = objectId }, requestHeaders).ConfigureAwait(false);
    }

    /////////////////////////////////////////////////////////////////////////
    //                             Permissions                             //
    /////////////////////////////////////////////////////////////////////////

    public async Task<GetPermissionsResponse> GetPermissions(string objectId) {
        await RefreshToken().ConfigureAwait(false);

        var response = await authzClient.GetPermissionsAsync(new Protobuf.GetPermissionsRequest{ ObjectId = objectId }, requestHeaders)
            .ConfigureAwait(false);
        
        return new GetPermissionsResponse(new List<string>(response.GroupIds));
    }

    public async Task AddPermission(string objectId, string groupId) {
        await RefreshToken().ConfigureAwait(false);

        await authzClient.AddPermissionAsync(new Protobuf.AddPermissionRequest{ ObjectId = objectId, GroupId = groupId }, requestHeaders)
            .ConfigureAwait(false);
    }

    public async Task RemovePermission(string objectId, string groupId) {
        await RefreshToken().ConfigureAwait(false);

        await authzClient.RemovePermissionAsync(new Protobuf.RemovePermissionRequest{ ObjectId = objectId, GroupId = groupId }, requestHeaders)
            .ConfigureAwait(false);
    }
}
