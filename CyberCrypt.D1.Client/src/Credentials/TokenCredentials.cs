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

namespace CyberCrypt.D1.Client.Credentials;

/// <summary>
/// Credentials based off a user provide token.
/// </summary>
public class TokenCredentials : ID1CallCredentials
{
    private readonly string? token;

    /// <summary>
    /// Initialize a new instance of the <see cref="TokenCredentials"/> class.
    /// </summary>
    /// <param name="token">The access token.</param>
    /// <returns>A new instance of the <see cref="TokenCredentials"/> class.</returns>
    public TokenCredentials(string token)
    {
        this.token = token;
    }

    /// <inheritdoc />
    public string? GetToken()
    {
        return token;
    }

    /// <inheritdoc />
    public Task<string?> GetTokenAsync()
    {
        return Task.FromResult(token);
    }
}