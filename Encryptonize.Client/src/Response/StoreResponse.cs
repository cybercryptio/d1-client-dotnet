// Copyright 2020-2022 CYBERCRYPT
namespace Encryptonize.Client.Response;

public class StoreResponse {
    public string ObjectId { get; private set; }

    public StoreResponse(string ObjectId) {
        this.ObjectId = ObjectId;
    }
}
