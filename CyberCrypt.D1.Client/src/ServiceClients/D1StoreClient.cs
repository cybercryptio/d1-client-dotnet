using CyberCrypt.D1.Client.Response;
using Google.Protobuf;
using Grpc.Net.Client;

namespace CyberCrypt.D1.Client.ServiceClients;

/// <summary>
/// Interface for Store client
/// </summary>
public interface ID1StoreClient
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
/// Store client for connection to a D1 server.
/// </summary>
public class D1StoreClient : ID1StoreClient
{
    private readonly Protobuf.Storage.StorageClient client;

    /// <summary>
    /// Initialize a new instance of the <see cref="D1StoreClient"/> class.
    /// </summary>
    /// <param name="channel">gRPC channel.</param>
    /// <returns>A new instance of the <see cref="D1StoreClient"/> class.</returns>
    public D1StoreClient(GrpcChannel channel)
    {
        client = new(channel);
    }

    /// <inheritdoc />
    public async Task<StoreResponse> StoreAsync(byte[] plaintext, byte[] associatedData)
    {
        var response = await client.StoreAsync(new Protobuf.StoreRequest
        {
            Plaintext = ByteString.CopyFrom(plaintext),
            AssociatedData = ByteString.CopyFrom(associatedData)
        }).ConfigureAwait(false);

        return new StoreResponse(response.ObjectId);
    }

    /// <inheritdoc />
    public StoreResponse Store(byte[] plaintext, byte[] associatedData)
    {
        var response = client.Store(new Protobuf.StoreRequest
        {
            Plaintext = ByteString.CopyFrom(plaintext),
            AssociatedData = ByteString.CopyFrom(associatedData)
        });

        return new StoreResponse(response.ObjectId);
    }

    /// <inheritdoc />
    public async Task<RetrieveResponse> RetrieveAsync(string objectId)
    {
        var response = await client.RetrieveAsync(new Protobuf.RetrieveRequest { ObjectId = objectId }).ConfigureAwait(false);

        return new RetrieveResponse(response.Plaintext.ToByteArray(), response.AssociatedData.ToByteArray());
    }

    /// <inheritdoc />
    public RetrieveResponse Retrieve(string objectId)
    {
        var response = client.Retrieve(new Protobuf.RetrieveRequest { ObjectId = objectId });

        return new RetrieveResponse(response.Plaintext.ToByteArray(), response.AssociatedData.ToByteArray());
    }

    /// <inheritdoc />
    public async Task UpdateAsync(string objectId, byte[] plaintext, byte[] associatedData)
    {
        await client.UpdateAsync(new Protobuf.UpdateRequest
        {
            ObjectId = objectId,
            Plaintext = ByteString.CopyFrom(plaintext),
            AssociatedData = ByteString.CopyFrom(associatedData)
        }).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public void Update(string objectId, byte[] plaintext, byte[] associatedData)
    {
        client.Update(new Protobuf.UpdateRequest
        {
            ObjectId = objectId,
            Plaintext = ByteString.CopyFrom(plaintext),
            AssociatedData = ByteString.CopyFrom(associatedData)
        });
    }

    /// <inheritdoc />
    public async Task DeleteAsync(string objectId)
    {
        await client.DeleteAsync(new Protobuf.DeleteRequest { ObjectId = objectId }).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public void Delete(string objectId)
    {
        client.Delete(new Protobuf.DeleteRequest { ObjectId = objectId });
    }
}
