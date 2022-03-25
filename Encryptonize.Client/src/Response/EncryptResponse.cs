// Copyright 2020-2022 CYBERCRYPT
namespace Encryptonize.Client.Response;

public class EncryptResponse {
    public string ObjectId { get; private set; }
    public byte[] Ciphertext { get; private set; }
    public byte[] AssociatedData { get; private set; }

    public EncryptResponse(string ObjectId, byte[] Ciphertext, byte[] AssociatedData) {
        this.ObjectId = ObjectId;
        this.Ciphertext = Ciphertext;
        this.AssociatedData = AssociatedData;
    }
}
