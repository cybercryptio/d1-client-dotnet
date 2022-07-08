// Copyright 2020-2022 CYBERCRYPT
using System.Security.Cryptography.X509Certificates;
using Grpc.Net.Client;
using Grpc.Core;
using CyberCrypt.D1.Client.Utils;
using CyberCrypt.D1.Client.Response;
using CyberCrypt.D1.Client.Credentials;

namespace CyberCrypt.D1.Client;

/// <summary>
/// Interface for Encryption service client
/// </summary>
public interface ID1Base
{
    /// <summary>
    /// The authn client.
    /// </summary>
    ID1AuthnClient Authn { get; }

    /// <summary>
    /// The authz client.
    /// </summary>
    ID1AuthzClient Authz { get; }

    /// <summary>
    /// The version client.
    /// </summary>
    ID1VersionClient Version { get; }
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
    public ID1AuthnClient Authn { get; private set; }

    /// <inheritdoc />
    public ID1AuthzClient Authz { get; private set; }

    /// <inheritdoc />
    public ID1VersionClient Version { get; private set; }

    /// <summary>
    /// Initialize a new instance of the <see cref="D1BaseClient"/> class.
    /// </summary>
    /// <param name="endpoint">The endpoint of the D1 server.</param>
    /// <param name="options">Client options <see cref="D1ClientOptions" />.</param>
    /// <param name="credentials">Credentials used to authenticate with D1.</param>
    /// <returns>A new instance of the <see cref="D1BaseClient"/> class.</returns>
    protected D1BaseClient(string endpoint, ID1Credentials credentials, D1ClientOptions? options = null)
    {
        if (credentials is null)
        {
            throw new ArgumentNullException(nameof(credentials));
        }

        if (options == null)
        {
            options = new D1ClientOptions();
        }

        var callCredentials = CallCredentials.FromInterceptor(async (context, metadata) =>
        {
            var token = await credentials.GetTokenAsync();
            metadata.Add("Authorization", $"Bearer {token}");
        });

        if (string.IsNullOrWhiteSpace(options.CertPath))
        {
            var grpcCredentials = new D1CompositeCredentials(ChannelCredentials.Insecure, callCredentials);
            channel = GrpcChannel.ForAddress(endpoint, new GrpcChannelOptions
            {
                Credentials = grpcCredentials,
                UnsafeUseInsecureChannelCallCredentials = true
            });
        }
        else
        {
            var cert = new X509Certificate2(File.ReadAllBytes(options.CertPath));
            var handler = new HttpClientHandler();
            handler.ClientCertificates.Add(cert);

            var grpcCredentials = new D1CompositeCredentials(ChannelCredentials.SecureSsl, callCredentials);
            channel = GrpcChannel.ForAddress(endpoint, new GrpcChannelOptions { HttpHandler = handler, Credentials = grpcCredentials });
        }

        Authn = new D1AuthnClient(channel);
        Authz = new D1AuthzClient(channel);
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