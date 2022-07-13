// Copyright 2020-2022 CYBERCRYPT
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
