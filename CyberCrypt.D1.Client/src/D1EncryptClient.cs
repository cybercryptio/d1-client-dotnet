using CyberCrypt.D1.Client.Credentials;
using CyberCrypt.D1.Client.Response;
using Grpc.Core;
using Google.Protobuf;
using Grpc.Net.Client;

namespace CyberCrypt.D1.Client;

/// <summary>
/// Interface for Encrypt client
/// </summary>
public interface ID1EncryptClient
{
    /// <summary>
    /// Decrypt an encrypted object.
    /// </summary>
    /// <param name="objectId">The ID of the object.</param>
    /// <param name="ciphertext">The ciphertext.</param>
    /// <param name="associatedData">The associated data attached to the data.</param>
    /// <returns>An instance of <see cref="DecryptResponse" />.</returns>
    DecryptResponse Decrypt(string objectId, byte[] ciphertext, byte[] associatedData);

    /// <inheritdoc cref="Decrypt"/>
    Task<DecryptResponse> DecryptAsync(string objectId, byte[] ciphertext, byte[] associatedData);

    /// <summary>
    /// Encrypt an object with associated data.
    /// </summary>
    /// <param name="plaintext">The plaintext to encrypt.</param>
    /// <param name="associatedData">The attached associated data.</param>
    /// <returns>An instance of <see cref="EncryptResponse" />.</returns>
    EncryptResponse Encrypt(byte[] plaintext, byte[] associatedData);

    /// <inheritdoc cref="Encrypt"/>
    Task<EncryptResponse> EncryptAsync(byte[] plaintext, byte[] associatedData);
}

/// <summary>
/// Encrypt client for connection to a D1 server.
/// </summary>
public class D1EncryptClient : ID1EncryptClient
{
    private readonly Protobuf.Generic.EncryptClient client;
    private readonly ICredentials credentials;

    /// <summary>
    /// Initialize a new instance of the <see cref="D1EncryptClient"/> class.
    /// </summary>
    /// <param name="channel">gRPC channel.</param>
    /// <param name="credentials">Credentials used to authenticate with D1.</param>
    /// <returns>A new instance of the <see cref="D1EncryptClient"/> class.</returns>
    public D1EncryptClient(GrpcChannel channel, ICredentials credentials)
    {
        client = new(channel);
        this.credentials = credentials;
    }

    /// <inheritdoc />
    public async Task<EncryptResponse> EncryptAsync(byte[] plaintext, byte[] associatedData)
    {
        var token = await credentials.GetTokenAsync();
        var metadata = new Metadata();
        metadata.Add("Authorization", $"Bearer {token}");
        var response = await client.EncryptAsync(new Protobuf.EncryptRequest
        {
            Plaintext = ByteString.CopyFrom(plaintext),
            AssociatedData = ByteString.CopyFrom(associatedData)
        }, metadata).ConfigureAwait(false);

        return new EncryptResponse(response.ObjectId, response.Ciphertext.ToByteArray(), response.AssociatedData.ToByteArray());
    }

    /// <inheritdoc />
    public EncryptResponse Encrypt(byte[] plaintext, byte[] associatedData)
    {
        var token = credentials.GetToken();
        var metadata = new Metadata();
        metadata.Add("Authorization", $"Bearer {token}");
        var response = client.Encrypt(new Protobuf.EncryptRequest
        {
            Plaintext = ByteString.CopyFrom(plaintext),
            AssociatedData = ByteString.CopyFrom(associatedData)
        }, metadata);

        return new EncryptResponse(response.ObjectId, response.Ciphertext.ToByteArray(), response.AssociatedData.ToByteArray());
    }

    /// <inheritdoc />
    public async Task<DecryptResponse> DecryptAsync(string objectId, byte[] ciphertext, byte[] associatedData)
    {
        var token = await credentials.GetTokenAsync();
        var metadata = new Metadata();
        metadata.Add("Authorization", $"Bearer {token}");
        var response = await client.DecryptAsync(new Protobuf.DecryptRequest
        {
            ObjectId = objectId,
            Ciphertext = ByteString.CopyFrom(ciphertext),
            AssociatedData = ByteString.CopyFrom(associatedData)
        }, metadata).ConfigureAwait(false);

        return new DecryptResponse(response.Plaintext.ToByteArray(), response.AssociatedData.ToByteArray());
    }

    /// <inheritdoc />
    public DecryptResponse Decrypt(string objectId, byte[] ciphertext, byte[] associatedData)
    {
        var token = credentials.GetToken();
        var metadata = new Metadata();
        metadata.Add("Authorization", $"Bearer {token}");
        var response = client.Decrypt(new Protobuf.DecryptRequest
        {
            ObjectId = objectId,
            Ciphertext = ByteString.CopyFrom(ciphertext),
            AssociatedData = ByteString.CopyFrom(associatedData)
        }, metadata);

        return new DecryptResponse(response.Plaintext.ToByteArray(), response.AssociatedData.ToByteArray());
    }
}