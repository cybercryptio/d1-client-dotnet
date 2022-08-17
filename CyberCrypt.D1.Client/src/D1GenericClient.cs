// Copyright 2020-2022 CYBERCRYPT
using System.Runtime.CompilerServices;
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
    ID1Encrypt Generic { get; }
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
    public ID1Encrypt Generic { get; private set; }

    /// <summary>
    /// Initialize a new instance of the <see cref="D1GenericClient"/> class.
    /// </summary>
    /// <param name="d1Channel">The <see cref="D1Channel"/> to use.</param>
    /// <returns>A new instance of the <see cref="D1GenericClient"/> class.</returns>
    public D1GenericClient(D1Channel d1Channel) : base(d1Channel)
    {
        Generic = new D1EncryptClient(base.channel);
    }
}
