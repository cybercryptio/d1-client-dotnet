// Copyright 2020-2022 CYBERCRYPT
namespace Encryptonize.Client.Response;

public class RetrieveResponse {
    private byte[] plaintext;
    public byte[] Plaintext { get { return plaintext; } }
    private byte[] associatedData;
    public byte[] AssociatedData { get { return associatedData; } }

    public RetrieveResponse(byte[] plaintext, byte[] associatedData) {
        this.plaintext = plaintext;
        this.associatedData = associatedData;
    }
}
