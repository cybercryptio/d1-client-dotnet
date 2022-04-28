// Copyright 2020-2022 CYBERCRYPT
using System;
using System.Collections.Generic;
using Xunit;
using Encryptonize.Client.Utils;

namespace Encryptonize.Client.Tests;

public class EncryptonizeClientTest {
    private string encryptonizeUser;
    private string encryptonizePassword;
    private string encryptonizeClientCert;
    private string encryptonizeEndpoint;
    private List<Scope> allScopes = new List<Scope>{Scope.Read, Scope.Create, Scope.Index, Scope.ObjectPermissions, Scope.UserManagement,
    Scope.Update, Scope.Delete};

    public EncryptonizeClientTest() {
        encryptonizeUser = Environment.GetEnvironmentVariable("E2E_TEST_UID") ?? throw new ArgumentNullException("E2E_TEST_UID must be set");
        encryptonizePassword = Environment.GetEnvironmentVariable("E2E_TEST_PASS") ?? throw new ArgumentNullException("E2E_TEST_PASS must be set");
        encryptonizeClientCert = Environment.GetEnvironmentVariable("E2E_TEST_CERT") ?? "";
        encryptonizeEndpoint = Environment.GetEnvironmentVariable("E2E_TEST_URL") ?? "http://127.0.0.1:9000";
    }

    [Fact]
    public async void TestClientConnectionAsync() {
        var client = new EncryptonizeClient(encryptonizeEndpoint, encryptonizeUser, encryptonizePassword, encryptonizeClientCert);

        await client.VersionAsync().ConfigureAwait(false);

        await client.DisposeAsync();
    }

    [Fact]
    public async void TestUserManagementAsync() {
        var client = new EncryptonizeClient(encryptonizeEndpoint, encryptonizeUser, encryptonizePassword, encryptonizeClientCert);

        var createUserResponse = await client.CreateUserAsync(allScopes).ConfigureAwait(false);
        var createGroupResponse = await client.CreateGroupAsync(allScopes).ConfigureAwait(false);

        await client.AddUserToGroupAsync(createUserResponse.UserId, createGroupResponse.GroupId).ConfigureAwait(false);
        await client.RemoveUserFromGroupAsync(createUserResponse.UserId, createGroupResponse.GroupId).ConfigureAwait(false);

        await client.RemoveUserAsync(createUserResponse.UserId).ConfigureAwait(false);

        await client.DisposeAsync();
    }

    [Fact]
    [Trait("Category", "EAAS")]
    public async void TestEncryptionAsync() {
        var plaintext = System.Text.Encoding.ASCII.GetBytes("plaintext");
        var associatedData = System.Text.Encoding.ASCII.GetBytes("associatedData");

        var client = new EncryptonizeClient(encryptonizeEndpoint, encryptonizeUser, encryptonizePassword, encryptonizeClientCert);

        var createUserResponse = await client.CreateUserAsync(allScopes).ConfigureAwait(false);
        await client.LoginAsync(createUserResponse.UserId, createUserResponse.Password).ConfigureAwait(false);

        var encryptResponse = await client.EncryptAsync(plaintext, associatedData).ConfigureAwait(false);
        var decryptResponse = await client.DecryptAsync(encryptResponse.ObjectId, encryptResponse.Ciphertext, encryptResponse.AssociatedData)
            .ConfigureAwait(false);
        Assert.Equal(plaintext, decryptResponse.Plaintext);
        Assert.Equal(associatedData, decryptResponse.AssociatedData);

        await client.DisposeAsync();
    }

    [Fact]
    [Trait("Category", "Objects")]
    public async void TestStoreAsync() {
        var plaintext = System.Text.Encoding.ASCII.GetBytes("plaintext");
        var associatedData = System.Text.Encoding.ASCII.GetBytes("associatedData");
        var updatedPlaintext = System.Text.Encoding.ASCII.GetBytes("updatedPlaintext");
        var updatedAssociatedData = System.Text.Encoding.ASCII.GetBytes("updatedAssociatedData");

        var client = new EncryptonizeClient(encryptonizeEndpoint, encryptonizeUser, encryptonizePassword, encryptonizeClientCert);

        var createUserResponse = await client.CreateUserAsync(allScopes).ConfigureAwait(false);
        await client.LoginAsync(createUserResponse.UserId, createUserResponse.Password).ConfigureAwait(false);

        var storeResponse = await client.StoreAsync(plaintext, associatedData).ConfigureAwait(false);
        var retrieveResponse = await client.RetrieveAsync(storeResponse.ObjectId).ConfigureAwait(false);
        Assert.Equal(plaintext, retrieveResponse.Plaintext);
        Assert.Equal(associatedData, retrieveResponse.AssociatedData);

        await client.UpdateAsync(storeResponse.ObjectId, updatedPlaintext, updatedAssociatedData).ConfigureAwait(false);
        var retrieveResponseAfterUpdate = await client.RetrieveAsync(storeResponse.ObjectId).ConfigureAwait(false);
        Assert.Equal(updatedPlaintext, retrieveResponseAfterUpdate.Plaintext);
        Assert.Equal(updatedAssociatedData, retrieveResponseAfterUpdate.AssociatedData);

        await client.DeleteAsync(storeResponse.ObjectId).ConfigureAwait(false);
        var e = await Assert.ThrowsAsync<Grpc.Core.RpcException>(async() => await client.RetrieveAsync(storeResponse.ObjectId).ConfigureAwait(false))
            .ConfigureAwait(false);
        Assert.Equal(Grpc.Core.StatusCode.NotFound, e.StatusCode);

        await client.DisposeAsync();
    }

    [Fact]
    [Trait("Category", "Objects")]
    public async void TestPermissionsAsync() {
        var plaintext = System.Text.Encoding.ASCII.GetBytes("plaintext");
        var associatedData = System.Text.Encoding.ASCII.GetBytes("associatedData");

        var client = new EncryptonizeClient(encryptonizeEndpoint, encryptonizeUser, encryptonizePassword, encryptonizeClientCert);

        var createUserResponse = await client.CreateUserAsync(allScopes).ConfigureAwait(false);
        await client.LoginAsync(createUserResponse.UserId, createUserResponse.Password).ConfigureAwait(false);

        var storeResponse = await client.StoreAsync(plaintext, associatedData).ConfigureAwait(false);

        await client.AddPermissionAsync(storeResponse.ObjectId, createUserResponse.UserId).ConfigureAwait(false);
        var getPermissionsResponse = await client.GetPermissionsAsync(storeResponse.ObjectId).ConfigureAwait(false);
        Assert.Contains(createUserResponse.UserId, getPermissionsResponse.GroupIds);

        await client.RemovePermissionAsync(storeResponse.ObjectId, createUserResponse.UserId).ConfigureAwait(false);
        var e = await Assert.ThrowsAsync<Grpc.Core.RpcException>(async() => await client.GetPermissionsAsync(storeResponse.ObjectId).ConfigureAwait(false))
            .ConfigureAwait(false);
        Assert.Equal(Grpc.Core.StatusCode.PermissionDenied, e.StatusCode);

        await client.DisposeAsync();
    }

    [Fact]
    public async void TestClientRefreshTokenAsync() {
        var client = new EncryptonizeClient(encryptonizeEndpoint, encryptonizeUser, encryptonizePassword, encryptonizeClientCert);
        
        var initialAccessToken = client.accessToken;

        client.ExpiryTime = DateTime.Now;

        await client.VersionAsync().ConfigureAwait(false);

        Assert.NotEqual(client.accessToken, initialAccessToken);
        Assert.False(string.IsNullOrWhiteSpace(client.accessToken));

        await client.DisposeAsync();
    }

    [Fact]
    public void TestClientConnection() {
        var client = new EncryptonizeClient(encryptonizeEndpoint, encryptonizeUser, encryptonizePassword, encryptonizeClientCert);

        client.Version();

        client.Dispose();
    }

    [Fact]
    public void TestUserManagement() {
        var client = new EncryptonizeClient(encryptonizeEndpoint, encryptonizeUser, encryptonizePassword, encryptonizeClientCert);

        var createUserResponse = client.CreateUser(allScopes);
        var createGroupResponse = client.CreateGroup(allScopes);

        client.AddUserToGroup(createUserResponse.UserId, createGroupResponse.GroupId);
        client.RemoveUserFromGroup(createUserResponse.UserId, createGroupResponse.GroupId);

        client.RemoveUser(createUserResponse.UserId);

        client.Dispose();
    }

    [Fact]
    [Trait("Category", "EAAS")]
    public void TestEncryption() {
        var plaintext = System.Text.Encoding.ASCII.GetBytes("plaintext");
        var associatedData = System.Text.Encoding.ASCII.GetBytes("associatedData");

        var client = new EncryptonizeClient(encryptonizeEndpoint, encryptonizeUser, encryptonizePassword, encryptonizeClientCert);

        var createUserResponse = client.CreateUser(allScopes);
        client.Login(createUserResponse.UserId, createUserResponse.Password);

        var encryptResponse = client.Encrypt(plaintext, associatedData);
        var decryptResponse = client.Decrypt(encryptResponse.ObjectId, encryptResponse.Ciphertext, encryptResponse.AssociatedData);
        Assert.Equal(plaintext, decryptResponse.Plaintext);
        Assert.Equal(associatedData, decryptResponse.AssociatedData);

        client.Dispose();
    }

    [Fact]
    [Trait("Category", "Objects")]
    public void TestStore() {
        var plaintext = System.Text.Encoding.ASCII.GetBytes("plaintext");
        var associatedData = System.Text.Encoding.ASCII.GetBytes("associatedData");
        var updatedPlaintext = System.Text.Encoding.ASCII.GetBytes("updatedPlaintext");
        var updatedAssociatedData = System.Text.Encoding.ASCII.GetBytes("updatedAssociatedData");

        var client = new EncryptonizeClient(encryptonizeEndpoint, encryptonizeUser, encryptonizePassword, encryptonizeClientCert);

        var createUserResponse = client.CreateUser(allScopes);
        client.Login(createUserResponse.UserId, createUserResponse.Password);

        var storeResponse = client.Store(plaintext, associatedData);
        var retrieveResponse = client.Retrieve(storeResponse.ObjectId);
        Assert.Equal(plaintext, retrieveResponse.Plaintext);
        Assert.Equal(associatedData, retrieveResponse.AssociatedData);

        client.Update(storeResponse.ObjectId, updatedPlaintext, updatedAssociatedData);
        var retrieveResponseAfterUpdate = client.Retrieve(storeResponse.ObjectId);
        Assert.Equal(updatedPlaintext, retrieveResponseAfterUpdate.Plaintext);
        Assert.Equal(updatedAssociatedData, retrieveResponseAfterUpdate.AssociatedData);

        client.Delete(storeResponse.ObjectId);
        var e = Assert.Throws<Grpc.Core.RpcException>(() => client.Retrieve(storeResponse.ObjectId));
        Assert.Equal(Grpc.Core.StatusCode.NotFound, e.StatusCode);

        client.Dispose();
    }

    [Fact]
    [Trait("Category", "Objects")]
    public void TestPermissions() {
        var plaintext = System.Text.Encoding.ASCII.GetBytes("plaintext");
        var associatedData = System.Text.Encoding.ASCII.GetBytes("associatedData");

        var client = new EncryptonizeClient(encryptonizeEndpoint, encryptonizeUser, encryptonizePassword, encryptonizeClientCert);

        var createUserResponse = client.CreateUser(allScopes);
        client.Login(createUserResponse.UserId, createUserResponse.Password);

        var storeResponse = client.Store(plaintext, associatedData);

        client.AddPermission(storeResponse.ObjectId, createUserResponse.UserId);
        var getPermissionsResponse = client.GetPermissions(storeResponse.ObjectId);
        Assert.Contains(createUserResponse.UserId, getPermissionsResponse.GroupIds);

        client.RemovePermission(storeResponse.ObjectId, createUserResponse.UserId);
        var e = Assert.Throws<Grpc.Core.RpcException>(() => client.GetPermissions(storeResponse.ObjectId));
        Assert.Equal(Grpc.Core.StatusCode.PermissionDenied, e.StatusCode);

        client.Dispose();
    }

    [Fact]
    public void TestClientRefreshToken() {
        var client = new EncryptonizeClient(encryptonizeEndpoint, encryptonizeUser, encryptonizePassword, encryptonizeClientCert);
        
        var initialAccessToken = client.accessToken;

        client.ExpiryTime = DateTime.Now;

        client.Version();

        Assert.NotEqual(client.accessToken, initialAccessToken);
        Assert.False(string.IsNullOrWhiteSpace(client.accessToken));

        client.Dispose();
    }
}
