// Copyright 2020-2022 CYBERCRYPT
namespace Encryptonize.Client.Response;

public class EncryptResponse {
    private string objectId;
    public string ObjectId { get { return objectId; } }
    private byte[] ciphertext;
    public byte[] Ciphertext { get { return ciphertext; } }
    private byte[] associatedData;
    public byte[] AssociatedData { get { return associatedData; } }

    public EncryptResponse(string objectId, byte[] ciphertext, byte[] associatedData) {
        this.objectId = objectId;
        this.ciphertext = ciphertext;
        this.associatedData = associatedData;
    }
}
