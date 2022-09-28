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
    /// <param name="d1Channel">The <see cref="D1Channel"/> to use.</param>
    /// <returns>A new instance of the <see cref="D1StorageClient"/> class.</returns>
    public D1StorageClient(D1Channel d1Channel) : base(d1Channel)
    {
        Storage = new D1StoreClient(base.channel);
    }
}
