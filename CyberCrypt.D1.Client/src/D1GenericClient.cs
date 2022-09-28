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
