// Copyright 2020-2022 CYBERCRYPT
using System.Runtime.CompilerServices;
using Google.Protobuf;
using CyberCrypt.D1.Client.Response;
using Protobuf = Encryptonize.Client.Protobuf;

[assembly: InternalsVisibleTo("CyberCrypt.D1.Client.Tests")]

namespace CyberCrypt.D1.Client;

/// <summary>
/// Interface for Encryption Core service client
/// </summary>
public interface ID1Generic : ID1Base
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
/// Client for connection to an D1 Generic server.
/// </summary>
/// <remarks>
/// Login is done on-demand and the access token is automatically refreshed when it expires.
/// </remarks>
public class D1GenericClient : D1BaseClient, ID1Generic
{
    private Protobuf.Core.CoreClient coreClient;

    /// <summary>
    /// Initialize a new instance of the <see cref="D1GenericClient"/> class.
    /// </summary>
    /// <param name="endpoint">The endpoint of the D1 server.</param>
    /// <param name="username">The username used to authenticate with the D1 server.</param>
    /// <param name="password">The password used to authenticate with the D1 server.</param>
    /// <param name="certPath">The optional path to the certificate used to authenticate with the D1 server when mTLS is enabled.</param>
    /// <returns>A new instance of the <see cref="D1GenericClient"/> class.</returns>
    public D1GenericClient(string endpoint, string username, string password, string certPath = "")
        : base(endpoint, username, password, certPath)
    {
        coreClient = new(channel);
    }

    /// <inheritdoc />
    public async Task<EncryptResponse> EncryptAsync(byte[] plaintext, byte[] associatedData)
    {
        await RefreshTokenAsync().ConfigureAwait(false);

        var response = await coreClient.EncryptAsync(new Protobuf.EncryptRequest
        {
            Plaintext = ByteString.CopyFrom(plaintext),
            AssociatedData = ByteString.CopyFrom(associatedData)
        }, requestHeaders).ConfigureAwait(false);

        return new EncryptResponse(response.ObjectId, response.Ciphertext.ToByteArray(), response.AssociatedData.ToByteArray());
    }

    /// <inheritdoc />
    public EncryptResponse Encrypt(byte[] plaintext, byte[] associatedData)
    {
        RefreshToken();

        var response = coreClient.Encrypt(new Protobuf.EncryptRequest
        {
            Plaintext = ByteString.CopyFrom(plaintext),
            AssociatedData = ByteString.CopyFrom(associatedData)
        }, requestHeaders);

        return new EncryptResponse(response.ObjectId, response.Ciphertext.ToByteArray(), response.AssociatedData.ToByteArray());
    }

    /// <inheritdoc />
    public async Task<DecryptResponse> DecryptAsync(string objectId, byte[] ciphertext, byte[] associatedData)
    {
        await RefreshTokenAsync().ConfigureAwait(false);

        var response = await coreClient.DecryptAsync(new Protobuf.DecryptRequest
        {
            ObjectId = objectId,
            Ciphertext = ByteString.CopyFrom(ciphertext),
            AssociatedData = ByteString.CopyFrom(associatedData)
        }, requestHeaders)
            .ConfigureAwait(false);

        return new DecryptResponse(response.Plaintext.ToByteArray(), response.AssociatedData.ToByteArray());
    }

    /// <inheritdoc />
    public DecryptResponse Decrypt(string objectId, byte[] ciphertext, byte[] associatedData)
    {
        RefreshToken();

        var response = coreClient.Decrypt(new Protobuf.DecryptRequest
        {
            ObjectId = objectId,
            Ciphertext = ByteString.CopyFrom(ciphertext),
            AssociatedData = ByteString.CopyFrom(associatedData)
        }, requestHeaders);

        return new DecryptResponse(response.Plaintext.ToByteArray(), response.AssociatedData.ToByteArray());
    }
}
