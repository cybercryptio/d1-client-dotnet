// Copyright 2020-2022 CYBERCRYPT
using CyberCrypt.D1.Client.Credentials;
using Grpc.Core;
using Grpc.Net.Client;

namespace CyberCrypt.D1.Client;

/// <summary>
/// Channel for communication with the D1 server.
/// </summary>
public class D1Channel
{
    private readonly Uri endpoint;
    private readonly string? username;
    private readonly string? password;
    internal ID1CallCredentials? d1CallCredentials;

    public D1Channel(Uri endpoint, ID1CallCredentials callCredentials)
    {
        this.endpoint = endpoint;
        this.d1CallCredentials = callCredentials ?? throw new ArgumentNullException(nameof(callCredentials));
    }

    public D1Channel(Uri endpoint, string username, string password)
    {
        if (string.IsNullOrEmpty(username))
        {
            throw new ArgumentException($"'{nameof(username)}' cannot be null or empty.", nameof(username));
        }

        if (string.IsNullOrEmpty(password))
        {
            throw new ArgumentException($"'{nameof(password)}' cannot be null or empty.", nameof(password));
        }

        this.endpoint = endpoint;
        this.username = username;
        this.password = password;
    }

    /// <summary>
    /// The <see cref="HttpClientHandler"/> to use. If not specified, a default one is created.
    /// </summary>
    /// <remarks>
    /// This can for example be used to enable mTLS.
    /// </remarks>
    public HttpClientHandler? HttpClientHandler { get; set; }

    /// <summary>
    /// The <see cref="ChannelCredentials"/> to use. Must be specified.
    /// </summary>
    public ChannelCredentials ChannelCredentials { get; set; } = ChannelCredentials.SecureSsl;

    /// <summary>
    /// Create the gRPC channel.
    /// </summary>
    public GrpcChannel Build()
    {
        if (d1CallCredentials is null)
        {
            var rpcChannel = GrpcChannel.ForAddress(endpoint, new GrpcChannelOptions
            {
                HttpHandler = HttpClientHandler,
                Credentials = ChannelCredentials,
            });
            d1CallCredentials = new UsernamePasswordCredentials(username!, password!, rpcChannel);
        }

        var callCredentials = CallCredentials.FromInterceptor(async (context, metadata) =>
            {
                var token = await d1CallCredentials.GetTokenAsync();
                metadata.Add("Authorization", $"Bearer {token}");
            });
        var grpcCredentials = new D1CompositeCredentials(ChannelCredentials, callCredentials);
        return GrpcChannel.ForAddress(endpoint, new GrpcChannelOptions
        {
            HttpHandler = HttpClientHandler,
            Credentials = grpcCredentials,
            UnsafeUseInsecureChannelCallCredentials = true
        });
    }
}