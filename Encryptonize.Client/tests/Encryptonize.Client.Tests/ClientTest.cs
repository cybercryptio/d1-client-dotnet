// Copyright 2020-2022 CYBERCRYPT
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Encryptonize.Client.Utils;

namespace Encryptonize.Client.Tests;

public class ClientTest : IDisposable, IAsyncDisposable {
    private string encryptonizeUser;
    private string encryptonizePassword;
    private string encryptonizeClientCert;
    private string encryptonizeEndpoint;
    private Client client;
    private List<Scope> allScopes = new List<Scope>{Scope.Read, Scope.Create, Scope.Index, Scope.ObjectPermissions, Scope.UserManagement,
    Scope.Update, Scope.Delete};

    public ClientTest() {
        encryptonizeUser = Environment.GetEnvironmentVariable("E2E_TEST_UID") ?? throw new ArgumentNullException("E2E_TEST_UID must be set");
        encryptonizePassword = Environment.GetEnvironmentVariable("E2E_TEST_PASS") ?? throw new ArgumentNullException("E2E_TEST_PASS must be set");
        encryptonizeClientCert = Environment.GetEnvironmentVariable("E2E_TEST_CERT") ?? "";
        encryptonizeEndpoint = Environment.GetEnvironmentVariable("E2E_TEST_URL") ?? "http://127.0.0.1:9000";
        client = new Client(encryptonizeEndpoint, encryptonizeClientCert);
    }

    public void Dispose() {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing) {
        if (disposing) {
            client.Dispose();
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
        await client.DisposeAsync().ConfigureAwait(false);
    }

    [Fact]
    public async void TestClientConnection() {
        await client.Login(encryptonizeUser, encryptonizePassword).ConfigureAwait(false);
        await client.Version().ConfigureAwait(false);
    }

    [Fact]
    public async void TestUserManagement() {
        await client.Login(encryptonizeUser, encryptonizePassword).ConfigureAwait(false);
        var createUserResponse = await client.CreateUser(allScopes).ConfigureAwait(false);
        var createGroupResponse = await client.CreateGroup(allScopes).ConfigureAwait(false);
        await client.AddUserToGroup(createUserResponse.UserId, createGroupResponse.GroupId).ConfigureAwait(false);
        await client.RemoveUserFromGroup(createUserResponse.UserId, createGroupResponse.GroupId).ConfigureAwait(false);
        await client.RemoveUser(createUserResponse.UserId).ConfigureAwait(false);
    }

    [Fact]
    public async void TestEncryption() {
        var plaintext = System.Text.Encoding.ASCII.GetBytes("plaintext");
        var associatedData = System.Text.Encoding.ASCII.GetBytes("associatedData");

        await client.Login(encryptonizeUser, encryptonizePassword).ConfigureAwait(false);

        var createUserResponse = await client.CreateUser(allScopes).ConfigureAwait(false);
        await client.Login(createUserResponse.UserId, createUserResponse.Password).ConfigureAwait(false);

        var encryptResponse = await client.Encrypt(plaintext, associatedData).ConfigureAwait(false);
        var decryptResponse = await client.Decrypt(encryptResponse.ObjectId, encryptResponse.Ciphertext, encryptResponse.AssociatedData)
            .ConfigureAwait(false);
        Assert.Equal(plaintext, decryptResponse.Plaintext);
        Assert.Equal(associatedData, decryptResponse.AssociatedData);
    }

    [Fact]
    public async void TestStore() {
        var plaintext = System.Text.Encoding.ASCII.GetBytes("plaintext");
        var associatedData = System.Text.Encoding.ASCII.GetBytes("associatedData");
        var updatedPlaintext = System.Text.Encoding.ASCII.GetBytes("updatedPlaintext");
        var updatedAssociatedData = System.Text.Encoding.ASCII.GetBytes("updatedAssociatedData");

        await client.Login(encryptonizeUser, encryptonizePassword).ConfigureAwait(false);

        var createUserResponse = await client.CreateUser(allScopes).ConfigureAwait(false);
        await client.Login(createUserResponse.UserId, createUserResponse.Password).ConfigureAwait(false);

        var storeResponse = await client.Store(plaintext, associatedData).ConfigureAwait(false);
        var retrieveResponse = await client.Retrieve(storeResponse.ObjectId).ConfigureAwait(false);
        Assert.Equal(plaintext, retrieveResponse.Plaintext);
        Assert.Equal(associatedData, retrieveResponse.AssociatedData);

        await client.Update(storeResponse.ObjectId, updatedPlaintext, updatedAssociatedData).ConfigureAwait(false);
        var retrieveResponseAfterUpdate = await client.Retrieve(storeResponse.ObjectId).ConfigureAwait(false);
        Assert.Equal(updatedPlaintext, retrieveResponseAfterUpdate.Plaintext);
        Assert.Equal(updatedAssociatedData, retrieveResponseAfterUpdate.AssociatedData);

        await client.Delete(storeResponse.ObjectId).ConfigureAwait(false);
        var e = await Assert.ThrowsAsync<Grpc.Core.RpcException>(async() => await client.Retrieve(storeResponse.ObjectId).ConfigureAwait(false))
            .ConfigureAwait(false);
        Assert.Equal(Grpc.Core.StatusCode.NotFound, e.StatusCode);
    }

    [Fact]
    public async void TestPermissions() {
        var plaintext = System.Text.Encoding.ASCII.GetBytes("plaintext");
        var associatedData = System.Text.Encoding.ASCII.GetBytes("associatedData");

        await client.Login(encryptonizeUser, encryptonizePassword).ConfigureAwait(false);

        var createUserResponse = await client.CreateUser(allScopes).ConfigureAwait(false);
        await client.Login(createUserResponse.UserId, createUserResponse.Password).ConfigureAwait(false);

        var storeResponse = await client.Store(plaintext, associatedData).ConfigureAwait(false);

        await client.AddPermission(storeResponse.ObjectId, createUserResponse.UserId).ConfigureAwait(false);
        var getPermissionsResponse = await client.GetPermissions(storeResponse.ObjectId).ConfigureAwait(false);
        Assert.Contains(createUserResponse.UserId, getPermissionsResponse.GroupIds);

        await client.RemovePermission(storeResponse.ObjectId, createUserResponse.UserId).ConfigureAwait(false);
        var e = await Assert.ThrowsAsync<Grpc.Core.RpcException>(async() => await client.GetPermissions(storeResponse.ObjectId).ConfigureAwait(false))
            .ConfigureAwait(false);
        Assert.Equal(Grpc.Core.StatusCode.PermissionDenied, e.StatusCode);
    }

    [Fact]
    public async void TestClientRefreshToken() {
        var client = await ClientRT.New(encryptonizeEndpoint, encryptonizeUser, encryptonizePassword, encryptonizeClientCert);
        
        var initialAccessToken = client.AccessToken;

        client.ExpiryTime = DateTime.Now;

        await client.Version().ConfigureAwait(false);

        Assert.NotEqual(client.AccessToken, initialAccessToken);
        Assert.False(string.IsNullOrWhiteSpace(client.AccessToken));

        client.Dispose();
    }
}
