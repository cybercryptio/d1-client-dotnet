// Copyright 2020-2022 CYBERCRYPT
using Google.Protobuf;
using CyberCrypt.D1.Client.Response;

namespace CyberCrypt.D1.Client;

/// <summary>
/// Interface for D1 Storage service client
/// </summary>
public interface ID1Storage : ID1Base
{
    /// <summary>
    /// Delete data encrypted in the storage attached to D1.
    /// </summary>
    /// <param name="objectId">The ID of the object.</param>
    void Delete(string objectId);

    /// <inheritdoc cref="Delete"/>
    Task DeleteAsync(string objectId);

    /// <summary>
    /// Retreive some data encrypted in the storage attached to D1.
    /// </summary>
    /// <param name="objectId">The ID of the object.</param>
    /// <returns>An instance of <see cref="RetrieveResponse" />.</returns>
    RetrieveResponse Retrieve(string objectId);

    /// <inheritdoc cref="Retrieve"/>
    Task<RetrieveResponse> RetrieveAsync(string objectId);

    /// <summary>
    /// Store some data encrypted in the storage attached to D1.
    /// </summary>
    /// <param name="plaintext">The plaintext to store.</param>
    /// <param name="associatedData">The attached associated data.</param>
    /// <returns>An instance of <see cref="StoreResponse" />.</returns>
    StoreResponse Store(byte[] plaintext, byte[] associatedData);

    /// <inheritdoc cref="Store"/>
    Task<StoreResponse> StoreAsync(byte[] plaintext, byte[] associatedData);

    /// <summary>
    /// Update some data stored in the storage attached to D1.
    /// </summary>
    /// <param name="objectId">The ID of the object.</param>
    /// <param name="plaintext">The plaintext to store.</param>
    /// <param name="associatedData">The attached associated data.</param>
    void Update(string objectId, byte[] plaintext, byte[] associatedData);

    /// <inheritdoc cref="Update"/>
    Task UpdateAsync(string objectId, byte[] plaintext, byte[] associatedData);
}

/// <summary>
/// Client for connection to a D1 Storage server.
/// </summary>
/// <remarks>
/// Login is done on-demand and the access token is automatically refreshed when it expires.
/// </remarks>
public class D1StorageClient : D1BaseClient, ID1Storage
{
    private Protobuf.Storage.StorageClient storageClient;

    /// <summary>
    /// Initialize a new instance of the <see cref="D1StorageClient"/> class.
    /// </summary>
    /// <param name="endpoint">The endpoint of the D1 server.</param>
    /// <param name="username">The username used to authenticate with the D1 server.</param>
    /// <param name="password">The password used to authenticate with the D1 server.</param>
    /// <param name="certPath">The optional path to the certificate used to authenticate with the D1 server when mTLS is enabled.</param>
    /// <returns>A new instance of the <see cref="D1StorageClient"/> class.</returns>
    public D1StorageClient(string endpoint, string username, string password, string certPath = "")
        : base(endpoint, username, password, certPath)
    {
        storageClient = new(channel);
    }

    /// <inheritdoc />
    public async Task<StoreResponse> StoreAsync(byte[] plaintext, byte[] associatedData)
    {
        await RefreshTokenAsync().ConfigureAwait(false);

        var response = await storageClient.StoreAsync(new Protobuf.StoreRequest
        {
            Plaintext = ByteString.CopyFrom(plaintext),
            AssociatedData = ByteString.CopyFrom(associatedData)
        }, requestHeaders).ConfigureAwait(false);

        return new StoreResponse(response.ObjectId);
    }

    /// <inheritdoc />
    public StoreResponse Store(byte[] plaintext, byte[] associatedData)
    {
        RefreshToken();

        var response = storageClient.Store(new Protobuf.StoreRequest
        {
            Plaintext = ByteString.CopyFrom(plaintext),
            AssociatedData = ByteString.CopyFrom(associatedData)
        }, requestHeaders);

        return new StoreResponse(response.ObjectId);
    }

    /// <inheritdoc />
    public async Task<RetrieveResponse> RetrieveAsync(string objectId)
    {
        await RefreshTokenAsync().ConfigureAwait(false);

        var response = await storageClient.RetrieveAsync(new Protobuf.RetrieveRequest { ObjectId = objectId }, requestHeaders).ConfigureAwait(false);

        return new RetrieveResponse(response.Plaintext.ToByteArray(), response.AssociatedData.ToByteArray());
    }

    /// <inheritdoc />
    public RetrieveResponse Retrieve(string objectId)
    {
        RefreshToken();

        var response = storageClient.Retrieve(new Protobuf.RetrieveRequest { ObjectId = objectId }, requestHeaders);

        return new RetrieveResponse(response.Plaintext.ToByteArray(), response.AssociatedData.ToByteArray());
    }

    /// <inheritdoc />
    public async Task UpdateAsync(string objectId, byte[] plaintext, byte[] associatedData)
    {
        await RefreshTokenAsync().ConfigureAwait(false);

        await storageClient.UpdateAsync(new Protobuf.UpdateRequest
        {
            ObjectId = objectId,
            Plaintext = ByteString.CopyFrom(plaintext),
            AssociatedData = ByteString.CopyFrom(associatedData)
        }, requestHeaders).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public void Update(string objectId, byte[] plaintext, byte[] associatedData)
    {
        RefreshToken();

        storageClient.Update(new Protobuf.UpdateRequest
        {
            ObjectId = objectId,
            Plaintext = ByteString.CopyFrom(plaintext),
            AssociatedData = ByteString.CopyFrom(associatedData)
        }, requestHeaders);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(string objectId)
    {
        await RefreshTokenAsync().ConfigureAwait(false);

        await storageClient.DeleteAsync(new Protobuf.DeleteRequest { ObjectId = objectId }, requestHeaders).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public void Delete(string objectId)
    {
        RefreshToken();

        storageClient.Delete(new Protobuf.DeleteRequest { ObjectId = objectId }, requestHeaders);
    }
}
