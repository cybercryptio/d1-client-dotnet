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

using System.Security.Cryptography.X509Certificates;
using Grpc.Net.Client;

namespace CyberCrypt.D1.Client.Credentials;

/// <summary>
/// Credentials from username and password.
/// </summary>
public class UsernamePasswordCredentials : ID1CallCredentials
{
    private readonly Protobuf.Authn.Authn.AuthnClient client;
    private readonly string username;
    private readonly string password;
    private string? accessToken;

    /// <summary>
    /// The token expiration time.
    /// </summary>
    public DateTime ExpiryTime { get; internal set; }


    /// <summary>
    /// Initialize a new instance of the <see cref="UsernamePasswordCredentials"/> class.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <param name="password">The password.</param>
    /// <param name="channel">The <see cref="GrpcChannel"/> to use.</param>
    /// <returns>A new instance of the <see cref="UsernamePasswordCredentials"/> class.</returns>
    public UsernamePasswordCredentials(string username, string password, GrpcChannel channel)
    {
        if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
        {
            this.username = username;
            this.password = password;
        }
        else
        {
            throw new ArgumentNullException("username and password must be provided");
        }

        client = new(channel);
    }

    /// <inheritdoc />
    public string? GetToken()
    {
        if (DateTime.UtcNow.AddMinutes(1) > ExpiryTime)
        {
            var req = client.LoginUser(new Protobuf.Authn.LoginUserRequest { UserId = username, Password = password });
            accessToken = req.AccessToken;
            ExpiryTime = DateTimeOffset.FromUnixTimeSeconds(req.ExpiryTime).UtcDateTime;
        }

        return accessToken;
    }

    /// <inheritdoc />
    public async Task<string?> GetTokenAsync()
    {
        if (DateTime.UtcNow.AddMinutes(1) > ExpiryTime)
        {
            var req = await client.LoginUserAsync(new Protobuf.Authn.LoginUserRequest { UserId = username, Password = password }).ConfigureAwait(false);
            accessToken = req.AccessToken;
            ExpiryTime = DateTimeOffset.FromUnixTimeSeconds(req.ExpiryTime).UtcDateTime;
        }

        return accessToken;
    }
}
