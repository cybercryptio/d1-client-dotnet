// Copyright 2020-2022 CYBERCRYPT
using System;
using System.Collections.Generic;
using Xunit;
using CyberCrypt.D1.Client.Utils;
using CyberCrypt.D1.Client.Credentials;

namespace CyberCrypt.D1.Client.Tests;

public class D1ClientTest
{
    private string d1User;
    private string d1Password;
    private string d1Endpoint;
    private readonly UsernamePasswordCredentials credentials;
    private List<Scope> allScopes = new List<Scope>{Scope.Read, Scope.Create, Scope.GetAccess, Scope.ModifyAccess,
    Scope.Update, Scope.Delete, Scope.Index};

    public D1ClientTest()
    {
        d1User = Environment.GetEnvironmentVariable("E2E_TEST_UID") ?? throw new ArgumentNullException("E2E_TEST_UID must be set");
        d1Password = Environment.GetEnvironmentVariable("E2E_TEST_PASS") ?? throw new ArgumentNullException("E2E_TEST_PASS must be set");
        d1Endpoint = Environment.GetEnvironmentVariable("E2E_TEST_URL") ?? "http://127.0.0.1:9000";
        credentials = new UsernamePasswordCredentials(d1Endpoint, d1User, d1Password);
    }

    [Fact]
    public async void TestClientConnectionAsync()
    {
        var client = new D1GenericClient(d1Endpoint, credentials);

        await client.Version.VersionAsync().ConfigureAwait(false);

        await client.DisposeAsync();
    }

    [Fact]
    public async void TestUserManagementAsync()
    {
        var client = new D1GenericClient(d1Endpoint, credentials);

        var createUserResponse = await client.Authn.CreateUserAsync(allScopes).ConfigureAwait(false);
        var createGroupResponse = await client.Authn.CreateGroupAsync(allScopes).ConfigureAwait(false);

        await client.Authn.AddUserToGroupAsync(createUserResponse.UserId, createGroupResponse.GroupId).ConfigureAwait(false);
        await client.Authn.RemoveUserFromGroupAsync(createUserResponse.UserId, createGroupResponse.GroupId).ConfigureAwait(false);

        await client.Authn.RemoveUserAsync(createUserResponse.UserId).ConfigureAwait(false);

        await client.DisposeAsync();
    }

    [Fact]
    [Trait("Category", "Generic")]
    public async void TestEncryptionAsync()
    {
        var plaintext = System.Text.Encoding.ASCII.GetBytes("plaintext");
        var associatedData = System.Text.Encoding.ASCII.GetBytes("associatedData");

        var client = new D1GenericClient(d1Endpoint, credentials);

        var createUserResponse = await client.Authn.CreateUserAsync(allScopes).ConfigureAwait(false);
        using var client2 = new D1GenericClient(d1Endpoint, new UsernamePasswordCredentials(d1Endpoint, createUserResponse.UserId, createUserResponse.Password));

        var encryptResponse = await client2.Generic.EncryptAsync(plaintext, associatedData).ConfigureAwait(false);
        var decryptResponse = await client2.Generic.DecryptAsync(encryptResponse.ObjectId, encryptResponse.Ciphertext, encryptResponse.AssociatedData)
            .ConfigureAwait(false);
        Assert.Equal(plaintext, decryptResponse.Plaintext);
        Assert.Equal(associatedData, decryptResponse.AssociatedData);

        await client.DisposeAsync();
    }

    [Fact]
    [Trait("Category", "Storage")]
    public async void TestStoreAsync()
    {
        var plaintext = System.Text.Encoding.ASCII.GetBytes("plaintext");
        var associatedData = System.Text.Encoding.ASCII.GetBytes("associatedData");
        var updatedPlaintext = System.Text.Encoding.ASCII.GetBytes("updatedPlaintext");
        var updatedAssociatedData = System.Text.Encoding.ASCII.GetBytes("updatedAssociatedData");

        var client = new D1StorageClient(d1Endpoint, credentials);

        var createUserResponse = await client.Authn.CreateUserAsync(allScopes).ConfigureAwait(false);
        using var client2 = new D1StorageClient(d1Endpoint, new UsernamePasswordCredentials(d1Endpoint, createUserResponse.UserId, createUserResponse.Password));

        var storeResponse = await client2.Storage.StoreAsync(plaintext, associatedData).ConfigureAwait(false);
        var retrieveResponse = await client2.Storage.RetrieveAsync(storeResponse.ObjectId).ConfigureAwait(false);
        Assert.Equal(plaintext, retrieveResponse.Plaintext);
        Assert.Equal(associatedData, retrieveResponse.AssociatedData);

        await client2.Storage.UpdateAsync(storeResponse.ObjectId, updatedPlaintext, updatedAssociatedData).ConfigureAwait(false);
        var retrieveResponseAfterUpdate = await client2.Storage.RetrieveAsync(storeResponse.ObjectId).ConfigureAwait(false);
        Assert.Equal(updatedPlaintext, retrieveResponseAfterUpdate.Plaintext);
        Assert.Equal(updatedAssociatedData, retrieveResponseAfterUpdate.AssociatedData);

        await client2.Storage.DeleteAsync(storeResponse.ObjectId).ConfigureAwait(false);
        var e = await Assert.ThrowsAsync<Grpc.Core.RpcException>(async () => await client2.Storage.RetrieveAsync(storeResponse.ObjectId).ConfigureAwait(false))
            .ConfigureAwait(false);
        Assert.Equal(Grpc.Core.StatusCode.NotFound, e.StatusCode);

        await client.DisposeAsync();
    }

    [Fact]
    [Trait("Category", "Storage")]
    public async void TestPermissionsAsync()
    {
        var plaintext = System.Text.Encoding.ASCII.GetBytes("plaintext");
        var associatedData = System.Text.Encoding.ASCII.GetBytes("associatedData");

        var client = new D1StorageClient(d1Endpoint, credentials);

        var createUserResponse = await client.Authn.CreateUserAsync(allScopes).ConfigureAwait(false);
        using var client2 = new D1StorageClient(d1Endpoint, new UsernamePasswordCredentials(d1Endpoint, createUserResponse.UserId, createUserResponse.Password));

        var storeResponse = await client2.Storage.StoreAsync(plaintext, associatedData).ConfigureAwait(false);

        await client2.Authz.AddPermissionAsync(storeResponse.ObjectId, createUserResponse.UserId).ConfigureAwait(false);
        var getPermissionsResponse = await client2.Authz.GetPermissionsAsync(storeResponse.ObjectId).ConfigureAwait(false);
        Assert.Contains(createUserResponse.UserId, getPermissionsResponse.GroupIds);

        await client2.Authz.RemovePermissionAsync(storeResponse.ObjectId, createUserResponse.UserId).ConfigureAwait(false);
        var e = await Assert.ThrowsAsync<Grpc.Core.RpcException>(async () => await client2.Authz.GetPermissionsAsync(storeResponse.ObjectId).ConfigureAwait(false))
            .ConfigureAwait(false);
        Assert.Equal(Grpc.Core.StatusCode.PermissionDenied, e.StatusCode);

        await client.DisposeAsync();
    }

    [Fact]
    public async void TestClientRefreshTokenAsync()
    {
        var client = new D1GenericClient(d1Endpoint, credentials);

        credentials.ExpiryTime = DateTime.Now;

        await client.Version.VersionAsync().ConfigureAwait(false);

        await client.DisposeAsync();
    }

    [Fact]
    public void TestClientConnection()
    {
        var client = new D1GenericClient(d1Endpoint, credentials);

        client.Version.Version();

        client.Dispose();
    }

    [Fact]
    public void TestUserManagement()
    {
        var client = new D1GenericClient(d1Endpoint, credentials);

        var createUserResponse = client.Authn.CreateUser(allScopes);
        var createGroupResponse = client.Authn.CreateGroup(allScopes);

        client.Authn.AddUserToGroup(createUserResponse.UserId, createGroupResponse.GroupId);
        client.Authn.RemoveUserFromGroup(createUserResponse.UserId, createGroupResponse.GroupId);

        client.Authn.RemoveUser(createUserResponse.UserId);

        client.Dispose();
    }

    [Fact]
    [Trait("Category", "Generic")]
    public void TestEncryption()
    {
        var plaintext = System.Text.Encoding.ASCII.GetBytes("plaintext");
        var associatedData = System.Text.Encoding.ASCII.GetBytes("associatedData");

        var client = new D1GenericClient(d1Endpoint, credentials);

        var createUserResponse = client.Authn.CreateUser(allScopes);
        using var client2 = new D1GenericClient(d1Endpoint, new UsernamePasswordCredentials(d1Endpoint, createUserResponse.UserId, createUserResponse.Password));

        var encryptResponse = client2.Generic.Encrypt(plaintext, associatedData);
        var decryptResponse = client2.Generic.Decrypt(encryptResponse.ObjectId, encryptResponse.Ciphertext, encryptResponse.AssociatedData);
        Assert.Equal(plaintext, decryptResponse.Plaintext);
        Assert.Equal(associatedData, decryptResponse.AssociatedData);

        client.Dispose();
    }

    [Fact]
    [Trait("Category", "Generic")]
    public void TestAddSearch()
    {
        string[] keywords = { "keyword1", "keyword2", "keyword3" };
        var identifer = "id1";

        var client = new D1GenericClient(d1Endpoint, d1ClientOptions);

        var createUserResponse = client.CreateUser(allScopes);
        client.Login(createUserResponse.UserId, createUserResponse.Password);

        var addResponse = client.Add(keywords, identifer);
        var searchResponse = client.Search(keywords[0]);
        Assert.Equal(identifier, searchResponse.Identifiers[0]);

        client.Dispose();
    }

    [Fact]
    [Trait("Category", "Generic")]
    public void TestAddDeleteSearch()
    {
        string[] keywords = { "keyword1", "keyword2", "keyword3" };
        var identifer = "id1";

        var client = new D1GenericClient(d1Endpoint, d1ClientOptions);

        var createUserResponse = client.CreateUser(allScopes);
        client.Login(createUserResponse.UserId, createUserResponse.Password);

        var addResponse = client.Add(keywords, identifer);
        var deleteResponse = client.Delete(keywords, identifer);
        var searchResponse = client.Search(keywords[0]);
        Assert.Equal(0, searchResponse.Identifiers.Length);

        client.Dispose();
    }

    [Fact]
    [Trait("Category", "Storage")]
    public void TestStore()
    {
        var plaintext = System.Text.Encoding.ASCII.GetBytes("plaintext");
        var associatedData = System.Text.Encoding.ASCII.GetBytes("associatedData");
        var updatedPlaintext = System.Text.Encoding.ASCII.GetBytes("updatedPlaintext");
        var updatedAssociatedData = System.Text.Encoding.ASCII.GetBytes("updatedAssociatedData");

        var client = new D1StorageClient(d1Endpoint, credentials);

        var createUserResponse = client.Authn.CreateUser(allScopes);
        using var client2 = new D1StorageClient(d1Endpoint, new UsernamePasswordCredentials(d1Endpoint, createUserResponse.UserId, createUserResponse.Password));

        var storeResponse = client2.Storage.Store(plaintext, associatedData);
        var retrieveResponse = client2.Storage.Retrieve(storeResponse.ObjectId);
        Assert.Equal(plaintext, retrieveResponse.Plaintext);
        Assert.Equal(associatedData, retrieveResponse.AssociatedData);

        client2.Storage.Update(storeResponse.ObjectId, updatedPlaintext, updatedAssociatedData);
        var retrieveResponseAfterUpdate = client2.Storage.Retrieve(storeResponse.ObjectId);
        Assert.Equal(updatedPlaintext, retrieveResponseAfterUpdate.Plaintext);
        Assert.Equal(updatedAssociatedData, retrieveResponseAfterUpdate.AssociatedData);

        client2.Storage.Delete(storeResponse.ObjectId);
        var e = Assert.Throws<Grpc.Core.RpcException>(() => client2.Storage.Retrieve(storeResponse.ObjectId));
        Assert.Equal(Grpc.Core.StatusCode.NotFound, e.StatusCode);
    }

    [Fact]
    [Trait("Category", "Storage")]
    public void TestPermissions()
    {
        var plaintext = System.Text.Encoding.ASCII.GetBytes("plaintext");
        var associatedData = System.Text.Encoding.ASCII.GetBytes("associatedData");

        using var client = new D1StorageClient(d1Endpoint, credentials);

        var createUserResponse = client.Authn.CreateUser(allScopes);

        using var client2 = new D1StorageClient(d1Endpoint, new UsernamePasswordCredentials(d1Endpoint, createUserResponse.UserId, createUserResponse.Password));

        var storeResponse = client2.Storage.Store(plaintext, associatedData);

        client2.Authz.AddPermission(storeResponse.ObjectId, createUserResponse.UserId);
        var getPermissionsResponse = client2.Authz.GetPermissions(storeResponse.ObjectId);
        Assert.Contains(createUserResponse.UserId, getPermissionsResponse.GroupIds);

        client2.Authz.RemovePermission(storeResponse.ObjectId, createUserResponse.UserId);
        var e = Assert.Throws<Grpc.Core.RpcException>(() => client2.Authz.GetPermissions(storeResponse.ObjectId));
        Assert.Equal(Grpc.Core.StatusCode.PermissionDenied, e.StatusCode);
    }

    [Fact]
    public void TestClientRefreshToken()
    {
        var client = new D1GenericClient(d1Endpoint, credentials);

        credentials.ExpiryTime = DateTime.Now;

        client.Version.Version();

        client.Dispose();
    }
}
