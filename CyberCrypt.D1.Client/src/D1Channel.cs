// Copyright 2022 CYBERCRYPT
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// 	http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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

    /// <summary>
    /// Initializes a new instance of the <see cref="D1Channel"/> class.
    /// </summary>
    /// <param name="endpoint">The endpoint of the D1 server.</param>
    /// <param name="callCredentials">The call credentials.</param>
    /// <returns>The <see cref="D1Channel"/>.</returns>
    public D1Channel(Uri endpoint, ID1CallCredentials callCredentials)
    {
        this.endpoint = endpoint;
        this.d1CallCredentials = callCredentials ?? throw new ArgumentNullException(nameof(callCredentials));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="D1Channel"/> class.
    /// </summary>
    /// <param name="endpoint">The endpoint of the D1 server.</param>
    /// <param name="username">The username.</param>
    /// <param name="password">The password.</param>
    /// <returns>The <see cref="D1Channel"/>.</returns>
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