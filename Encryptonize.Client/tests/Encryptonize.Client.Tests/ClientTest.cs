// Copyright 2020-2022 CYBERCRYPT
using System;
using System.Collections.Generic;
using Xunit;
using Encryptonize.Client.Utils;

namespace Encryptonize.Client.Tests;

public class ClientTest {
    private string encryptonizeUser;
    private string encryptonizePassword;
    private string encryptonizeClientCert;
    private string encryptonizeEndpoint;
    private List<Scope> allScopes = new List<Scope>{Scope.Read, Scope.Create, Scope.Index, Scope.ObjectPermissions, Scope.UserManagement,
    Scope.Update, Scope.Delete};

    public ClientTest() {
        encryptonizeUser = Environment.GetEnvironmentVariable("E2E_TEST_UID") ?? throw new ArgumentNullException("E2E_TEST_UID must be set");
        encryptonizePassword = Environment.GetEnvironmentVariable("E2E_TEST_PASS") ?? throw new ArgumentNullException("E2E_TEST_PASS must be set");
        encryptonizeClientCert = Environment.GetEnvironmentVariable("E2E_TEST_CERT") ?? "";
        encryptonizeEndpoint = Environment.GetEnvironmentVariable("E2E_TEST_URL") ?? "http://127.0.0.1:9000";
    }

    [Fact]
    public void TestClientConnection() {
        Client? client = null;
        try {
            client = new Client(encryptonizeEndpoint, encryptonizeClientCert);
            client.Login(encryptonizeUser, encryptonizePassword);

            client.Version();
        } finally {
            if (client != null) {
                client.CloseConnection();
            }
        }
    }

    [Fact]
    public void TestUserManagement() {
        Client? client = null;
        try {
            client = new Client(encryptonizeEndpoint, encryptonizeClientCert);
            client.Login(encryptonizeUser, encryptonizePassword);

            var createUserResponse = client.CreateUser(allScopes);
            var createGroupResponse = client.CreateGroup(allScopes);

            client.AddUserToGroup(createUserResponse.UserId, createGroupResponse.GroupId);
            client.RemoveUserFromGroup(createUserResponse.UserId, createGroupResponse.GroupId);

            client.RemoveUser(createUserResponse.UserId);
        } finally {
            if (client != null) {
                client.CloseConnection();
            }
        }
    }

    [Fact]
    public void TestEncryption() {
        var plaintext = System.Text.Encoding.ASCII.GetBytes("plaintext");
        var associatedData = System.Text.Encoding.ASCII.GetBytes("associatedData");

        Client? client = null;
        try {
            client = new Client(encryptonizeEndpoint, encryptonizeClientCert);
            client.Login(encryptonizeUser, encryptonizePassword);

            var createUserResponse = client.CreateUser(allScopes);
            client.Login(createUserResponse.UserId, createUserResponse.Password);

            var encryptResponse = client.Encrypt(plaintext, associatedData);
            var decryptResponse = client.Decrypt(encryptResponse.ObjectId, encryptResponse.Ciphertext, encryptResponse.AssociatedData);
            Assert.Equal(plaintext, decryptResponse.Plaintext);
            Assert.Equal(associatedData, decryptResponse.AssociatedData);
        } finally {
            if (client != null) {
                client.CloseConnection();
            }
        }
    }

    [Fact]
    public void TestStore() {
        var plaintext = System.Text.Encoding.ASCII.GetBytes("plaintext");
        var associatedData = System.Text.Encoding.ASCII.GetBytes("associatedData");
        var updatedPlaintext = System.Text.Encoding.ASCII.GetBytes("updatedPlaintext");
        var updatedAssociatedData = System.Text.Encoding.ASCII.GetBytes("updatedAssociatedData");

        Client? client = null;
        try {
            client = new Client(encryptonizeEndpoint, encryptonizeClientCert);
            client.Login(encryptonizeUser, encryptonizePassword);

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
            try {
                var retrieveResponseAfterDelete = client.Retrieve(storeResponse.ObjectId);
            } catch (Grpc.Core.RpcException e) {
                Assert.Equal("NotFound", e.StatusCode.ToString());
            }
        } finally {
            if (client != null) {
                client.CloseConnection();
            }
        }
    }

    [Fact]
    public void TestPermissions() {
        var plaintext = System.Text.Encoding.ASCII.GetBytes("plaintext");
        var associatedData = System.Text.Encoding.ASCII.GetBytes("associatedData");

        Client? client = null;
        try {
            client = new Client(encryptonizeEndpoint, encryptonizeClientCert);
            client.Login(encryptonizeUser, encryptonizePassword);

            var createUserResponse = client.CreateUser(allScopes);
            client.Login(createUserResponse.UserId, createUserResponse.Password);

            var storeResponse = client.Store(plaintext, associatedData);

            client.AddPermission(storeResponse.ObjectId, createUserResponse.UserId);
            var getPermissionsResponse = client.GetPermissions(storeResponse.ObjectId);
            Assert.Contains(createUserResponse.UserId, getPermissionsResponse.GroupIds);

            client.RemovePermission(storeResponse.ObjectId, createUserResponse.UserId);
            try {
                var getPermissionsResponseAfterRemovePermission = client.GetPermissions(storeResponse.ObjectId);
            } catch (Grpc.Core.RpcException e) {
                Assert.Equal("PermissionDenied", e.StatusCode.ToString());
            }
        } finally {
            if (client != null) {
                client.CloseConnection();
            }
        }
    }
}
