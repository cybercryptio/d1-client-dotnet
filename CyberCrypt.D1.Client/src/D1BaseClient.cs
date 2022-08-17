// Copyright 2020-2022 CYBERCRYPT
using Grpc.Net.Client;
using CyberCrypt.D1.Client.ServiceClients;

namespace CyberCrypt.D1.Client;

/// <summary>
/// Interface for Encryption service client
/// </summary>
public interface ID1Base
{
    /// <summary>
    /// The authn client.
    /// </summary>
    ID1Authn Authn { get; }

    /// <summary>
    /// The authz client.
    /// </summary>
    ID1Authz Authz { get; }

    /// <summary>
    /// The index client.
    /// </summary>
    ID1Index Index { get; }

    /// <summary>
    /// The version client.
    /// </summary>
    ID1Version Version { get; }
}

/// <summary>
/// Client for connection to a D1 server.
/// </summary>
/// <remarks>
/// Login is done on-demand and the access token is automatically refreshed when it expires.
/// </remarks>
public abstract class D1BaseClient : IDisposable, IAsyncDisposable, ID1Base
{
    /// <summary>
    /// Grpc channel used for communication.
    /// </summary>
    protected GrpcChannel channel;

    /// <inheritdoc />
    public string? User { get; private set; }

    /// <inheritdoc />
    public ID1Authn Authn { get; private set; }

    /// <inheritdoc />
    public ID1Authz Authz { get; private set; }

    /// <inheritdoc />
    public ID1Index Index { get; private set; }

    /// <inheritdoc />
    public ID1Version Version { get; private set; }

    /// <summary>
    /// Initialize a new instance of the <see cref="D1BaseClient"/> class.
    /// </summary>
    /// <param name="d1Channel">The <see cref="D1Channel"/> to use.</param>
    /// <returns>A new instance of the <see cref="D1BaseClient"/> class.</returns>
    protected D1BaseClient(D1Channel d1Channel)
    {
        channel = d1Channel.Build();
        Authn = new D1AuthnClient(channel);
        Authz = new D1AuthzClient(channel);
        Index = new D1IndexClient(channel);
        Version = new D1VersionClient(channel);
    }

    /// <summary>
    /// Dispose the client.
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Dispose the client.
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            channel.Dispose();
        }
    }

    /// <summary>
    /// Asynchronously dispose the client.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);

        Dispose(disposing: false);
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
        GC.SuppressFinalize(this);
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
    }

    /// <summary>
    /// Asynchronously dispose the client.
    /// </summary>
    protected virtual async ValueTask DisposeAsyncCore()
    {
        await channel.ShutdownAsync().ConfigureAwait(false);
    }
}
