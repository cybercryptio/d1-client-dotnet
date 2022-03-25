// Copyright 2020-2022 CYBERCRYPT
namespace Encryptonize.Client.Response;

public class DecryptResponse {
    public byte[] Plaintext { get; private set; }
    public byte[] AssociatedData { get; private set; }

    public DecryptResponse(byte[] Plaintext, byte[] AssociatedData) {
        this.Plaintext = Plaintext;
        this.AssociatedData = AssociatedData;
    }
}
