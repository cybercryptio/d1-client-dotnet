// Copyright 2020-2022 CYBERCRYPT
namespace Encryptonize.Client.Response;

public class StoreResponse {
    private string objectId;
    public string ObjectId { get { return objectId; } }

    public StoreResponse(string objectId) {
        this.objectId = objectId;
    }
}
