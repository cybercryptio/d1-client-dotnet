// Copyright 2020-2022 CYBERCRYPT
using System.Runtime.CompilerServices;
using Google.Protobuf;
using CyberCrypt.D1.Client.Response;

[assembly: InternalsVisibleTo("CyberCrypt.D1.Client.Tests")]

namespace CyberCrypt.D1.Client;

/// <summary>
/// Interface for D1 Generic service client
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
/// Client for connection to a D1 Generic server.
/// </summary>
/// <remarks>
/// Login is done on-demand and the access token is automatically refreshed when it expires.
/// </remarks>
public class D1GenericClient : D1BaseClient, ID1Generic
{
    private Protobuf.Generic.GenericClient genericClient;

    /// <summary>
    /// Initialize a new instance of the <see cref="D1GenericClient"/> class.
    /// </summary>
    /// <param name="endpoint">The endpoint of the D1 server.</param>
    /// <param name="options">Client options <see cref="D1ClientOptions" />.</param>
    /// <returns>A new instance of the <see cref="D1GenericClient"/> class.</returns>
    public D1GenericClient(string endpoint, D1ClientOptions options)
        : base(endpoint, options)
    {
        genericClient = new(channel);
    }

    /// <inheritdoc />
    public async Task<EncryptResponse> EncryptAsync(byte[] plaintext, byte[] associatedData)
    {
        await RefreshTokenAsync().ConfigureAwait(false);

        var response = await genericClient.EncryptAsync(new Protobuf.EncryptRequest
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

        var response = genericClient.Encrypt(new Protobuf.EncryptRequest
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

        var response = await genericClient.DecryptAsync(new Protobuf.DecryptRequest
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

        var response = genericClient.Decrypt(new Protobuf.DecryptRequest
        {
            ObjectId = objectId,
            Ciphertext = ByteString.CopyFrom(ciphertext),
            AssociatedData = ByteString.CopyFrom(associatedData)
        }, requestHeaders);

        return new DecryptResponse(response.Plaintext.ToByteArray(), response.AssociatedData.ToByteArray());
    }
}
