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

using CyberCrypt.D1.Client.Response;
using Google.Protobuf;
using Grpc.Net.Client;

namespace CyberCrypt.D1.Client.ServiceClients;

/// <summary>
/// Interface for Encrypt client
/// </summary>
public interface ID1Encrypt
{
    /// <summary>
    /// Decrypt an encrypted object.
    /// </summary>
    /// <param name="objectId">The ID of the object.</param>
    /// <param name="ciphertext">The ciphertext.</param>
    /// <param name="associatedData">The associated data attached to the data.</param>
    /// <returns>An instance of <see cref="DecryptResponse" />.</returns>
    DecryptResponse Decrypt(string objectId, byte[] ciphertext, byte[] associatedData);

    /// <inheritdoc cref="Decrypt"/>
    Task<DecryptResponse> DecryptAsync(string objectId, byte[] ciphertext, byte[] associatedData);

    /// <summary>
    /// Encrypt an object with associated data.
    /// </summary>
    /// <param name="plaintext">The plaintext to encrypt.</param>
    /// <param name="associatedData">The attached associated data.</param>
    /// <param name="groupIds">The group IDs that should have access to the object.</param>
    /// <returns>An instance of <see cref="EncryptResponse" />.</returns>
    EncryptResponse Encrypt(byte[] plaintext, byte[] associatedData, IEnumerable<string>? groupIds = null);

    /// <inheritdoc cref="Encrypt"/>
    Task<EncryptResponse> EncryptAsync(byte[] plaintext, byte[] associatedData, IEnumerable<string>? groupIds = null);
}

/// <summary>
/// Encrypt client for connection to a D1 server.
/// </summary>
public class D1EncryptClient : ID1Encrypt
{
    private readonly Protobuf.Generic.Generic.GenericClient client;

    /// <summary>
    /// Initialize a new instance of the <see cref="D1EncryptClient"/> class.
    /// </summary>
    /// <param name="channel">gRPC channel.</param>
    /// <returns>A new instance of the <see cref="D1EncryptClient"/> class.</returns>
    public D1EncryptClient(GrpcChannel channel)
    {
        client = new(channel);
    }

    /// <inheritdoc />
    public async Task<EncryptResponse> EncryptAsync(byte[] plaintext, byte[] associatedData, IEnumerable<string>? groupIds = null)
    {
        var request = new Protobuf.Generic.EncryptRequest
        {
            Plaintext = ByteString.CopyFrom(plaintext),
            AssociatedData = ByteString.CopyFrom(associatedData)
        };
        if (groupIds is not null) {
            request.GroupIds.AddRange(groupIds);
        }
        var response = await client.EncryptAsync(request).ConfigureAwait(false);

        return new EncryptResponse(response.ObjectId, response.Ciphertext.ToByteArray(), response.AssociatedData.ToByteArray());
    }

    /// <inheritdoc />
    public EncryptResponse Encrypt(byte[] plaintext, byte[] associatedData, IEnumerable<string>? groupIds = null)
    {
        var request = new Protobuf.Generic.EncryptRequest
        {
            Plaintext = ByteString.CopyFrom(plaintext),
            AssociatedData = ByteString.CopyFrom(associatedData)
        };
        if (groupIds is not null) {
            request.GroupIds.AddRange(groupIds);
        }

        var response = client.Encrypt(request);

        return new EncryptResponse(response.ObjectId, response.Ciphertext.ToByteArray(), response.AssociatedData.ToByteArray());
    }

    /// <inheritdoc />
    public async Task<DecryptResponse> DecryptAsync(string objectId, byte[] ciphertext, byte[] associatedData)
    {
        var response = await client.DecryptAsync(new Protobuf.Generic.DecryptRequest
        {
            ObjectId = objectId,
            Ciphertext = ByteString.CopyFrom(ciphertext),
            AssociatedData = ByteString.CopyFrom(associatedData)
        }).ConfigureAwait(false);

        return new DecryptResponse(response.Plaintext.ToByteArray(), response.AssociatedData.ToByteArray());
    }

    /// <inheritdoc />
    public DecryptResponse Decrypt(string objectId, byte[] ciphertext, byte[] associatedData)
    {
        var response = client.Decrypt(new Protobuf.Generic.DecryptRequest
        {
            ObjectId = objectId,
            Ciphertext = ByteString.CopyFrom(ciphertext),
            AssociatedData = ByteString.CopyFrom(associatedData)
        });

        return new DecryptResponse(response.Plaintext.ToByteArray(), response.AssociatedData.ToByteArray());
    }
}
