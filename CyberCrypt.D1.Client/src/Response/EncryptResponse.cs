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

namespace CyberCrypt.D1.Client.Response;

/// <summary>
/// Response from <see cref="ServiceClients.ID1Encrypt.Encrypt"/> or <see cref="ServiceClients.ID1Encrypt.EncryptAsync"/>.
/// </summary>
public class EncryptResponse {
    /// <summary>
    /// Gets the object id.
    /// </summary>
    public string ObjectId { get; private set; }

    /// <summary>
    /// Gets the ciphertext.
    /// </summary>
    public byte[] Ciphertext { get; private set; }

    /// <summary>
    /// Gets the associated data.
    /// </summary>
    public byte[] AssociatedData { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EncryptResponse"/>.
    /// </summary>
    /// <param name="objectId">The object id.</param>
    /// <param name="ciphertext">The ciphertext.</param>
    /// <param name="associatedData">The associated data.</param>
    public EncryptResponse(string objectId, byte[] ciphertext, byte[] associatedData) {
        ObjectId = objectId;
        Ciphertext = ciphertext;
        AssociatedData = associatedData;
    }
}
