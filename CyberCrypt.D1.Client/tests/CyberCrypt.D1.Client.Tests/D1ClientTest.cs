// Copyright 2020-2022 CYBERCRYPT
using System;
using System.Collections.Generic;
using Xunit;
using CyberCrypt.D1.Client.Utils;
using CyberCrypt.D1.Client.Credentials;
using Grpc.Core;

namespace CyberCrypt.D1.Client.Tests;

public class D1ClientTest
{
    private string d1User;
    private string d1Password;
    private Uri d1Endpoint;
    private readonly D1Channel d1Channel;

    public D1ClientTest()
    {
        d1User = Environment.GetEnvironmentVariable("E2E_TEST_UID") ?? throw new ArgumentNullException("E2E_TEST_UID must be set");
        d1Password = Environment.GetEnvironmentVariable("E2E_TEST_PASS") ?? throw new ArgumentNullException("E2E_TEST_PASS must be set");
        d1Endpoint = new Uri(Environment.GetEnvironmentVariable("E2E_TEST_URL") ?? "http://127.0.0.1:9000");
        d1Channel = new D1Channel(d1Endpoint, d1User, d1Password) { ChannelCredentials = ChannelCredentials.Insecure };
    }

    private D1GenericClient CreateD1GenericClient(string username, string password)
    {
        return new D1GenericClient(new D1Channel(d1Endpoint, username, password) { ChannelCredentials = ChannelCredentials.Insecure });
    }

    private D1StorageClient CreateD1StorageClient(string username, string password)
    {
        return new D1StorageClient(new D1Channel(d1Endpoint, username, password) { ChannelCredentials = ChannelCredentials.Insecure });
    }

    [Fact]
    public async void TestClientConnectionAsync()
    {
        using var client = new D1GenericClient(d1Channel);

        await client.Version.VersionAsync().ConfigureAwait(false);

        await client.DisposeAsync();
    }

    [Fact]
    public async void TestUserManagementAsync()
    {
        using var client = new D1GenericClient(d1Channel);

        var createUserResponse = await client.Authn.CreateUserAsync(new List<Scope> { }).ConfigureAwait(false);
        var createGroupResponse = await client.Authn.CreateGroupAsync(new List<Scope> { }).ConfigureAwait(false);

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

        using var client = new D1GenericClient(d1Channel);

        var createUserResponse = await client.Authn.CreateUserAsync(new List<Scope> { Scope.Read, Scope.Create }).ConfigureAwait(false);
        using var client2 = CreateD1GenericClient(createUserResponse.UserId, createUserResponse.Password);

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

        using var client = new D1StorageClient(d1Channel);

        var createUserResponse = await client.Authn.CreateUserAsync(new List<Scope> { Scope.Read, Scope.Create, Scope.Update, Scope.Delete }).ConfigureAwait(false);
        using var client2 = CreateD1StorageClient(createUserResponse.UserId, createUserResponse.Password);

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

        using var client = new D1StorageClient(d1Channel);

        var createUserResponse = await client.Authn.CreateUserAsync(new List<Scope> { Scope.Create, Scope.GetAccess, Scope.ModifyAccess }).ConfigureAwait(false);
        using var client2 = CreateD1StorageClient(createUserResponse.UserId, createUserResponse.Password);

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
    public async void TestAddToIndexAsync()
    {
        string[] keywords = { "keyword1", "keyword2", "keyword3" };
        List<string> keywordsRange = new List<string>(keywords);
        var identifier = "id1";

        using var client = new D1GenericClient(d1Channel);

        var createUserResponse = await client.Authn.CreateUserAsync(new List<Scope> { Scope.Index }).ConfigureAwait(false);
        using var client2 = CreateD1GenericClient(createUserResponse.UserId, createUserResponse.Password);

        await client2.Index.AddAsync(keywordsRange, identifier).ConfigureAwait(false);

        foreach (var keyword in keywordsRange)
        {
            var searchResponse = await client2.Index.SearchAsync(keyword).ConfigureAwait(false);
            Assert.Equal(identifier, searchResponse.Identifiers[0]);
        }

        await client.DisposeAsync();
    }

    [Fact]
    public async void TestAddToDeleteFromIndexAsync()
    {
        string[] keywords = { "keyword1", "keyword2", "keyword3" };
        List<string> keywordsRange = new List<string>(keywords);
        var identifier = "id1";

        using var client = new D1GenericClient(d1Channel);

        var createUserResponse = await client.Authn.CreateUserAsync(new List<Scope> { Scope.Index }).ConfigureAwait(false);
        using var client2 = CreateD1GenericClient(createUserResponse.UserId, createUserResponse.Password);

        await client2.Index.AddAsync(keywordsRange, identifier).ConfigureAwait(false);
        await client2.Index.DeleteAsync(keywordsRange, identifier).ConfigureAwait(false);

        foreach (var keyword in keywordsRange)
        {
            var searchResponse = await client2.Index.SearchAsync(keyword).ConfigureAwait(false);
            Assert.Equal(0, searchResponse.Identifiers.Count);
        }

        await client.DisposeAsync();
    }

    [Fact]
    public void TestClientConnection()
    {
        var client = new D1GenericClient(d1Channel);

        client.Version.Version();

        client.Dispose();
    }

    [Fact]
    public void TestUserManagement()
    {
        using var client = new D1GenericClient(d1Channel);

        var createUserResponse = client.Authn.CreateUser(new List<Scope> { });
        var createGroupResponse = client.Authn.CreateGroup(new List<Scope> { });

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

        var client = new D1GenericClient(d1Channel);

        var createUserResponse = client.Authn.CreateUser(new List<Scope> { Scope.Read, Scope.Create });
        using var client2 = CreateD1GenericClient(createUserResponse.UserId, createUserResponse.Password);

        var encryptResponse = client2.Generic.Encrypt(plaintext, associatedData);
        var decryptResponse = client2.Generic.Decrypt(encryptResponse.ObjectId, encryptResponse.Ciphertext, encryptResponse.AssociatedData);
        Assert.Equal(plaintext, decryptResponse.Plaintext);
        Assert.Equal(associatedData, decryptResponse.AssociatedData);

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

        using var client = new D1StorageClient(d1Channel);

        var createUserResponse = client.Authn.CreateUser(new List<Scope> { Scope.Read, Scope.Create, Scope.Update, Scope.Delete });
        using var client2 = CreateD1StorageClient(createUserResponse.UserId, createUserResponse.Password);

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

        using var client = new D1StorageClient(d1Channel);

        var createUserResponse = client.Authn.CreateUser(new List<Scope> { Scope.Create, Scope.GetAccess, Scope.ModifyAccess });
        using var client2 = CreateD1StorageClient(createUserResponse.UserId, createUserResponse.Password);

        var storeResponse = client2.Storage.Store(plaintext, associatedData);

        client2.Authz.AddPermission(storeResponse.ObjectId, createUserResponse.UserId);
        var getPermissionsResponse = client2.Authz.GetPermissions(storeResponse.ObjectId);
        Assert.Contains(createUserResponse.UserId, getPermissionsResponse.GroupIds);

        client2.Authz.RemovePermission(storeResponse.ObjectId, createUserResponse.UserId);
        var e = Assert.Throws<Grpc.Core.RpcException>(() => client2.Authz.GetPermissions(storeResponse.ObjectId));
        Assert.Equal(Grpc.Core.StatusCode.PermissionDenied, e.StatusCode);
    }

    [Fact]
    public void TestAddToIndex()
    {
        string[] keywords = { "keyword1", "keyword2", "keyword3" };
        List<string> keywordsRange = new List<string>(keywords);
        var identifier = "id1";

        using var client = new D1GenericClient(d1Channel);

        var createUserResponse = client.Authn.CreateUser(new List<Scope> { Scope.Index });
        using var client2 = CreateD1GenericClient(createUserResponse.UserId, createUserResponse.Password);

        client2.Index.Add(keywordsRange, identifier);

        foreach (var keyword in keywordsRange)
        {
            var searchResponse = client2.Index.Search(keyword);
            Assert.Equal(identifier, searchResponse.Identifiers[0]);
        }

        client.Dispose();
    }

    [Fact]
    public void TestAddToDeleteFromIndex()
    {
        string[] keywords = { "keyword1", "keyword2", "keyword3" };
        List<string> keywordsRange = new List<string>(keywords);
        string identifer = "id1";

        using var client = new D1GenericClient(d1Channel);

        var createUserResponse = client.Authn.CreateUser(new List<Scope> { Scope.Index });
        using var client2 = CreateD1GenericClient(createUserResponse.UserId, createUserResponse.Password);

        client2.Index.Add(keywordsRange, identifer);
        client2.Index.Delete(keywordsRange, identifer);

        foreach (var keyword in keywordsRange)
        {
            var searchResponse = client2.Index.Search(keyword);
            Assert.Equal(0, searchResponse.Identifiers.Count);
        }

        client.Dispose();
    }
}
