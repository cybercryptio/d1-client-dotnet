// Copyright 2020-2022 CYBERCRYPT
using Encryptonize.Client.Utils;
using Encryptonize.Client.Response;

namespace Encryptonize.Client;

// ClientRT automatically refreshes the Client access token after it expires.
public class ClientRT : Client {
    public string User { get; private set; }
    public string Password { get; private set; }

    private ClientRT(string endpoint, string certPath =  "") : base(endpoint, certPath) {
        User = "";
        Password = "";
    }

    public static async Task<ClientRT> New(string endpoint, string User, string Password, string certPath =  "") {
        var client = new ClientRT(endpoint, certPath);
        await client.Login(User, Password).ConfigureAwait(false);
        return client;
    }

    private async Task RefreshToken() {
        if (DateTime.Compare(DateTime.Now, ExpiryTime.AddMinutes(-1)) > 0) {
            await Login(User, Password).ConfigureAwait(false);
        }
    }

    /////////////////////////////////////////////////////////////////////////
    //                               Utility                               //
    /////////////////////////////////////////////////////////////////////////

    public override async Task<VersionResponse> Version() {
        await RefreshToken().ConfigureAwait(false);
        return await base.Version().ConfigureAwait(false);
    }

    /////////////////////////////////////////////////////////////////////////
    //                           User Management                           //
    /////////////////////////////////////////////////////////////////////////

    public override async Task Login(string user, string password) {
        await base.Login(user, password).ConfigureAwait(false);
        User = user;
        Password = password;
    }

    public override async Task<CreateUserResponse> CreateUser(IList<Scope> scopes) {
        await RefreshToken().ConfigureAwait(false);
        return await base.CreateUser(scopes).ConfigureAwait(false);
    }

    public override async Task RemoveUser(string userId) {
        await RefreshToken().ConfigureAwait(false);
        await base.RemoveUser(userId).ConfigureAwait(false);
    }

    public override async Task<CreateGroupResponse> CreateGroup(IList<Scope> scopes) {
        await RefreshToken().ConfigureAwait(false);
        return await base.CreateGroup(scopes).ConfigureAwait(false);
    }

    public override async Task AddUserToGroup(string userId, string groupId) {
        await RefreshToken().ConfigureAwait(false);
        await base.AddUserToGroup(userId, groupId).ConfigureAwait(false);
    }

    public override async Task RemoveUserFromGroup(string userId, string groupId) {
        await RefreshToken().ConfigureAwait(false);
        await base.RemoveUserFromGroup(userId, groupId).ConfigureAwait(false);
    }

    /////////////////////////////////////////////////////////////////////////
    //                              Encryption                             //
    /////////////////////////////////////////////////////////////////////////

    public override async Task<EncryptResponse> Encrypt(byte[] plaintext, byte[] associatedData) {
        await RefreshToken().ConfigureAwait(false);
        return await base.Encrypt(plaintext, associatedData).ConfigureAwait(false);
    }

    public override async Task<DecryptResponse> Decrypt(string objectId, byte[] ciphertext, byte[] associatedData) {
        await RefreshToken().ConfigureAwait(false);
        return await base.Decrypt(objectId, ciphertext, associatedData).ConfigureAwait(false);
    }

    /////////////////////////////////////////////////////////////////////////
    //                               Storage                               //
    /////////////////////////////////////////////////////////////////////////

    public override async Task<StoreResponse> Store(byte[] plaintext, byte[] associatedData) {
        await RefreshToken().ConfigureAwait(false);
        return await base.Store(plaintext, associatedData).ConfigureAwait(false);
    }

    public override async Task<RetrieveResponse> Retrieve(string objectId) {
        await RefreshToken().ConfigureAwait(false);
        return await base.Retrieve(objectId).ConfigureAwait(false);
    }

    public override async Task Update(string objectId, byte[] plaintext, byte[] associatedData) {
        await RefreshToken().ConfigureAwait(false);
        await base.Update(objectId, plaintext, associatedData).ConfigureAwait(false);
    }

    public override async Task Delete(string objectId) {
        await RefreshToken().ConfigureAwait(false);
        await base.Delete(objectId).ConfigureAwait(false);
    }

    /////////////////////////////////////////////////////////////////////////
    //                             Permissions                             //
    /////////////////////////////////////////////////////////////////////////

    public override async Task<GetPermissionsResponse> GetPermissions(string objectId) {
        await RefreshToken().ConfigureAwait(false);
        return await base.GetPermissions(objectId).ConfigureAwait(false);
    }

    public override async Task AddPermission(string objectId, string groupId) {
        await RefreshToken().ConfigureAwait(false);
        await base.AddPermission(objectId, groupId).ConfigureAwait(false);
    }

    public override async Task RemovePermission(string objectId, string groupId) {
        await RefreshToken().ConfigureAwait(false);
        await base.RemovePermission(objectId, groupId).ConfigureAwait(false);
    }
}
