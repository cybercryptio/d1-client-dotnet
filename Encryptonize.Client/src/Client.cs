// Copyright 2020-2022 CYBERCRYPT
using System.Security.Cryptography.X509Certificates;
using Grpc.Net.Client;
using Grpc.Core;
using Google.Protobuf;
using Encryptonize.Client.Utils;
using Encryptonize.Client.Response;

namespace Encryptonize.Client;

public class Client {
    private string? accessToken;
    public string? AccessToken { get { return accessToken; } }
    private long expiryTime;
    public long ExpiryTime { get { return expiryTime; } }
    private Metadata requestHeaders = new Metadata();
    private GrpcChannel channel;
    private App.Encryptonize.EncryptonizeClient appClient;
    private Authn.Encryptonize.EncryptonizeClient authnClient;
    private Enc.Encryptonize.EncryptonizeClient encClient;
    private Storage.Encryptonize.EncryptonizeClient storageClient;
    private Authz.Encryptonize.EncryptonizeClient authzClient;

    public Client(string endpoint, string certPath) {
        if (certPath.Equals("")) {
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

    public void CloseConnection() {
        channel.ShutdownAsync().Wait();
    }

    /////////////////////////////////////////////////////////////////////////
    //                               Utility                               //
    /////////////////////////////////////////////////////////////////////////

    public VersionResponse Version() {
        var response = appClient.Version(new App.VersionRequest(), requestHeaders);
        return new VersionResponse(response.Commit, response.Tag);
    }

    /////////////////////////////////////////////////////////////////////////
    //                           User Management                           //
    /////////////////////////////////////////////////////////////////////////

    public void Login(string user, string password) {
        var response = authnClient.LoginUser(new Authn.LoginUserRequest{ UserId = user, Password = password });

        accessToken = response.AccessToken;
        expiryTime = response.ExpiryTime;

        requestHeaders = new Metadata();
        requestHeaders.Add("Authorization", $"Bearer {accessToken}");
    }

    public CreateUserResponse CreateUser(IList<Scope> scopes) {
        var request = new Authn.CreateUserRequest();
        foreach (Scope scope in scopes) {
            request.Scopes.Add(scope.GetServiceScope());
        }

        var response = authnClient.CreateUser(request, requestHeaders);

        return new CreateUserResponse(response.UserId, response.Password);
    }

    public void RemoveUser(string userId) {
        authnClient.RemoveUser(new Authn.RemoveUserRequest{ UserId = userId }, requestHeaders);
    }

    public CreateGroupResponse CreateGroup(IList<Scope> scopes) {
        var request = new Authn.CreateGroupRequest();
        foreach (Scope scope in scopes) {
            request.Scopes.Add(scope.GetServiceScope());
        }

        var response = authnClient.CreateGroup(request, requestHeaders);

        return new CreateGroupResponse(response.GroupId);
    }

    public void AddUserToGroup(string userId, string groupId) {
        authnClient.AddUserToGroup(new Authn.AddUserToGroupRequest{ UserId = userId, GroupId = groupId }, requestHeaders);
    }

    public void RemoveUserFromGroup(string userId, string groupId) {
        authnClient.RemoveUserFromGroup(new Authn.RemoveUserFromGroupRequest{ UserId = userId, GroupId = groupId },
            requestHeaders);
    }

    /////////////////////////////////////////////////////////////////////////
    //                              Encryption                             //
    /////////////////////////////////////////////////////////////////////////

    public EncryptResponse Encrypt(byte[] plaintext, byte[] associatedData) {
        var response = encClient.Encrypt(new Enc.EncryptRequest{ Plaintext = ByteString.CopyFrom(plaintext),
            AssociatedData = ByteString.CopyFrom(associatedData) }, requestHeaders);
        return new EncryptResponse(response.ObjectId, response.Ciphertext.ToByteArray(), response.AssociatedData.ToByteArray());
    }

    public DecryptResponse Decrypt(string objectId, byte[] ciphertext, byte[] associatedData) {
        var response = encClient.Decrypt(new Enc.DecryptRequest{ ObjectId = objectId,
            Ciphertext = ByteString.CopyFrom(ciphertext), AssociatedData = ByteString.CopyFrom(associatedData) }, requestHeaders);
        return new DecryptResponse(response.Plaintext.ToByteArray(), response.AssociatedData.ToByteArray());
    }

    /////////////////////////////////////////////////////////////////////////
    //                               Storage                               //
    /////////////////////////////////////////////////////////////////////////

    public StoreResponse Store(byte[] plaintext, byte[] associatedData) {
        var response = storageClient.Store(new Storage.StoreRequest{ Plaintext = ByteString.CopyFrom(plaintext),
            AssociatedData = ByteString.CopyFrom(associatedData) }, requestHeaders);
        return new StoreResponse(response.ObjectId);
    }

    public RetrieveResponse Retrieve(string objectId) {
        var response = storageClient.Retrieve(new Storage.RetrieveRequest{ ObjectId = objectId }, requestHeaders);
        return new RetrieveResponse(response.Plaintext.ToByteArray(), response.AssociatedData.ToByteArray());
    }

    public void Update(string objectId, byte[] plaintext, byte[] associatedData) {
        storageClient.Update(new Storage.UpdateRequest{ ObjectId = objectId, Plaintext = ByteString.CopyFrom(plaintext),
            AssociatedData = ByteString.CopyFrom(associatedData) }, requestHeaders);
    }

    public void Delete(string objectId) {
        storageClient.Delete(new Storage.DeleteRequest{ ObjectId = objectId }, requestHeaders);
    }

    /////////////////////////////////////////////////////////////////////////
    //                             Permissions                             //
    /////////////////////////////////////////////////////////////////////////

    public GetPermissionsResponse GetPermissions(string objectId) {
        var response = authzClient.GetPermissions(new Authz.GetPermissionsRequest{ ObjectId = objectId }, requestHeaders);
        return new GetPermissionsResponse(new List<string>(response.GroupIds));
    }

    public void AddPermission(string objectId, string groupId) {
        authzClient.AddPermission(new Authz.AddPermissionRequest{ ObjectId = objectId, GroupId = groupId }, requestHeaders);
    }

    public void RemovePermission(string objectId, string groupId) {
        authzClient.RemovePermission(new Authz.RemovePermissionRequest{ ObjectId = objectId, GroupId = groupId }, requestHeaders);
    }
}
