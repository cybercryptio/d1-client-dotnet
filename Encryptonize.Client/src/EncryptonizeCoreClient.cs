// Copyright 2020-2022 CYBERCRYPT
using System.Runtime.CompilerServices;
using Google.Protobuf;
using Encryptonize.Client.Response;

[assembly: InternalsVisibleTo("Encryptonize.Client.Tests")]

namespace Encryptonize.Client;

/// <summary>
/// Interface for Encryption Core service client
/// </summary>
public interface IEncryptonizeCore : IEncryptonizeBase
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
/// Client for connection to an Encryptonize Core server.
/// </summary>
/// <remarks>
/// Login is done on-demand and the access token is automatically refreshed when it expires.
/// </remarks>
public class EncryptonizeCoreClient : EncryptonizeBaseClient, IEncryptonizeCore
{
    private Protobuf.Core.CoreClient coreClient;

    /// <summary>
    /// Initialize a new instance of the <see cref="EncryptonizeCoreClient"/> class.
    /// </summary>
    /// <param name="endpoint">The endpoint of the Encryptonize server.</param>
    /// <param name="username">The username used to authenticate with the Encryptonize server.</param>
    /// <param name="password">The password used to authenticate with the Encryptonize server.</param>
    /// <param name="certPath">The optional path to the certificate used to authenticate with the Encryptonize server when mTLS is enabled.</param>
    /// <returns>A new instance of the <see cref="EncryptonizeCoreClient"/> class.</returns>
    public EncryptonizeCoreClient(string endpoint, string username, string password, string certPath = "")
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
