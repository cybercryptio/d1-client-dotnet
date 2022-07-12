// Copyright 2020-2022 CYBERCRYPT
using CyberCrypt.D1.Client.Credentials;
using CyberCrypt.D1.Client.ServiceClients;

namespace CyberCrypt.D1.Client;

/// <summary>
/// Interface for D1 Storage service client
/// </summary>
public interface ID1Storage : ID1Base
{
    /// <summary>
    /// The storage client.
    /// </summary>
    ID1Store Storage { get; }
}

/// <summary>
/// Client for connection to a D1 Storage server.
/// </summary>
/// <remarks>
/// Login is done on-demand and the access token is automatically refreshed when it expires.
/// </remarks>
public class D1StorageClient : D1BaseClient, ID1Storage
{
    /// <inheritdoc />
    public ID1Store Storage { get; private set; }

    /// <summary>
    /// Initialize a new instance of the <see cref="D1StorageClient"/> class.
    /// </summary>
    /// <param name="endpoint">The endpoint of the D1 server.</param>
    /// <param name="options">Client options <see cref="D1ClientOptions" />.</param>
    /// <param name="credentials">Credentials used to authenticate with D1.</param>
    /// <returns>A new instance of the <see cref="D1StorageClient"/> class.</returns>
    public D1StorageClient(string endpoint, ID1Credentials credentials, D1ClientOptions? options = null)
        : base(endpoint, credentials, options)
    {
        Storage = new D1StoreClient(channel);
    }
}
