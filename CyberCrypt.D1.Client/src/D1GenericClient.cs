// Copyright 2020-2022 CYBERCRYPT
using System.Runtime.CompilerServices;
using CyberCrypt.D1.Client.Credentials;
using CyberCrypt.D1.Client.ServiceClients;

[assembly: InternalsVisibleTo("CyberCrypt.D1.Client.Tests")]

namespace CyberCrypt.D1.Client;

/// <summary>
/// Interface for D1 Generic service client
/// </summary>
public interface ID1Generic : ID1Base
{
    /// <summary>
    /// The encrypt client.
    /// </summary>
    ID1EncryptClient Generic { get; }
}

/// <summary>
/// Client for connection to a D1 Generic server.
/// </summary>
/// <remarks>
/// Login is done on-demand and the access token is automatically refreshed when it expires.
/// </remarks>
public class D1GenericClient : D1BaseClient, ID1Generic
{

    /// <inheritdoc />
    public ID1EncryptClient Generic { get; private set; }

    /// <summary>
    /// Initialize a new instance of the <see cref="D1GenericClient"/> class.
    /// </summary>
    /// <param name="endpoint">The endpoint of the D1 server.</param>
    /// <param name="options">Client options <see cref="D1ClientOptions" />.</param>
    /// <param name="credentials">Credentials used to authenticate with D1.</param>
    /// <returns>A new instance of the <see cref="D1GenericClient"/> class.</returns>
    public D1GenericClient(string endpoint, ID1Credentials credentials, D1ClientOptions? options = null)
        : base(endpoint, credentials, options)
    {
        Generic = new D1EncryptClient(channel);
    }
}
