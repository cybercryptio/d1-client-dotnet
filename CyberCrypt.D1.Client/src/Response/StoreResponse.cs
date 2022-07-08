// Copyright 2020-2022 CYBERCRYPT
namespace CyberCrypt.D1.Client.Response;

/// <summary>
/// Response from <see cref="ID1StoreClient.Store"/> or <see cref="ID1StoreClient.StoreAsync"/>.
/// </summary>
public class StoreResponse {
    /// <summary>
    /// Gets the object id.
    /// </summary>
    public string ObjectId { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="StoreResponse"/>.
    /// </summary>
    /// <param name="objectId">The object id.</param>
    public StoreResponse(string objectId) {
        ObjectId = objectId;
    }
}
